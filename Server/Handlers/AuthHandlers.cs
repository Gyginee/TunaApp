// File: Server/Handlers/AuthHandlers.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Handlers
{
    public static class AuthHandlers
    {
        public static Dictionary<string, Dictionary<string, ClientState>> ClientsByUser;
        public static object ClientLock;
        public static UserRepository _userRepo;

        public static ClientState? GetClient(string username, string type)
        {
            if (ClientsByUser.TryGetValue(username, out var dict) &&
                dict.TryGetValue(type, out var client) &&
                client.Client.Connected)
                return client;
            return null;
        }

        public static async Task HandleRegistration(StreamWriter writer, string username, string password)
        {
            try
            {
                var existing = _userRepo.GetByUsername(username);
                if (existing != null)
                {
                    await writer.WriteLineAsync("REGISTER_FAIL|Username already exists.");
                    return;
                }

                var user = new User { Username = username, Password = password };
                _userRepo.Insert(user);

                await writer.WriteLineAsync("REGISTER_SUCCESS");
                Console.WriteLine($"✅ Đăng ký thành công: '{username}'");
            }
            catch
            {
                await writer.WriteLineAsync("REGISTER_FAIL|An unexpected error occurred.");
            }
        }

        public static async Task HandleLogin(ClientState clientState, string username, string password)
        {
            bool loginSuccess = false;
            try
            {
                Console.WriteLine($"[LOGIN] Đăng nhập yêu cầu từ: {username}");

                var user = _userRepo.GetByUsername(username);
                if (user != null && user.Password == password)
                {
                    lock (ClientLock)
                    {
                        if (!ClientsByUser.ContainsKey(username))
                            ClientsByUser[username] = new();

                        if (ClientsByUser[username].TryGetValue("LOGIN", out var existingLogin) &&
                            existingLogin.Client.Connected)
                        {
                            Console.WriteLine($"[LOGIN_FAIL] {username} đã đăng nhập từ nơi khác.");
                        }
                        else
                        {
                            clientState.Username = username;
                            clientState.ConnectionType = "LOGIN";
                            ClientsByUser[username]["LOGIN"] = clientState;
                            loginSuccess = true;
                        }
                    }

                    if (loginSuccess)
                    {
                        Console.WriteLine($"📥 Đăng nhập thành công: {username}");
                        await clientState.Writer.WriteLineAsync("LOGIN_SUCCESS");
                        NotifyClientConnected(username);
                    }
                    else
                    {
                        await clientState.Writer.WriteLineAsync("LOGIN_FAIL|User already logged in.");
                    }

                    Console.WriteLine($"📥 Đăng nhập thành công: {username}");
                    await clientState.Writer.WriteLineAsync("LOGIN_SUCCESS");
                    NotifyClientConnected(username);
                }
                else
                {
                    Console.WriteLine($"[LOGIN_FAIL] Sai thông tin đăng nhập: {username}");
                    await clientState.Writer.WriteLineAsync("LOGIN_FAIL|Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGIN EXCEPTION] {ex.Message}");
                await clientState.Writer.WriteLineAsync("LOGIN_FAIL|An unexpected error occurred.");
            }
        }

        public static void HandleLogout(ClientState clientState)
        {
            if (clientState?.Username == null || clientState.ConnectionType == null) return;

            lock (ClientLock)
            {
                if (ClientsByUser.TryGetValue(clientState.Username, out var connDict))
                {
                    if (connDict.TryGetValue(clientState.ConnectionType, out var existing) && existing == clientState)
                    {
                        connDict.Remove(clientState.ConnectionType);
                        Console.WriteLine($"[LOGOUT] {clientState.Username} - {clientState.ConnectionType}");

                        if (connDict.Count == 0)
                        {
                            ClientsByUser.Remove(clientState.Username);
                            NotifyClientDisconnected(clientState.Username);
                        }
                    }
                }
            }
        }

        public static void HandleDisconnect(ClientState clientState)
        {
            if (clientState?.Username == null || clientState.ConnectionType == null) return;

            lock (ClientLock)
            {
                if (ClientsByUser.TryGetValue(clientState.Username, out var connDict))
                {
                    if (connDict.TryGetValue(clientState.ConnectionType, out var existing) && existing == clientState)
                    {
                        connDict.Remove(clientState.ConnectionType);
                        Console.WriteLine($"[DISCONNECT] {clientState.Username} - {clientState.ConnectionType}");

                        if (connDict.Count == 0)
                        {
                            ClientsByUser.Remove(clientState.Username);
                            NotifyClientDisconnected(clientState.Username);
                        }
                    }
                }
            }
        }

        public static void NotifyClientConnected(string username)
        {
            lock (ClientLock)
            {
                foreach (var userEntry in ClientsByUser)
                {
                    string otherUser = userEntry.Key;
                    if (otherUser == username) continue;

                    var messageClient = GetClient(otherUser, "MESSAGE");
                    messageClient?.Writer.WriteLineAsync($"USER_CONNECTED|{username}");
                }

                Console.WriteLine($"[NOTIFY] {username} đã online");
            }
        }

        public static void NotifyClientDisconnected(string username)
        {
            lock (ClientLock)
            {
                foreach (var userEntry in ClientsByUser)
                {
                    string otherUser = userEntry.Key;
                    if (otherUser == username) continue;

                    var messageClient = GetClient(otherUser, "MESSAGE");
                    messageClient?.Writer.WriteLineAsync($"USER_DISCONNECTED|{username}");
                }

                Console.WriteLine($"[NOTIFY] {username} đã offline");
            }
        }
    }
}