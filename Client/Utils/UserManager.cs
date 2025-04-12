using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Utils
{
    public static class UserManager
    {
        private static TcpClient _client;
        private static NetworkStream _stream;
        private static StreamReader _reader;
        private static SemaphoreSlim _lock = new(1, 1);
        private static bool _isClosed = false;

        public static bool IsConnected => _client?.Connected ?? false;

        public static async Task<bool> InitializeAsync(string username)
        {
            try
            {
                if (_client != null && (!_client.Connected || !_stream.CanRead || !_stream.CanWrite))
                {
                    Close();
                    await Task.Delay(200);
                }

                if (_client == null || !_client.Connected)
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync(ServerConfig.ServerIP, ServerConfig.ServerPort);
                    _stream = _client.GetStream();
                    _reader = new StreamReader(_stream, Encoding.UTF8);

                    await SendRawAsync("IDENTIFY|USER\n");
                    await Task.Delay(100);
                    await SendRawAsync($"AUTH_SYNC|{username}\n");
                    await Task.Delay(100);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UserManager INIT] " + ex.Message);
                return false;
            }
        }

        public static async Task<string[]> GetOnlineUsersAsync(string currentUser, int timeoutMs = 3000)
        {
            if (!IsConnected) return Array.Empty<string>();

            await _lock.WaitAsync();
            try
            {
                await SendRawAsync($"GET_USER_AND_GROUPS|{currentUser}\n");

                var readTask = _reader.ReadLineAsync();
                if (await Task.WhenAny(readTask, Task.Delay(timeoutMs)) == readTask)
                {
                    string response = readTask.Result?.Trim();
                    if (response?.StartsWith("USERS_AND_GROUPS|") == true)
                    {
                        var parts = response.Split('|');
                        if (parts.Length >= 2)
                        {
                            return parts[1]
                                .Split(',')
                                .Where(u => !string.IsNullOrWhiteSpace(u) && u != currentUser)
                                .ToArray();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[UserManager] ❌ Timeout khi chờ phản hồi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserManager ERROR] {ex.Message}");
            }
            finally
            {
                _lock.Release();
            }

            return Array.Empty<string>();
        }


        private static async Task SendRawAsync(string data)
        {
            if (_stream != null && _stream.CanWrite)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                await _stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        public static void Close()
        {
            _isClosed = true;
            try { _reader?.Close(); } catch { }
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }

            _reader = null;
            _stream = null;
            _client = null;
        }
    }
}
