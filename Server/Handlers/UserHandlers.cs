using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Handlers
{
    public static class UserHandlers
    {
        // Khai báo UserRepository để có thể truy vấn thông tin user.
        // Repository này đã được khởi tạo ở Program.cs.
        public static UserRepository _userRepo;

        // Phương thức xử lý lệnh GET_USER.
        // Lệnh có định dạng: GET_USER|username cần truy vấn.
        // Nếu tìm thấy user thì trả về chuỗi "USER|<username>"
        // Nếu không tìm thấy, trả về lỗi.
        public static async Task HandleGetUser(StreamWriter writer, string username)
        {
            try
            {
                var user = _userRepo.GetByUsername(username);
                if (user != null)
                {
                    // Ví dụ trả về thông tin user dạng: USER|username
                    await writer.WriteLineAsync("USER|" + user.Username);
                }
                else
                {
                    await writer.WriteLineAsync("ERROR|User not found");
                }
            }
            catch (Exception ex)
            {
                await writer.WriteLineAsync("ERROR|" + ex.Message);
            }
        }

        public static async Task<string> PingHostAsync(string ipAddress)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    var reply = await ping.SendPingAsync(ipAddress, 10000); // timeout 10s

                    if (reply.Status == IPStatus.Success)
                    {
                        return $"✔ Ping OK: {reply.RoundtripTime} ms";
                    }
                    else
                    {
                        return $"❌ Ping thất bại: {reply.Status}";
                    }
                }
                catch (PingException ex)
                {
                    return $"❌ Lỗi Ping: {ex.Message}";
                }
                catch (Exception ex)
                {
                    return $"❌ Lỗi hệ thống: {ex.Message}";
                }
            }
        }
    }
}
