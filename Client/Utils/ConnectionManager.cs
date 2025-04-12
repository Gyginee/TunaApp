
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client;

namespace Client.Utils
{
    public static class ConnectionManager
    {
        private static TcpClient _client;
        private static NetworkStream _stream;
        private static SemaphoreSlim _lock = new(1, 1);
        private static bool _isClosed = false;
        private static StreamReader _reader;


        public static bool IsConnected => _client?.Connected ?? false;

        public static async Task<bool> InitializeAsync()
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
                    _reader = new StreamReader(_stream, new UTF8Encoding(false));

                    await SendRawAsync("IDENTIFY|LOGIN\n");
                    await Task.Delay(100); // Cho server phân loại
                }

                _isClosed = false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGIN INIT ERROR] {ex.Message}");
                return false;
            }
        }



        public static async Task SendCommandAsync(string command)
        {

            if (_isClosed || _stream == null || !_stream.CanWrite || !_client.Connected)
            {
                MessageBox.Show("[SEND] Không thể gửi vì stream/socket đã đóng.");        
                return;
            }
            await _lock.WaitAsync();
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(command + "\n");
                await _stream.WriteAsync(buffer, 0, buffer.Length);
            }
            finally { _lock.Release(); }
        }

        public static async Task<string> ReadResponseAsync()
        {
            if (_reader == null) return string.Empty;
            string line = await _reader.ReadLineAsync();
            return line?.Trim() ?? "";
        }


        public static void Close()
        {
            _isClosed = true;

            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }

            try { _stream?.Dispose(); } catch { }      
            try { _client?.Dispose(); } catch { }      

            _client = null;
            _stream = null;
        }


        private static async Task SendRawAsync(string raw)
        {
            byte[] data = Encoding.UTF8.GetBytes(raw);
            await _stream.WriteAsync(data, 0, data.Length);
        }
    }
}