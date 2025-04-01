using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordHeticWpf.Services
{
    public class WebSocketService
    {
        private ClientWebSocket _clientWebSocket;
        private readonly Uri _uri;
        private CancellationTokenSource _cts;

        // Event to notify subscribers when a message is received.
        public event Action<string> OnMessageReceived;

        public WebSocketService(string endpoint)
        {
            _uri = new Uri(endpoint);
            _clientWebSocket = new ClientWebSocket();
        }

        /// <summary>
        /// Connects to the WebSocket endpoint and starts the receive loop.
        /// </summary>
        public async Task ConnectAsync()
        {
            _cts = new CancellationTokenSource();
            await _clientWebSocket.ConnectAsync(_uri, _cts.Token);
            _ = ReceiveLoop(); // fire-and-forget the receive loop
        }

        /// <summary>
        /// Disconnects the WebSocket.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
            {
                _cts.Cancel();
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }

        /// <summary>
        /// Serializes the message to JSON and sends it.
        /// </summary>
        public async Task SendMessageAsync(object message)
        {
            var json = JsonConvert.SerializeObject(message);
            var encodedMessage = Encoding.UTF8.GetBytes(json);
            var buffer = new ArraySegment<byte>(encodedMessage);
            await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Continuously receives messages from the WebSocket and raises the OnMessageReceived event.
        /// </summary>
        private async Task ReceiveLoop()
        {
            var buffer = new byte[1024 * 4];
            try
            {
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        OnMessageReceived?.Invoke(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled (disconnecting), so exit gracefully.
            }
            catch (Exception ex)
            {
                // Log error if needed.
                Console.WriteLine("WebSocket receive error: " + ex.Message);
            }
        }
    }
}
