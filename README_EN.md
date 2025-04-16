
# 🧩 ChatApp - Versatile TCP Chat Application

A client-server chat application written in C# using TCP sockets, supporting:
- 💬 Private & group chat
- 📁 Send images and attachments
- 👥 Create and join groups
- 📡 View latest gold prices from an API
- 🌐 Built-in local mini web hosting
- ✉️ Simple internal email sending
- ⚙️ User-friendly WinForms interface

---

## 🚀 Key Objectives

This app aims to:
- Build a multi-user chat system over TCP Socket with a custom protocol
- Optimize for **multi-threaded model** (each user has 3 TCP streams: LOGIN, INFO, MESSAGE)
- Integrate GUI and backend logic clearly
- Simulate real-life features like file transfer, group chat, online tracking...

---

## 🛠 Project Structure

```
📦 ChatApp/
├── Server/
│   ├── Program.cs                 # Launch the server, listen on TCP
│   ├── Handlers/                 # Process logic for each command
│   │   ├── AuthHandlers.cs       # Login/Register/Logout
│   │   ├──...
│   └── Database/init_schema.sql # SQLite DB schema
│
├── Client/
│   ├── Menu.cs                   # Main form after login
│   ├── Chat.cs                   # Chat UI and logic
│   ├── Gold.cs                   # Gold price table
│   ├── GroupCreationDialog.cs    # Create/Join group form
│   ├── PingUserDialog.cs         # User ping dialog
│   ├── Utils/                    # Manage connections and commands
│   │   ├── ConnectionManager.cs  # LOGIN stream
│   │   ├──...
│
└── README.md                     # This guide
```

---

## ✅ Core Features

### 💬 Personal and Group Chat
- Automatically sync online users
- Send text, images, files
- Dynamic group creation and chat

### 🪙 Gold Prices
- Get real-time gold rates (SJC, jewelry...) from real API
- Displayed in a clear grid/table

### 📂 File/Image Transfer
- Send previewable images
- Download files with original names

### 🛜 User Ping
- Ping to check who's online

### 🌐 Mini Web Hosting
- Create HostingTemplate folder and access via `http://localhost:PORT`
- Useful for internal docs or info sharing

---

## 🧪 How to Run

### Server:
```bash
cd Server
dotnet run
```

> Note: `init_schema.sql` will be executed automatically to set up SQLite DB

### Client:
```bash
cd Client
Open in Visual Studio (WinForms App)
```

---

## 📡 TCP Protocol

Each client sends this on connection:
```plaintext
IDENTIFY|[LOGIN|MESSAGE|INFO]
AUTH_SYNC|username
```

Main commands:
- `LOGIN|user|pass`, `LOGOUT`
- `MESSAGE|recipient|content`, `GROUP_MESSAGE|group|content`
- `FILE|user|name|base64`, `IMAGE|user|base64`
- `CREATE_GROUP|group|user1,user2`, `JOIN_GROUP|group|username`
- `GOLD`, `GET_USER_AND_GROUPS|user`, `GET_MESSAGES`
- `PING_USER|username`, `SEND_MAIL|to|title|cc|bcc|body`

---

## 📥 Dependencies

- `.NET 6.0+`
- SQLite (`System.Data.SQLite`)
- Newtonsoft.Json
- WinForms

---

## 📌 Dev Notes

- Each user connects with 3 parallel streams: LOGIN, CHAT, INFO
- Protocol is simplified for LAN testing
- Designed for learning purpose, can extend to voice, mail, WebSocket, XMPP...

---

## 📃 License

MIT License - Educational use only
