
# 🧩 ChatApp - Ứng dụng chat TCP đa năng

Một ứng dụng chat máy khách - máy chủ viết bằng C# sử dụng giao thức TCP, hỗ trợ:
- 💬 Nhắn tin cá nhân & nhóm
- 📁 Gửi ảnh và tệp đính kèm
- 👥 Tạo và tham gia nhóm
- 📡 Xem giá vàng mới nhất từ API
- 🌐 Hosting web mini nội bộ
- ✉️ Gửi mail nội bộ đơn giản
- ⚙️ Giao diện WinForms tiện dụng, dễ dùng

---

## 🚀 Mục tiêu chính

Ứng dụng hướng tới:
- Xây dựng hệ thống chat nhiều người dùng sử dụng TCP Socket với giao thức tùy chỉnh
- Tối ưu cho mô hình **đa luồng** (mỗi user có 3 luồng kết nối: LOGIN, INFO, MESSAGE)
- Tích hợp giao diện đồ họa và backend xử lý logic rõ ràng
- Mô phỏng nhiều tính năng thực tế như gửi file, chat nhóm, theo dõi người online...

---

## 🛠 Kiến trúc tổng quan

```
📦 ChatApp/
├── Server/
│   ├── Program.cs                 # Khởi động server, lắng nghe TCP
│   ├── Handlers/                 # Xử lý logic từng lệnh
│   │   ├── AuthHandlers.cs       # Đăng nhập/Đăng ký/Logout
│   │   ├──...
│   ├── Models/                   # Model và repository SQLite
│   └── Database/init_schema.sql # Cấu trúc DB SQLite
│
├── Client/
│   ├── Menu.cs                   # Form chính sau khi đăng nhập
│   ├── Chat.cs                   # Giao diện và xử lý tin nhắn
│   ├── Gold.cs                   # Hiển thị bảng giá vàng
│   ├── GroupCreationDialog.cs    # Form tạo/Join nhóm
│   ├── PingUserDialog.cs         # Form Ping người dùng
│   ├── Utils/                    # Quản lý kết nối và lệnh
│   │   ├── ConnectionManager.cs  # Kết nối LOGIN
│   │   ├──...
│
└── README.md                     # Hướng dẫn này
```

---

## ✅ Các tính năng nổi bật

### 💬 Chat cá nhân và nhóm
- Tự động đồng bộ người dùng online
- Nhắn tin, gửi ảnh và file
- Chat nhóm, tạo nhóm động

### 🪙 Giá vàng
- Lấy giá vàng theo từng loại (SJC, nữ trang...) từ API thật
- Giao diện bảng rõ ràng

### 📂 Gửi ảnh/tệp
- Gửi hình ảnh với preview
- Gửi tệp tải xuống với tên gốc

### 🛜 Ping người dùng
- Ping để kiểm tra ai đang online

### 🌐 Web hosting mini
- Tạo folder HostingTemplate và truy cập qua `http://localhost:PORT`
- Có thể dùng để hiển thị tài liệu nội bộ

---

## 🧪 Cách chạy project

### Server:
```bash
cd Server
dotnet run
```

> Lưu ý: File `init_schema.sql` sẽ được thực thi để tạo database SQLite (nếu chưa có)

### Client:
```bash
cd Client
Open in Visual Studio (WinForms App)
```

---

## 📡 Giao thức TCP tùy chỉnh

Mỗi client khi kết nối sẽ gửi:
```plaintext
IDENTIFY|[LOGIN|MESSAGE|INFO]
AUTH_SYNC|username
```

Các lệnh chính:
- `LOGIN|user|pass`, `LOGOUT`
- `MESSAGE|recipient|content`, `GROUP_MESSAGE|group|content`
- `FILE|user|name|base64`, `IMAGE|user|base64`
- `CREATE_GROUP|group|user1,user2`, `JOIN_GROUP|group|username`
- `GOLD`, `GET_USER_AND_GROUPS|user`, `GET_MESSAGES`
- `PING_USER|username`, `SEND_MAIL|to|title|cc|bcc|body`

---

## 📥 Phụ thuộc

- `.NET 6.0+`
- SQLite (`System.Data.SQLite`)
- Newtonsoft.Json
- WinForms

---

## 📌 Ghi chú phát triển

- Mỗi user có 3 kết nối song song để tách luồng: Đăng nhập, Chat, Thông tin
- Giao thức đơn giản hóa để dễ test và triển khai trong LAN
- Thiết kế hướng học tập, dễ mở rộng thêm voice, mail, XMPP, WebSocket...

---

## 📃 License

MIT License - Dự án hướng học tập và nghiên cứu
