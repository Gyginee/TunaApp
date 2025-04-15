using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handlers
{
    public static class HostHandlers
    {
        private static HttpListener _listener;
        private static bool _isRunning = false;

        private static readonly string _hostingFolder =
    Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Server\HostingTemplate"));


        public static Action<string> LogCallback { get; set; } = Console.WriteLine;

        public static void StartWebHosting(int port)
        {
            if (_isRunning) return;

            if (!Directory.Exists(_hostingFolder))
            {
                Directory.CreateDirectory(_hostingFolder);
                File.WriteAllText(Path.Combine(_hostingFolder, "index.html"),
                    $"<h1>{_hostingFolder}\nWelcome to your hosted chat server</h1><p>Edit this file in /HostingTemplate/index.html</p>");
            }

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/");
            _listener.Start();
            _isRunning = true;

            Log("🌐 Server started at http://localhost:" + port);
            Task.Run(() => HandleRequests());
        }

        public static void StopWebHosting()
        {
            if (!_isRunning) return;

            _isRunning = false;
            try { _listener?.Close(); } catch { }
            _listener = null;

            Log("🛑 Server stopped.");
        }

        private static async Task HandleRequests()
        {
            while (_isRunning)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    var req = context.Request;
                    var res = context.Response;

                    string requestPath = req.Url.AbsolutePath.TrimStart('/');
                    string clientIP = req.RemoteEndPoint.ToString();
                    string method = req.HttpMethod;

                    // 🧾 Log HTTP Request chi tiết
                    StringBuilder requestLog = new();
                    requestLog.AppendLine($"📥 {method} /{requestPath} từ {clientIP}");
                    requestLog.AppendLine("------ Request Headers ------");
                    foreach (string headerKey in req.Headers.AllKeys)
                        requestLog.AppendLine($"{headerKey}: {req.Headers[headerKey]}");
                    requestLog.AppendLine("-----------------------------");

                    Log(requestLog.ToString());

                    if (requestPath == "favicon.ico")
                    {
                        res.StatusCode = 204;
                        res.Close();
                        continue;
                    }

                    string filePath = string.IsNullOrWhiteSpace(requestPath)
                        ? Path.Combine(_hostingFolder, "index.html")
                        : Path.Combine(_hostingFolder, requestPath);

                    if (File.Exists(filePath))
                    {
                        string content = await File.ReadAllTextAsync(filePath);
                        byte[] buffer = Encoding.UTF8.GetBytes(content);

                        res.ContentType = GetMimeType(filePath);
                        res.ContentLength64 = buffer.Length;
                        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        res.Close();

                        Log($"📤 Phản hồi: 200 OK ({buffer.Length} bytes) | File: {requestPath}");
                    }
                    else
                    {
                        string notFound = $"<h1>404 - Không tìm thấy /{requestPath}</h1>";
                        byte[] buffer = Encoding.UTF8.GetBytes(notFound);

                        res.StatusCode = 404;
                        res.ContentType = "text/html";
                        res.ContentLength64 = buffer.Length;
                        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        res.Close();

                        Log($"❌ 404 Not Found: /{requestPath}");
                    }
                }
                catch (Exception ex)
                {
                    Log($"[WEB ERROR] {ex.Message}");
                }
            }
        }

        private static void Log(string message)
        {
            LogCallback?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        private static string GetMimeType(string path)
        {
            return Path.GetExtension(path).ToLower() switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".gif" => "image/gif",
                ".ico" => "image/x-icon",
                _ => "text/plain"
            };
        }
    }
}
