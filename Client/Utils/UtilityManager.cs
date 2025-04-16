using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Utils
{
    public static class UtilityManager
    {
        public static async Task<string> SendSingleCommandAsync(string username, string command, string type = "INFO", int timeoutMs = 30000)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            StreamWriter writer = null;
            StreamReader reader = null;

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(ServerConfig.ServerIP, ServerConfig.ServerPort);

                stream = client.GetStream();
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                reader = new StreamReader(stream, Encoding.UTF8);

                string[] messages = {
            $"IDENTIFY|{type}",
            $"AUTH_SYNC|{username}",
            command
        };

                foreach (var msg in messages)
                {
                    await writer.WriteLineAsync(msg); 
                    await Task.Delay(100); 
                }

                await writer.FlushAsync();

                // Thêm độ trễ nhỏ trước khi đọc phản hồi
                await Task.Delay(500);

                var readTask = reader.ReadLineAsync();

                if (await Task.WhenAny(readTask, Task.Delay(timeoutMs)) == readTask)
                {
                    var result = readTask.Result?.Trim();
                    return string.IsNullOrWhiteSpace(result) ? "ERROR|Empty response" : result;
                }
                else
                {
                    return "ERROR|Timeout waiting for response";
                }
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.GetType().Name}: {ex.Message}";
            }
            finally
            {
     
                reader?.Close();
                writer?.Close();
                stream?.Close();
                client?.Close();
            
            }
        }
    }
}