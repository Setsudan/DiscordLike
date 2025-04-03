using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordLikeChatApp.Services {
    public class StompWebSocketClientService : IDisposable {
        private ClientWebSocket _client;
        private CancellationTokenSource _cts;

        // Event fired when a complete STOMP frame is received.
        public event Action<string> MessageReceived;

        //   for sending messages.
        public string Destination {
            get; set;
        }

        public async Task ConnectAsync(string uri) {
            _client = new ClientWebSocket();
            _cts = new CancellationTokenSource();
            await _client.ConnectAsync(new Uri(uri), _cts.Token);

            // Send a STOMP CONNECT frame
            string connectFrame = "CONNECT\naccept-version:1.2\nheart-beat:10000,10000\n\n\0";
            await SendFrameAsync(connectFrame);

            // Optionally wait for a CONNECTED frame (simplistic parsing)
            string response = await ReceiveFrameAsync();
            if (response == null || !response.StartsWith("CONNECTED")) {
                throw new Exception("STOMP CONNECT failed: " + response);
            }

            // Start the receive loop in the background.
            _ = Task.Run(ReceiveLoopAsync);
        }

        // New method: accepts a message string, creates a JSON payload, and sends it.
        public async Task SendJsonMessageAsync(string messageText, string bearerToken) {
            if (_client.State != WebSocketState.Open)
                throw new InvalidOperationException("WebSocket is not connected.");

            if (string.IsNullOrEmpty(Destination))
                throw new InvalidOperationException("Destination is not set.");

            if (string.IsNullOrEmpty(bearerToken))
                throw new InvalidOperationException("Bearer token is not provided.");

            // Create a JSON payload; adjust the properties as needed.
            var payloadObj = new {
                content = messageText
            };
            string jsonPayload = JsonSerializer.Serialize(payloadObj);

            // Create a STOMP SEND frame with Authorization header.
            string frame = $"SEND\ndestination:{Destination}\nAuthorization: Bearer {bearerToken}\ncontent-length:{Encoding.UTF8.GetByteCount(jsonPayload)}\n\n{jsonPayload}\0";
            await SendFrameAsync(frame);
        }

        private async Task SendFrameAsync(string frame) {
            byte[] frameBytes = Encoding.UTF8.GetBytes(frame);
            var segment = new ArraySegment<byte>(frameBytes);
            await _client.SendAsync(segment, WebSocketMessageType.Text, true, _cts.Token);
        }

        private async Task<string> ReceiveFrameAsync() {
            var buffer = new byte[4096];
            var messageBuilder = new StringBuilder();
            WebSocketReceiveResult result;

            do {
                result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                if (result.MessageType == WebSocketMessageType.Close) {
                    await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", _cts.Token);
                    return null;
                }
                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            } while (!messageBuilder.ToString().Contains("\0")); // End-of-frame marker

            return messageBuilder.ToString().TrimEnd('\0');
        }

        private async Task ReceiveLoopAsync() {
            try {
                while (_client.State == WebSocketState.Open) {
                    string frame = await ReceiveFrameAsync();
                    if (frame != null) {
                        MessageReceived?.Invoke(frame);
                    }
                }
            }
            catch (OperationCanceledException) {
                // Expected on disconnect.
            }
            catch (Exception ex) {
                Console.WriteLine("WebSocket STOMP error: " + ex.Message);
            }
        }

        public async Task DisconnectAsync() {
            if (_client != null && _client.State == WebSocketState.Open) {
                _cts.Cancel();
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }

        public void Dispose() {
            _cts?.Cancel();
            _client?.Dispose();
            _cts?.Dispose();
        }
    }
}
