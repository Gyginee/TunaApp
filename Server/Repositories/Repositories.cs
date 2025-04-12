// Repository base interface (optional, for abstraction)
using Server.Models;
using System.Data.SQLite;

public interface IRepository<T>
{
    T GetById(int id);
    void Insert(T item);
    void Delete(int id);
    IEnumerable<T> GetAll();
}
public class UserRepository : IRepository<User>
{
    private readonly SQLiteConnection _connection;
    public UserRepository(SQLiteConnection connection) => _connection = connection;

    public User GetById(int id)
    {
        using var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Id = @id", _connection);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? new User
        {
            Id = Convert.ToInt32(reader["Id"]),
            Username = reader["Username"].ToString(),
            Password = reader["Password"].ToString()
        } : null;
    }

    public User GetByUsername(string username)
    {
        using var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Username = @username", _connection);
        cmd.Parameters.AddWithValue("@username", username);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? new User
        {
            Id = Convert.ToInt32(reader["Id"]),
            Username = reader["Username"].ToString(),
            Password = reader["Password"].ToString()
        } : null;
    }

    public void Insert(User user)
    {
        using var cmd = new SQLiteCommand("INSERT INTO Users (Username, Password) VALUES (@username, @password)", _connection);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var cmd = new SQLiteCommand("DELETE FROM Users WHERE Id = @id", _connection);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<User> GetAll()
    {
        var list = new List<User>();
        using var cmd = new SQLiteCommand("SELECT * FROM Users", _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new User
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString()
            });
        }
        return list;
    }
}
public class GroupRepository : IRepository<Group>
{
    private readonly SQLiteConnection _connection;
    public GroupRepository(SQLiteConnection connection) => _connection = connection;

    public Group GetById(int id)
    {
        using var cmd = new SQLiteCommand("SELECT * FROM Groups WHERE Id = @id", _connection);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? new Group
        {
            Id = Convert.ToInt32(reader["Id"]),
            GroupName = reader["GroupName"].ToString(),
            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
        } : null;
    }

    public Group GetByName(string groupName)
    {
        using var cmd = new SQLiteCommand("SELECT * FROM Groups WHERE GroupName = @groupName", _connection);
        cmd.Parameters.AddWithValue("@groupName", groupName);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? new Group
        {
            Id = Convert.ToInt32(reader["Id"]),
            GroupName = reader["GroupName"].ToString(),
            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
        } : null;
    }

    public IEnumerable<Group> GetAll()
    {
        var list = new List<Group>();
        using var cmd = new SQLiteCommand("SELECT * FROM Groups", _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Group
            {
                Id = Convert.ToInt32(reader["Id"]),
                GroupName = reader["GroupName"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            });
        }
        return list;
    }

    public void Insert(Group group)
    {
        using var cmd = new SQLiteCommand("INSERT INTO Groups (GroupName) VALUES (@groupName)", _connection);
        cmd.Parameters.AddWithValue("@groupName", group.GroupName);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var cmd = new SQLiteCommand("DELETE FROM Groups WHERE Id = @id", _connection);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void AddMember(int groupId, int userId)
    {
        using var cmd = new SQLiteCommand("INSERT INTO GroupMembers (GroupId, UserId) VALUES (@groupId, @userId)", _connection);
        cmd.Parameters.AddWithValue("@groupId", groupId);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.ExecuteNonQuery();
    }

    public bool IsMember(int groupId, int userId)
    {
        using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM GroupMembers WHERE GroupId = @groupId AND UserId = @userId", _connection);
        cmd.Parameters.AddWithValue("@groupId", groupId);
        cmd.Parameters.AddWithValue("@userId", userId);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<User> GetMembersByGroupId(int groupId)
    {
        var list = new List<User>();
        using var cmd = new SQLiteCommand(@"
            SELECT u.* FROM Users u
            JOIN GroupMembers gm ON u.Id = gm.UserId
            WHERE gm.GroupId = @groupId", _connection);

        cmd.Parameters.AddWithValue("@groupId", groupId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new User
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString()
            });
        }
        return list;
    }

    public List<int> GetGroupIdsByUserId(int userId)
    {
        var list = new List<int>();
        using var cmd = new SQLiteCommand("SELECT GroupId FROM GroupMembers WHERE UserId = @userId", _connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(Convert.ToInt32(reader["GroupId"]));
        }
        return list;
    }
}
public class MessageRepository
{
    private readonly SQLiteConnection _connection;
    public MessageRepository(SQLiteConnection connection) => _connection = connection;

