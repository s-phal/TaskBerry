using System;

namespace TaskBerry;

class Program
{

    static void Main(string[] args)
    {

        TaskItem.CreateTableIfNotExists();

        Console.Clear();
        Console.WriteLine();

        if (args.Length == 0)
        {
            ShowAllTasks();
            return;
        }

        string command = args[0];
        string argument = args.Length > 1 ? args[1] : string.Empty;
        string title = argument;
        int taskId = int.TryParse(argument, out var id) ? id : 0;



        switch (command)
        {
            case "add":
                {
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        PrintErrorMessage("Please enter a task title.");
                        return;
                    }

                    var taskItem = new TaskItem();
                    taskItem.Title = GetTitle(args);
                    taskItem.Category = GetCategory(args);
                    taskItem.Save();

                    ShowAllTasks();
                    PrintSuccessMessage("Task added.");
                    return;
                }


            case "edit":
                {
                    if (taskId == 0)
                    {
                        ShowAllTasks();
                        PrintErrorMessage("Please enter a valid Task ID.");
                        return;
                    }
                    var argList = args.ToList();

                    argList.RemoveAt(1);

                    args = argList.ToArray();

                    var taskItem = new TaskItem();
                    taskItem.Id = taskId;
                    taskItem.Title = GetTitle(args);
                    taskItem.Category = GetCategory(args);
                    taskItem.Save();

                    ShowAllTasks();
                    PrintSuccessMessage($"[{taskId}] Task updated.");
                    return;
                }

            case "done":
                {
                    if (taskId == 0)
                    {
                        ShowAllTasks();
                        PrintErrorMessage("Please enter a valid Task ID.");
                        return;
                    }




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
                    PrintSuccessMessage($"[{taskId}] Task updated.");
                    ShowAllTasks();
                    return;

                }

            case "list":
                ShowAllTasks();
                break;

            case "help":
                ShowHelpMenu();
                break;

            default:
                ShowHelpMenu();
                break;
        }

        Console.WriteLine("shouldnt hit");
        Console.ReadKey();

    }

    private static string GetCategory(string[] args)
    {
        string category = string.Empty;

        for (int i = 1; i < args.Length; i++)
        {
            if (args[i].ToLower().Equals("--cat") || args[i].ToLower().Equals("--category"))
            {
                category = args[i + 1];
                break;
            }
        }

        return category;
    }



    static string GetTitle(string[] args)
    {
        string title = string.Empty;
        foreach (string arg in args.Skip(1))
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


    static void PrintSuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.WriteLine();
        Console.ResetColor();
    }

    static void PrintErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(message);
        Console.WriteLine();
        Console.ResetColor();
    }

    static void PrintRow(string taskId, string title, string category, int paddingCount)
    {
        Console.WriteLine(
            taskId.PadRight(5) +
            title.PadRight(paddingCount) +
            category
        );
    }

    static void ShowAllTasks()
    {
        Console.Clear();
        Console.WriteLine();

        var tasks = TaskItem.GetAll();

        if (tasks.Count == 0)
        {
            Console.WriteLine("0 tasks.");
            return;
        }

        int paddingCount = GetMaxTitleLength(tasks) + 5;

        PrintRow("ID", "Title", "Category", paddingCount);
        PrintRow("--", "-----", "--------", paddingCount);

        foreach (var task in tasks)
        {
            Console.ResetColor();

            if (task.IsCompleted)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrintRow(task.Id.ToString(), task.Title, task.Category, paddingCount);
            }
            else
            {
                PrintRow(task.Id.ToString(), task.Title, task.Category, paddingCount);
            }
        }

        Console.WriteLine();
    }

    static int GetMaxTitleLength(List<TaskItem> items)
    {
        int maxLength = 0;
        foreach (var item in items)
        {
            maxLength = item.Title.Length > maxLength ? item.Title.Length : maxLength;
        }
        return maxLength;

    }

    static void ShowHelpMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  task add <title> []");

        // var grid = new Grid();
        // grid.AddColumn(new GridColumn().NoWrap());
        // grid.AddColumn(new GridColumn().PadLeft(2));
        // grid.AddRow("Options:");
        // grid.AddRow("  [blue]-h[/], [blue]--help[/]", "Show command line help.");
        // grid.AddRow("  [blue]-c[/], [blue]--configuration[/] <CONFIGURATION>", "The configuration to run for.");
        // grid.AddRow("  [blue]-v[/], [blue]--verbosity[/] <LEVEL>", "Set the [grey]MSBuild[/] verbosity level.");
        //
        // Console.Write(grid);

    }


}

