namespace TaskBerry;

internal class Program
{
    private static void Main(string[] args)
    {
        TaskItem.CreateTableIfNotExists();

        Console.Clear();
        Console.ResetColor();
        Console.WriteLine();

        if (args.Length == 0)
        {
            ShowAllTasks();
            return;
        }

        var commands = new Dictionary<string, Action<string[]>>
        {
            ["add"] = HandleAdd,
            ["update"] = HandleUpdate,
            ["done"] = HandleDone,
            ["undone"] = HandleUndone,
            ["list"] = HandleList,
            ["delete"] = HandleDelete
        };

        var command = args[0].ToLower();

        if (!commands.TryGetValue(command, out var handler))
        {
            ShowHelpMenu();
            return;
        }

        handler(args);
    }

    private static void HandleDelete(string[] args)
    {
        var argument = args.Length > 1 ? args[1] : string.Empty;
        var taskId = int.TryParse(argument, out var id) ? id : 0;

        var taskItem = new TaskItem();
        taskItem = TaskItem.GetByID(taskId);

        if (taskItem == null)
        {
            ShowAllTasks();
            PrintErrorMessage("Please enter a valid Task ID.");
            return;
        }

        taskItem.Delete();

        ShowAllTasks();
        PrintSuccessMessage($"[{taskId}] Task removed.");
    }

    private static void HandleList(string[] args)
    {
        var argument = args.Length > 1 ? args[1] : string.Empty;
        var category = argument.ToLower();
        var all = args.Length > 2 ? args[2] : string.Empty;
        var showCompleted = all.Equals("all") ? true : false;

        if (category == "all")
        {
            ShowAllTasks("", true);
            return;
        }

        if (string.IsNullOrEmpty(category))
        {
            ShowAllTasks();
            return;
        }

        ShowAllTasks(category, showCompleted);
    }

    private static void HandleUndone(string[] args)
    {
        var argument = args.Length > 1 ? args[1] : string.Empty;
        var taskId = int.TryParse(argument, out var id) ? id : 0;

        var taskItem = new TaskItem();
        taskItem = TaskItem.GetByID(taskId);

        if (taskItem == null)
        {
            ShowAllTasks();
            PrintErrorMessage("Please enter a valid Task ID.");
            return;
        }

        taskItem.IsCompleted = false;
        taskItem.Save();

        ShowAllTasks();
        PrintSuccessMessage($"[{taskId}] Task updated.");
    }

    private static void HandleDone(string[] args)
    {
        var argument = args.Length > 1 ? args[1] : string.Empty;
        var taskId = int.TryParse(argument, out var id) ? id : 0;


        var taskItem = new TaskItem();
        taskItem = TaskItem.GetByID(taskId);

        if (taskItem == null)
        {
            ShowAllTasks();
            PrintErrorMessage("Please enter a valid Task ID.");
            return;
        }

        taskItem.IsCompleted = true;
        taskItem.Save();

        ShowAllTasks();
        PrintSuccessMessage($"[{taskId}] Task updated.");
    }

    private static void HandleUpdate(string[] args)
    {
        var argument = args.Length > 1 ? args[1] : string.Empty;
        var taskId = int.TryParse(argument, out var id) ? id : 0;

        if (taskId == 0)
        {
            ShowAllTasks();
            PrintErrorMessage("Please enter a valid Task ID.");
            return;
        }

        var argList = args.ToList();
        argList.RemoveAt(1);

        args = argList.ToArray();

        if (string.IsNullOrEmpty(GetTitle(args)))
        {
            ShowAllTasks();
            PrintErrorMessage($"[{taskId}] Please include a title.");
            return;
        }

        var taskItem = new TaskItem();
        taskItem.Id = taskId;
        taskItem.Title = GetTitle(args);
        taskItem.Category = GetCategory(args);
        taskItem.IsImportant = CheckImportant(args);
        taskItem.Save();

        ShowAllTasks();
        PrintSuccessMessage($"[{taskId}] Task updated.");
    }

