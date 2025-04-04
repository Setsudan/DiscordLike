using System.Net.WebSockets;
using System.Text;

namespace DiscordLikeChatApp.Services
{
  
        public class WebSocketClientService : IDisposable {
            private ClientWebSocket _client;
            private CancellationTokenSource _cts;

            // Event that fires when a message is received
            public event Action<string> MessageReceived;

            public async Task ConnectAsync(string uri) {
                _client = new ClientWebSocket();
                _cts = new CancellationTokenSource();
                await _client.ConnectAsync(new Uri(uri), _cts.Token);
                _ = Task.Run(ReceiveLoopAsync);
            }

            public async Task SendMessageAsync(string message) {
                if (_client.State != WebSocketState.Open)
                    throw new InvalidOperationException("WebSocket is not connected.");

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(messageBytes);
                await _client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }

            public async Task DisconnectAsync() {
                if (_client != null && _client.State == WebSocketState.Open) {
                    _cts.Cancel();
                    await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }

            private async Task ReceiveLoopAsync() {
                var buffer = new byte[4096];
                try {
                    while (_client.State == WebSocketState.Open) {
                        // Si le message est fragmenté, on l'accumule
                        var messageBuilder = new StringBuilder();
                        WebSocketReceiveResult result;
                        do {
                            result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                            if (result.MessageType == WebSocketMessageType.Close) {
                                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                                return;
                            }
                            messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        } while (!result.EndOfMessage);

                        string message = messageBuilder.ToString();
                        MessageReceived?.Invoke(message);
                    }
                }
                catch (OperationCanceledException) {
                    // Expected when disconnecting.
                }
                catch (Exception ex) {
                    Console.WriteLine("WebSocket error: " + ex.Message);
                }
            }

            public void Dispose() {
                _cts?.Cancel();
                _client?.Dispose();
                _cts?.Dispose();
            }
        }
    }