    public void InsertPrivateMessage(PrivateMessage message)
    {
        using var cmd = new SQLiteCommand(@"
        INSERT INTO PrivateMessages (SenderId, ReceiverId, Content)
        VALUES (@senderId, @receiverId, @content)", _connection);

        cmd.Parameters.AddWithValue("@senderId", message.SenderId);
        cmd.Parameters.AddWithValue("@receiverId", message.ReceiverId);
        cmd.Parameters.AddWithValue("@content", message.Content);

        cmd.ExecuteNonQuery();
    }

    public void InsertGroupMessage(GroupMessage message)
    {
        using var cmd = new SQLiteCommand(@"
        INSERT INTO GroupMessages (GroupId, SenderId, Content)
        VALUES (@groupId, @senderId, @content)", _connection);

        cmd.Parameters.AddWithValue("@groupId", message.GroupId);
        cmd.Parameters.AddWithValue("@senderId", message.SenderId);
        cmd.Parameters.AddWithValue("@content", message.Content);

        cmd.ExecuteNonQuery();
    }


    public List<PrivateMessage> GetPrivateMessagesByUserId(int userId)
    {
        var list = new List<PrivateMessage>();
        using var cmd = new SQLiteCommand("SELECT * FROM PrivateMessages WHERE SenderId = @id OR ReceiverId = @id", _connection);
        cmd.Parameters.AddWithValue("@id", userId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new PrivateMessage
            {
                Id = Convert.ToInt32(reader["Id"]),
                SenderId = Convert.ToInt32(reader["SenderId"]),
                ReceiverId = Convert.ToInt32(reader["ReceiverId"]),
                Content = reader["Content"].ToString(),
                Timestamp = Convert.ToDateTime(reader["Timestamp"])
            });
        }
        return list;
    }

    public List<GroupMessage> GetGroupMessagesByGroupIds(List<int> groupIds)
    {
        var list = new List<GroupMessage>();
        var idList = string.Join(",", groupIds);

        if (groupIds.Count == 0) return list;

        using var cmd = new SQLiteCommand($"SELECT * FROM GroupMessages WHERE GroupId IN ({idList})", _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new GroupMessage
            {
                Id = Convert.ToInt32(reader["Id"]),
                GroupId = Convert.ToInt32(reader["GroupId"]),
                SenderId = Convert.ToInt32(reader["SenderId"]),
                Content = reader["Content"].ToString(),
                Timestamp = Convert.ToDateTime(reader["Timestamp"])
            });
        }
        return list;
    }
}

public class FileMessageRepository : IRepository<FileMessage>
{
    private readonly SQLiteConnection _connection;

    public FileMessageRepository(SQLiteConnection connection)
    {
        _connection = connection;
    }

    public FileMessage GetById(int id)
    {
        using var cmd = new SQLiteCommand("SELECT * FROM FileMessages WHERE Id = @id", _connection);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new FileMessage
            {
                Id = Convert.ToInt32(reader["Id"]),
                SenderId = Convert.ToInt32(reader["SenderId"]),
                ReceiverId = Convert.ToInt32(reader["ReceiverId"]),
                ReceiverType = reader["ReceiverType"].ToString(),
                FileName = reader["FileName"].ToString(),
                FilePath = reader["FilePath"].ToString(),
                Timestamp = Convert.ToDateTime(reader["Timestamp"])
            };
        }
        return null;
    }

    public IEnumerable<FileMessage> GetAll()
    {
        var list = new List<FileMessage>();
        using var cmd = new SQLiteCommand("SELECT * FROM FileMessages", _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new FileMessage
            {
                Id = Convert.ToInt32(reader["Id"]),
                SenderId = Convert.ToInt32(reader["SenderId"]),
                ReceiverId = Convert.ToInt32(reader["ReceiverId"]),
                ReceiverType = reader["ReceiverType"].ToString(),
                FileName = reader["FileName"].ToString(),
                FilePath = reader["FilePath"].ToString(),
                Timestamp = Convert.ToDateTime(reader["Timestamp"])
            });
        }
        return list;
    }

    public void Insert(FileMessage message)
    {
        using var cmd = new SQLiteCommand(@"
            INSERT INTO FileMessages (SenderId, ReceiverId, ReceiverType, FileName, FilePath)
            VALUES (@senderId, @receiverId, @receiverType, @fileName, @filePath)", _connection);

        cmd.Parameters.AddWithValue("@senderId", message.SenderId);
        cmd.Parameters.AddWithValue("@receiverId", message.ReceiverId);
        cmd.Parameters.AddWithValue("@receiverType", message.ReceiverType);
        cmd.Parameters.AddWithValue("@fileName", message.FileName);
        cmd.Parameters.AddWithValue("@filePath", message.FilePath);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var cmd = new SQLiteCommand("DELETE FROM FileMessages WHERE Id = @id", _connection);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}