    private static void HandleAdd(string[] args)
    {
        if (string.IsNullOrWhiteSpace(GetTitle(args)))
        {
            ShowAllTasks();
            PrintErrorMessage("Please enter a task title.");
            return;
        }

        var taskItem = new TaskItem
        {
            Title = GetTitle(args),
            Category = GetCategory(args),
            IsImportant = CheckImportant(args)
        };

        taskItem.Save();

        ShowAllTasks();
        PrintSuccessMessage("Task added.");
    }

    private static bool CheckImportant(string[] args)
    {
        return args.Any(a => a.ToLower().Equals("--i"));
    }

    private static List<TaskItem> LoadTasks(string category)
    {
        return string.IsNullOrEmpty(category) ? TaskItem.GetAll() : TaskItem.GetByCategory(category);
    }

    private static void ShowAllTasks(string category = "", bool showCompleted = false)
    {
        Console.Clear();
        Console.WriteLine();

        var tasks = LoadTasks(category);

        if (tasks.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.IsNullOrEmpty(category)
                ? "You currently have 0 tasks."
                : $"[{category}] currently has 0 tasks.");
            Console.WriteLine();
            return;
        }


        var paddingCount = GetMaxTitleLength(tasks) + 5;

        PrintRow("ID", "Title", "Category", paddingCount);
        PrintRow("--", "-----", "--------", paddingCount);

        var useDarkGray = true;

        foreach (var task in tasks)
        {
            Console.BackgroundColor = useDarkGray ? ConsoleColor.DarkGray : ConsoleColor.Black;

            if (!task.IsCompleted)
            {
                Console.ForegroundColor = ConsoleColor.White;
                if (task.IsImportant) Console.ForegroundColor = ConsoleColor.DarkYellow;
                PrintRow(task.Id.ToString(), task.Title, task.Category, paddingCount);
            }

            useDarkGray = !useDarkGray;
        }


        if (showCompleted)
        {
            Console.WriteLine();
            Console.WriteLine();

            useDarkGray = true;

            foreach (var task in tasks)
            {
                Console.BackgroundColor = useDarkGray ? ConsoleColor.DarkGray : ConsoleColor.Black;

                if (task.IsCompleted)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    PrintRow(task.Id.ToString(), task.Title, task.Category, paddingCount);
                }

                useDarkGray = !useDarkGray;
            }
        }


        Console.ResetColor();
        Console.WriteLine();
    }

    private static string GetCategory(string[] args)
    {
        var category = string.Empty;

        for (var i = 1; i < args.Length; i++)
            if (args[i].ToLower().Equals("--c"))
            {
                category = args[i + 1];
                break;
            }

        return category;
    }


    private static string GetTitle(string[] args)
    {
        var title = string.Empty;
        foreach (var arg in args.Skip(1))
        {
            title = $"{title} {arg}";
            if (arg.StartsWith("--"))
            {
                title = title.Replace(arg, "");
                break;
            }
        }

        return title.Trim();
    }


    private static void PrintSuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.WriteLine();
        Console.ResetColor();
    }

    private static void PrintErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(message);
        Console.WriteLine();
        Console.ResetColor();
    }

    private static void PrintRow(string taskId, string title, string category, int paddingCount)
    {
        Console.WriteLine(
            taskId.PadLeft(5).PadRight(10) +
            title.PadRight(paddingCount) +
            category.PadRight(10)
        );
    }


    private static int GetMaxTitleLength(List<TaskItem> items)
    {
        var maxLength = 0;
        foreach (var item in items) maxLength = item.Title.Length > maxLength ? item.Title.Length : maxLength;
        return maxLength;
    }


    private static void ShowHelpMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine(
            "  task add <string> --c <string> --i          : Create task with category, mark task as important.");
        Console.WriteLine("  task update ID <string> --c <String>        : Updates the task title and category.");
        Console.WriteLine("  task list                                   : List all pending tasks.");
        Console.WriteLine(
            "  task list <string>                          : List all pending tasks with specified category.");
        Console.WriteLine("  task list <string> all                      : List all tasks with specified category.");
        Console.WriteLine("  task list all                               : List all tasks.");
        Console.WriteLine("  task done ID                                : Mark task completed.");
        Console.WriteLine("  task undone ID                              : Mark task pending.");
        Console.WriteLine("  task delete ID                              : Delete task.");
        Console.WriteLine();
    }
}