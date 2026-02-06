namespace TaskBerry;

using Microsoft.Data.Sqlite;

public class TaskItem
{
    private static readonly string _connectionString = "Data Source=taskberry.db;";

    public int Id { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;

    public static void CreateTableIfNotExists()
    {

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS task (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    title TEXT NOT NULL,
                    category TEXT DEFAULT '',
                    is_completed INTEGER NOT NULL DEFAULT 0
                    );
        """;

        cmd.ExecuteNonQuery();
    }

    public void Save()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();

        if (Id == 0)
        {
            cmd.CommandText = """
                INSERT INTO task (title, category, is_completed)
                VALUES           (@title, @category, @is_completed);

            SELECT last_insert_rowid();
            """;
        }
        else
        {
            cmd.CommandText = """
                UPDATE task
                SET title = @title,
                    category = @category,
                    is_completed = @is_completed
                WHERE id = @id;
            """;
        }

        cmd.Parameters.AddWithValue("@id", Id);
        cmd.Parameters.AddWithValue("@title", Title);
        cmd.Parameters.AddWithValue("@category", Category);
        cmd.Parameters.AddWithValue("@is_completed", IsCompleted ? 1 : 0);

        cmd.ExecuteNonQuery();

    }

    public static List<TaskItem> GetAll()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT id,
                   title,
                   category,
                   is_completed
            FROM task
            ORDER BY is_completed asc, id;
        """;

        using var reader = cmd.ExecuteReader();

        var tasks = new List<TaskItem>();

        while (reader.Read())
        {
            var task = new TaskItem
            {
                Id = Convert.ToInt32(reader["id"]),
                Title = reader["title"].ToString()!,
                Category = reader["category"].ToString()!,
                IsCompleted = Convert.ToBoolean(reader["is_completed"])
            };

            tasks.Add(task);
        }

        return tasks;
    }

    public static TaskItem? GetByID(int id)
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
        SELECT id, title, category, is_completed
        FROM task
        WHERE id = @id;
    """;
        cmd.Parameters.AddWithValue("@id", id);


        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null; 

        var task = new TaskItem
        {
            Id = Convert.ToInt32(reader["id"]),
            Title = reader["title"].ToString()!,
            Category = reader["category"].ToString()!,
            IsCompleted = Convert.ToInt32(reader["is_completed"]) != 0
        };

        return task;
    }


}
