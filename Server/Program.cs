using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.Handlers;
using Server.Models;
using Server;

namespace Server
{
    public class ClientState
    {
        public TcpClient Client { get; set; }
        public string Username { get; set; }
        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }
        public string ConnectionType { get; set; }
    }

    class Program
    {
        private static readonly Dictionary<string, Dictionary<string, ClientState>> ClientsByUser = new();
        private static readonly Dictionary<string, List<string>> Groups = new();
        private static readonly object ClientLock = new();

        private static SQLiteConnection DbConnection;
        private static UserRepository _userRepo;
        private static MessageRepository _messageRepo;
        private static GroupRepository _groupRepo;
        private static FileMessageRepository _fileRepo;

        private static string filesDirectory = "Files";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Clear();
            Console.WriteLine("Starting Server...");
            Directory.CreateDirectory(filesDirectory);

            InitializeDatabase();
            InitializeRepositories();
           
            // Inject dependencies
            MessageHandlers.ClientsByUser = ClientsByUser;
            MessageHandlers.Groups = Groups;
            MessageHandlers._userRepo = _userRepo;
            MessageHandlers._messageRepo = _messageRepo;
            MessageHandlers._groupRepo = _groupRepo;
            MessageHandlers._fileRepo = _fileRepo;
            MessageHandlers.filesDirectory = filesDirectory;

            AuthHandlers.ClientsByUser = ClientsByUser;
            AuthHandlers.ClientLock = ClientLock;
            AuthHandlers._userRepo = _userRepo;

            UserHandlers._userRepo = _userRepo;

            await GoldHandlers.InitializeAsync();
            await StartServer(ServerConfig.ServerPort);

            Console.WriteLine("Server stopped.");
        }

        static void InitializeDatabase()
        {
            DbConnection = new SQLiteConnection("Data Source=chat.db;Version=3;");
            DbConnection.Open();

            string schemaPath = "Database/init_schema.sql";
            if (!File.Exists(schemaPath))
            {
                Console.WriteLine("❌ Không tìm thấy file init_schema.sql");
                Environment.Exit(1); // hoặc return;
            }

            Console.WriteLine("📄 Đang khởi tạo cấu trúc cơ sở dữ liệu...");
            using (var cmd = new SQLiteCommand(File.ReadAllText(schemaPath), DbConnection))
            {
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("🧹 Xoá dữ liệu tin nhắn cũ...");
            using (var cleanupCmd = new SQLiteCommand(
                @"DELETE FROM PrivateMessages;
          DELETE FROM GroupMessages;
          DELETE FROM FileMessages;", DbConnection))
            {
                cleanupCmd.ExecuteNonQuery();
            }

            Console.WriteLine("✅ Database đã sẵn sàng.");
        }

        static void InitializeRepositories()
        {
            _userRepo = new UserRepository(DbConnection);
            _messageRepo = new MessageRepository(DbConnection);
            _groupRepo = new GroupRepository(DbConnection);
            _fileRepo = new FileMessageRepository(DbConnection);
        }

        static async Task StartServer(int port)
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"Server listening on port {port}...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        static async Task HandleClient(TcpClient tcpClient)
        {
            ClientState clientState = new ClientState { Client = tcpClient };

            try
            {
                using NetworkStream stream = tcpClient.GetStream();
                stream.ReadTimeout = 5000;
                stream.WriteTimeout = 5000;

                using StreamReader reader = new StreamReader(stream, new UTF8Encoding(false));
                using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };


                clientState.Writer = writer;
                clientState.Reader = reader;
                var lastActivity = DateTime.UtcNow;

                while (true)
                {
                    if (stream.DataAvailable)
                    {
                        string rawMessage = await reader.ReadLineAsync();
                        lastActivity = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(rawMessage))
                            await ProcessMessage(clientState, rawMessage);
                    }
                    if (stream.DataAvailable)
                    {
                        string rawMessage = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(rawMessage))
                            await ProcessMessage(clientState, rawMessage);
                    }
                    else
                    {
                        await Task.Delay(100); // nhẹ CPU
                    }
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                AuthHandlers.HandleDisconnect(clientState);
                tcpClient.Close();
            }
        }

        static async Task ProcessMessage(ClientState state, string message)
        {
            try
            {
                string[] parts = message.Split('|');
                string command = parts[0].ToUpper();

                //DEBUG
                Console.WriteLine($"[RECEIVED] {message}");

                switch (command)
                {
                    case "IDENTIFY":
                        state.ConnectionType = parts.ElementAtOrDefault(1);
                        Console.WriteLine($"[SERVER] Luồng kết nối dạng: {state.ConnectionType}");
                        break;

                    case "AUTH_SYNC":
                        state.Username = parts.ElementAtOrDefault(1);
                        if (!ClientsByUser.ContainsKey(state.Username))
                            ClientsByUser[state.Username] = new();
                        ClientsByUser[state.Username][state.ConnectionType ?? "UNKNOWN"] = state;
                        Console.WriteLine($"[SERVER] AUTH_SYNC: {state.Username} on {state.ConnectionType}");
                        break;

                    case "REGISTER": await AuthHandlers.HandleRegistration(state.Writer, parts[1], parts[2]); break;
                    case "LOGIN": await AuthHandlers.HandleLogin(state, parts[1], parts[2]); break;
                    case "LOGOUT": AuthHandlers.HandleLogout(state); break;
                    case "GET_USER":
                        // Lệnh có định dạng: GET_USER|username
                        if (parts.Length >= 2)
                        {
                            await UserHandlers.HandleGetUser(state.Writer, parts[1]);
                        }
                        else
                        {
                            await state.Writer.WriteLineAsync("ERROR|Missing user parameter for GET_USER");
                        }
                        break;
                    case "GET_USER_AND_GROUPS": await MessageHandlers.HandleUserAndGroupList(state.Writer, parts[1]); break;
                    case "GET_MESSAGES": await MessageHandlers.HandleGetMessages(state.Writer, state.Username); break;
                    case "MESSAGE": await MessageHandlers.HandleDirectMessage(state.Username, parts[1], parts[2]); break;
                    case "GROUP_MESSAGE": await MessageHandlers.HandleGroupMessage(state.Username, parts[1], parts[2]); break;
                    case "CREATE_GROUP": MessageHandlers.HandleGroupCreation(parts[1], parts[2].Split(',')); break;
                    case "JOIN_GROUP": await MessageHandlers.HandleJoinGroup(parts[1], parts[2]); break;
                    case "IMAGE": await MessageHandlers.HandleImageTransfer(state.Username, parts[1], parts[2]); break;
                    case "GROUP_IMAGE": await MessageHandlers.HandleGroupImageTransfer(state.Username, parts[1], parts[2]); break;
                    case "FILE": await MessageHandlers.HandleFileTransfer(state.Username, parts[1], parts[2], parts[3]); break;
                    case "GROUP_FILE": await MessageHandlers.HandleGroupFileTransfer(state.Username, parts[1], parts[2], parts[3]); break;
                    case "GOLD":
                        {
                            Console.WriteLine($"[GOLD] Nhận yêu cầu GOLD từ {state.Username} ({state.ConnectionType})");
                            string json = await GoldHandlers.GetGoldPriceJsonAsync();
                            Console.WriteLine($"[GOLD] Trả JSON dài {json.Length} ký tự");
                            await state.Writer.WriteLineAsync("GOLD_JSON|" + json);
                            break;
                        }
                    case "SEND_MAIL":
                        // Định dạng lệnh: SEND_MAIL|recipient|title|cc|bcc|body
                        if (parts.Length >= 6)
                        {
                            await MailHandlers.HandleSendMail(state.Username, parts[1], parts[2], parts[3], parts[4], parts[5]);
                            await state.Writer.WriteLineAsync("SEND_MAIL_SUCCESS");
                        }
                        else
                        {
                            await state.Writer.WriteLineAsync("SEND_MAIL_FAIL|Thiếu thông tin gửi mail");
                        }
                        break;

                    case "PING_USER":
                        // Lệnh có định dạng: PING_USER|targetUser
                        if (parts.Length >= 2)
                        {
                            string target = parts[1];
                            var targetClient = AuthHandlers.GetClient(target, "LOGIN");
                            if (targetClient != null)
                            {
                                await state.Writer.WriteLineAsync($"PONG_USER|{target}");
                            }
                            else
                            {
                                await state.Writer.WriteLineAsync("PING_FAIL|User không online");
                            }
                        }
                        else
                        {
                            await state.Writer.WriteLineAsync("PING_FAIL|Thiếu target");
                        }
                        break;

                    default: await state.Writer.WriteLineAsync("UNKNOWN_COMMAND"); break;
                }
            }
            catch
            {
                await state.Writer.WriteLineAsync("ERROR|Failed to process command");
            }
        }

       

    }
}