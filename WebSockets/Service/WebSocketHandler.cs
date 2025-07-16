﻿using System.Net.WebSockets;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using WebSockets.Metrics;
using WebSockets.Models;
using WebSockets.Models.WsMessage;
using WebSockets.Validation;

namespace WebSockets.Service;

public class WebSocketHandler
{
    private readonly ILogger<WebSocketHandler> _logger;
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly ChatService _chatService;
    private readonly GroupService _groupService;
    private readonly UserWSMetrics _userWSMetrics;

    public WebSocketHandler(ILogger<WebSocketHandler> logger,
        JsonSerializerSettings jsonSerializerSettings,
        WebSocketConnectionManager connectionManager,
        ChatService chatService,
        GroupService groupService,
        UserWSMetrics userWSMetrics)
    {
        _connectionManager = connectionManager;
        _logger = logger;
        _jsonSerializerSettings = jsonSerializerSettings;
        _chatService = chatService;
        _groupService = groupService;
        _userWSMetrics = userWSMetrics;
    }

    public async Task HandleWebSocket(WebSocket webSocket, User user, CancellationToken ct)
    {
        bool isLoadSuccessful = await LoadUserInfo(webSocket, user, ct);
        if (!isLoadSuccessful)
        {
            return;
        }
        
        // Здесь на поставить REDIS соединение и поставить ключ user.UserId и статус Online = true
        string? errorMessage = _connectionManager.AddSocket(webSocket, user);
        if (errorMessage != null)
        {
            _logger.LogError($"Error while saving webSocket: {errorMessage}");
            return;
        }
        _logger.LogInformation($"WebSocket connection established for user {user.UserId}");

        try
        {
            await MainWsCycle(webSocket, user, ct);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, $"An exception occurred during WebSocket connection");
        }
        catch (ConnectionAbortedException ex)
        {
            _logger.LogInformation(ex, $"User {user.UserId} finalize before shutdown server");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Unknown exception.");
        }
        _connectionManager.RemoveSocket(user.UserId!.Value);
        try
        {
            FinalizeWsConnection(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An exception occurred while finalizing WebSocket connection");
        }

        _logger.LogInformation($"WebSocket connection closed for user {user.UserId}");
    }

