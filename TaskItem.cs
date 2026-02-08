namespace TaskBerry;

using Microsoft.Data.Sqlite;

public class TaskItem
{
    private static readonly string _connectionString = "Data Source=taskberry.db;";

    public int Id { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public bool IsImportant { get; set; } = false;



    private static SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();

        return connection;
    }

    private static TaskItem FromReader(SqliteDataReader reader)
    {
        return new TaskItem
        {
            Id = Convert.ToInt32(reader["id"]),
            Title = reader["title"].ToString()!,
            Category = reader["category"].ToString()!,
            IsCompleted = Convert.ToBoolean(reader["is_completed"]),
            IsImportant = Convert.ToBoolean(reader["is_important"])
        };
    }


    public static void CreateTableIfNotExists()
    {
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();

        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS task (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    title TEXT NOT NULL,
                    category TEXT DEFAULT '',
                    is_completed INTEGER NOT NULL DEFAULT 0,        
                    is_important INTEGER NOT NULL DEFAULT 0        
                    );
        """;

        cmd.ExecuteNonQuery();
    }

    public void Save()
    {
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();

        if (Id == 0)
        {
            cmd.CommandText = """
                INSERT INTO task (title, category, is_completed, is_important)
                VALUES           (@title, @category, @is_completed, @is_important);
            """;
        }
        else
        {
            cmd.CommandText = """
                UPDATE task
                SET title = @title,
                    category = @category,
                    is_completed = @is_completed,
                    is_important = @is_important
                WHERE id = @id;
            """;
        }

        cmd.Parameters.AddWithValue("@id", Id);
        cmd.Parameters.AddWithValue("@title", Title);
        cmd.Parameters.AddWithValue("@category", Category);
        cmd.Parameters.AddWithValue("@is_completed", IsCompleted ? 1 : 0);
        cmd.Parameters.AddWithValue("@is_important", IsImportant ? 1 : 0);

        cmd.ExecuteNonQuery();

    }

    public void Delete()
    {
        if (this.Id == 0)
        {
            Console.WriteLine("Cannot delete unsaved task.");
            return;
        }

        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();

        cmd.CommandText = """
                DELETE FROM task
                WHERE id = @id;
            """;


        cmd.Parameters.AddWithValue("@id", this.Id);

        cmd.ExecuteNonQuery();

    }


    public static List<TaskItem> GetAll()
    {
        using var connection = OpenConnection();
        using var cmd = connection.CreateCommand();

        cmd.CommandText = """
            SELECT id,
                   title,
                   category,
                   is_completed,
                   is_important
            FROM task
            ORDER BY is_important desc, is_completed asc, id;
        """;

        using var reader = cmd.ExecuteReader();

        var tasks = new List<TaskItem>();

        while (reader.Read())
        {
            var task = FromReader(reader);

            tasks.Add(task);
        }

        return tasks;
    }

    public static List<TaskItem> GetByCategory(string category)
    {
        using var connection = OpenConnection();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT id,
                   title,
                   category,
                   is_completed,
                   is_important
            FROM task
            WHERE category = @category
            ORDER BY is_important desc, is_completed asc, id;
        """;

        cmd.Parameters.AddWithValue("@category", category);

        using var reader = cmd.ExecuteReader();

        var tasks = new List<TaskItem>();

        while (reader.Read())
        {
            var task = FromReader(reader);
            tasks.Add(task);
        }

        return tasks;
    }



    public static TaskItem? GetByID(int id)
    {
        using var connection = OpenConnection();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
        SELECT id, title, category, is_completed, is_important
        FROM task
        WHERE id = @id;
    """;
        cmd.Parameters.AddWithValue("@id", id);


        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return FromReader(reader);
    }





}
