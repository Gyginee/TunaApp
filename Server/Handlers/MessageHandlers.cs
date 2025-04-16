// This file should go under Server/Handlers/MessageHandlers.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Handlers
{
    public static class MessageHandlers
    {
        public static Dictionary<string, Dictionary<string, ClientState>> ClientsByUser;
        public static Dictionary<string, List<string>> Groups;
        public static UserRepository _userRepo;
        public static MessageRepository _messageRepo;
        public static GroupRepository _groupRepo;
        public static FileMessageRepository _fileRepo;
        public static string filesDirectory;

        public static ClientState? GetClient(string username, string type)
        {
            if (ClientsByUser.TryGetValue(username, out var dict) &&
                dict.TryGetValue(type, out var client) &&
                client.Client.Connected)
                return client;
            return null;
        }

        public static async Task HandleUserAndGroupList(StreamWriter writer, string username)
        {
            try
            {
                var onlineUsers = string.Join(",", ClientsByUser.Keys.Where(u => u != username));

                var user = _userRepo.GetByUsername(username);
                if (user == null)
                {
                    await writer.WriteLineAsync("ERROR|Invalid user");
                    return;
                }

                var groupIds = _groupRepo.GetGroupIdsByUserId(user.Id);
                var groupNames = groupIds
                    .Select(id => _groupRepo.GetById(id)?.GroupName)
                    .Where(name => !string.IsNullOrWhiteSpace(name));

                string response = $"USERS_AND_GROUPS|{onlineUsers}|{string.Join(",", groupNames)}";
                await writer.WriteLineAsync(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HandleUserAndGroupList ERROR] {ex.Message}");
                await writer.WriteLineAsync("ERROR|Failed to retrieve users and groups");
            }
        }

        public static async Task HandleDirectMessage(string sender, string recipient, string content)
        {
            var receiverClient = GetClient(recipient, "MESSAGE");
            string processedMessage = ProcessEmojis(content);

            if (receiverClient != null)
            {
                await receiverClient.Writer.WriteLineAsync($"MESSAGE|{sender}|{processedMessage}");
            }

            var senderId = _userRepo.GetByUsername(sender)?.Id ?? 0;
            var receiverId = _userRepo.GetByUsername(recipient)?.Id ?? 0;

            _messageRepo.InsertPrivateMessage(new PrivateMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content
            });
        }

        public static async Task HandleGetMessages(StreamWriter writer, string username)
        {
            try
            {
                var user = _userRepo.GetByUsername(username);
                if (user == null)
                {
                    await writer.WriteLineAsync("ERROR|User not found");
                    return;
                }

                var privateMsgs = _messageRepo.GetPrivateMessagesByUserId(user.Id);
                var groupIds = _groupRepo.GetGroupIdsByUserId(user.Id);
                var groupMsgs = _messageRepo.GetGroupMessagesByGroupIds(groupIds);

                var sb = new StringBuilder();

                foreach (var msg in privateMsgs)
                {
                    var senderName = _userRepo.GetById(msg.SenderId)?.Username;
                    sb.Append($"MESSAGE|{senderName}|{msg.Content}|||");
                }

                foreach (var gmsg in groupMsgs)
                {
                    var senderName = _userRepo.GetById(gmsg.SenderId)?.Username;
                    var groupName = _groupRepo.GetById(gmsg.GroupId)?.GroupName;
                    sb.Append($"GROUP_MESSAGE|{senderName}|{groupName}|{gmsg.Content}|||");
                }

                await writer.WriteLineAsync(sb.ToString().TrimEnd('|'));
            }
            catch
            {
                await writer.WriteLineAsync("ERROR|Failed to retrieve messages");
            }
        }


        public static void HandleGroupCreation(string groupName, string[] usernames)
        {
            if (_groupRepo.GetByName(groupName) != null) return;

            var group = new Group { GroupName = groupName };
            _groupRepo.Insert(group);
            var groupId = _groupRepo.GetByName(groupName).Id;

            foreach (var username in usernames)
            {
                var userId = _userRepo.GetByUsername(username)?.Id ?? 0;
                _groupRepo.AddMember(groupId, userId);
            }

            Groups[groupName] = new List<string>(usernames);

            foreach (var user in usernames)
            {
                var client = GetClient(user, "MESSAGE");
                if (client != null)
                {
                    client.Writer.WriteLineAsync($"GROUP_CREATED|{groupName}|{string.Join(",", usernames)}");
                }
            }
        }

        public static async Task HandleJoinGroup(string groupName, string username)
        {
            var group = _groupRepo.GetByName(groupName);
            var user = _userRepo.GetByUsername(username);
            if (group == null || user == null)
            {
                var client = GetClient(username, "MESSAGE");
                if (client != null)
                {
                    await client.Writer.WriteLineAsync("JOIN_FAIL|Group or user not found.");
                }
                return;
            }

            if (_groupRepo.IsMember(group.Id, user.Id))
            {
                var client = GetClient(username, "MESSAGE");
                if (client != null)
                {
                    await client.Writer.WriteLineAsync("JOIN_FAIL|Already in group.");
                }
                return;
            }

            _groupRepo.AddMember(group.Id, user.Id);
            if (!Groups.ContainsKey(groupName))
                Groups[groupName] = new List<string>();

            Groups[groupName].Add(username);

            foreach (var member in Groups[groupName])
            {
                var client = GetClient(member, "MESSAGE");
                if (client != null)
                {
                    await client.Writer.WriteLineAsync($"GROUP_JOINED|{username}|{groupName}");
                }
            }
        }

        public static async Task HandleGroupMessage(string sender, string groupName, string content)
        {
            var senderId = _userRepo.GetByUsername(sender)?.Id ?? 0;
            var group = _groupRepo.GetByName(groupName);
            if (group == null) return;

            foreach (var member in _groupRepo.GetMembersByGroupId(group.Id))
            {
                var receiver = GetClient(member.Username, "MESSAGE");
                if (receiver != null)
                {
                    await receiver.Writer.WriteLineAsync($"GROUP_MESSAGE|{sender}|{groupName}|{content}");
                }
            }

            _messageRepo.InsertGroupMessage(new GroupMessage
            {
                GroupId = group.Id,
                SenderId = senderId,
                Content = content
            });
        }

        public static async Task HandleImageTransfer(string sender, string recipient, string base64Image)
        {
            var receiver = GetClient(recipient, "MESSAGE");
            if (receiver != null)
            {
                await receiver.Writer.WriteLineAsync($"IMAGE|{sender}|{base64Image}");
            }
        }

        public static async Task HandleGroupImageTransfer(string sender, string groupName, string base64Image)
        {
            var group = _groupRepo.GetByName(groupName);
            var senderId = _userRepo.GetByUsername(sender)?.Id ?? 0;
            if (group == null) return;

            foreach (var member in _groupRepo.GetMembersByGroupId(group.Id))
            {
                var receiver = GetClient(member.Username, "MESSAGE");
                if (receiver != null)
                {
                    await receiver.Writer.WriteLineAsync($"GROUP_IMAGE|{sender}|{groupName}|{base64Image}");
                }
            }

            _messageRepo.InsertGroupMessage(new GroupMessage
            {
                GroupId = group.Id,
                SenderId = senderId,
                Content = base64Image
            });
        }

        public static async Task HandleFileTransfer(string sender, string receiverName, string fileName, string base64Data)
        {
            byte[] fileData = Convert.FromBase64String(base64Data);
            string filePath = Path.Combine(filesDirectory, $"{Guid.NewGuid()}_{fileName}");
            await File.WriteAllBytesAsync(filePath, fileData);

            var receiver = GetClient(receiverName, "MESSAGE");
            if (receiver != null)
            {
                await receiver.Writer.WriteLineAsync($"FILE|{sender}|{fileName}|{base64Data}");
            }

            var senderId = _userRepo.GetByUsername(sender)?.Id ?? 0;
            var receiverId = _userRepo.GetByUsername(receiverName)?.Id ?? 0;
            _fileRepo.Insert(new FileMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                ReceiverType = "private",
                FileName = fileName,
                FilePath = filePath
            });
        }

        public static async Task HandleGroupFileTransfer(string sender, string groupName, string fileName, string base64Data)
        {
            var group = _groupRepo.GetByName(groupName);
            if (group == null) return;

            byte[] fileData = Convert.FromBase64String(base64Data);
            string filePath = Path.Combine(filesDirectory, $"{Guid.NewGuid()}_{fileName}");
            await File.WriteAllBytesAsync(filePath, fileData);

            var senderId = _userRepo.GetByUsername(sender)?.Id ?? 0;
            _fileRepo.Insert(new FileMessage
            {
                SenderId = senderId,
                ReceiverId = group.Id,
                ReceiverType = "group",
                FileName = fileName,
                FilePath = filePath
            });

            foreach (var member in _groupRepo.GetMembersByGroupId(group.Id))
            {
                var receiver = GetClient(member.Username, "MESSAGE");
                if (receiver != null)
                {
                    await receiver.Writer.WriteLineAsync($"GROUP_FILE|{sender}|{groupName}|{fileName}|{base64Data}");
                }
            }
        }

        public static string ProcessEmojis(string message)
        {
            return message
                .Replace(":)", "😊")
                .Replace(":D", "😃")
                .Replace(":P", "😛")
                .Replace(":*", "😘")
                .Replace(":v", "🖖")
                .Replace(":3", "😺")
                .Replace(":o", "😮")
                .Replace(":|", "😐")
                .Replace(":/", "😕");
        }
    }
}