    private async Task MainWsCycle(WebSocket ws, User user, CancellationToken ct)
    {
        var buffer = new Memory<byte>(new byte[1024]);
        await using MemoryStream dataStream = new MemoryStream();
        while (ws.State == WebSocketState.Open)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }
            string? message = await ReadMessageFromWebSocket(ws, buffer, dataStream, CancellationToken.None);
            if (message == null)
            {
                _logger.LogInformation($"The WebSocket has been already closed");
                break;
            }
            try
            {
                await ProcessWsMessageSafe(ws, user, message, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cannot process WebSocket message");
            }
            _neoDaoDbContext.ChangeTracker.Clear();
            // _validationStorage.Clear();
            dataStream.SetLength(0);
        }
    }

    private async Task ProcessWsMessageSafe(WebSocket ws, User user, string inputMessage, CancellationToken ct)
    {
        Guid userId = user.UserId!.Value;
        IncomingWsMessage? incomingMessage;
        try
        {
            incomingMessage = JsonConvert.DeserializeObject<IncomingWsMessage>(inputMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Cannot parse incoming message ${inputMessage}");
            incomingMessage = null;
        }
        if (incomingMessage == null)
        {
            await SendMessageToUser(ws, userId, _logger, "Error", "Error", null, ErrorCode.CannotParseMessage,
                "Cannot parse message", _jsonSerializerSettings, ct);
            return;
        }

        try
        {
            await ProcessWsMessage(ws, user, incomingMessage, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while processing ws message from user {user.UserId}");
            await SendMessageToUser(ws, userId, _logger, "Error", "Error", null, ErrorCode.InternalServerError,
                "Something went wrong", _jsonSerializerSettings, ct);
        }
    }

    private async Task ProcessWsMessage(WebSocket ws, User user, IncomingWsMessage incomingMessage, CancellationToken ct)
    {
        Guid userId = user.UserId!.Value;
        string? inputData = incomingMessage.Data;
        IOutputMessageData? outputData = null;
        _logger.LogInformation($"Incoming ws message: eventType={incomingMessage.EventType}, data={inputData}, timestamp={incomingMessage.Timestamp}");
        _userWSMetrics.AddInputWebSocketMessage();
        ErrorCode? errorCode;
        string? errorMessage;
        switch (incomingMessage.EventCategory)
        {
            // Тут происзодит обращение к Chat Service или как в проекте он называется Messenger для обращения в Direct Chats
            case EndpointCategory.Chat:
                (outputData, errorCode, errorMessage) = await _chatService.ProcessMessage(user, incomingMessage.EventType, incomingMessage.Data, ct);
                break;
            
            // Тут происзодит обращение к Chat Service или как в проекте он называется Messenger для обращения в Group Chats
           case EndpointCategory.Group:
                (outputData, errorCode, errorMessage) = await _groupService.ProcessMessage(user, incomingMessage.EventType, incomingMessage.Data, ct);
                break;
            default:
                errorCode = ErrorCode.WrongEndpointKind;
                errorMessage = $"Unknown event Category: {incomingMessage.EventCategory}";
                break;
        }
        _userWSMetrics.AddOutputWebSocketMessage();
        if (errorCode == null)
        {
            if (outputData != null)
            {
                _logger.LogInformation($"Outgoing message: eventType={incomingMessage.EventType}, data={outputData}");
                await SendMessageToUser(ws, userId, _logger, incomingMessage.EventType, incomingMessage.EventCategory.ToString(), outputData, _jsonSerializerSettings, ct);
            }
            else
            {
                _logger.LogInformation($"Outgoing message: eventType={incomingMessage.EventType}");
                await SendMessageToUser(ws, userId, _logger, incomingMessage.EventType, incomingMessage.EventCategory.ToString(), null, _jsonSerializerSettings, ct);
            }
        }
        else
        {
            _logger.LogInformation($"Outgoing message with error: eventType={incomingMessage.EventType}, data={outputData}" +
                    $", errorCode={errorCode}, errorMessage={errorMessage}");
            await SendMessageToUser(ws, userId, _logger, incomingMessage.EventType, incomingMessage.EventCategory.ToString(), outputData, errorCode, errorMessage,
                _jsonSerializerSettings, ct);
        }
    }

    private async Task<bool> LoadUserInfo(WebSocket ws, User user, CancellationToken ct)
    {
        GetMePayload? userInfo = await _userInfoService.LoadUserInfo(user, ct);
        if (userInfo == null)
        {
            _logger.LogError("Closing WebSocket because of errors");
            return false;
        }
        user.UserName = userInfo!.Name;
        user.UserId = userInfo.Id;
        await SendMessageToUser(ws, userInfo!.Id, _logger, "UserInfo", "UserInfo", userInfo!, _jsonSerializerSettings, ct);
        _userInfoService.UpdateIsOnlineStatus(user.UserId!.Value, true);
        return true;
    }

    private void FinalizeWsConnection(User user)
    {
        _groupService.FinalizeUserGroup(user.UserId!.Value, user.UserName!, CancellationToken.None).Wait();
        // _userInfoService.UpdateIsOnlineStatus(user.UserId!.Value, false);
        _logger.LogInformation($"User with id = {user.UserId} has offline status");
        
        //FinalzeUserSession only on production
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            _userInfoService.FinalizeUserSession(user, CancellationToken.None).Wait();
        }
        _chatService.RemoveUserFromAllChat(user);
        
        // Здесь надо поставить REDIS соединение и поставить ключ user.UserId и статус Online = false
    }
}