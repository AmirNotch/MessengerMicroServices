﻿using System.Diagnostics.Metrics;

namespace WebSockets.Metrics;

public class UserWSMetrics
{
    private readonly Gauge<int> _activeConnectionGauge;
    private readonly Counter<int> _inputWebSocketMessage;
    private readonly Counter<int> _outputWebSocketMessage;

    public UserWSMetrics(IMeterFactory metricFactory)
    {
        var meter = metricFactory.Create("Messenger.WebSocket");
        _activeConnectionGauge = meter.CreateGauge<int>("backend_Messenger.websocket_active_connection");
        _activeConnectionGauge.Record(0);
        _inputWebSocketMessage = meter.CreateCounter<int>("backend_Messenger.input_web_socket_message");
        _inputWebSocketMessage.Add(0);
        _outputWebSocketMessage = meter.CreateCounter<int>("backend_Messenger.output_web_socket_message");
        _outputWebSocketMessage.Add(0);
    }

    public void WebSocketConnectionUpdate(int count)
    {
        _activeConnectionGauge.Record(count);
    }
    public void AddOutputWebSocketMessage()
    {
        _outputWebSocketMessage.Add(1);
    }
    public void AddInputWebSocketMessage()
    {
        _inputWebSocketMessage.Add(1);
    }
}