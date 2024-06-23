using ConsoleApp18.Models;
using Task = ConsoleApp18.Models.Task;


public class DataBaseRequests
{
    private static PostgresContext db = new PostgresContext();

    public static int UserSignIn(string username, string password)
    {
        User user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            return user.UserId;
        }

        return 0;
    }

    public static int SignUpUserQuery(string username, string password)
    {
        User user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
        {
            User newUser = new User()
            {
                Username = username,
                Password = password
            };
            db.Users.Add(newUser);
            db.SaveChanges();
            return newUser.UserId;
        }

        return 0;
    }

    public static int AddTaskQuery(int userId, string name, string description, DateTime time)
    {
        bool checkTask = db.Tasks.Any(t => t.Name == name && t.Userid == userId && t.Description == description && t.Time == time);
        if (!checkTask)
        {
            Task addTask = new Task()
            {
                Name = name,
                Description = description,
                Time = time,
                Userid = userId
            };

            db.Tasks.Add(addTask);
            db.SaveChanges();
            
            return addTask.TaskId;
        }

        return 0;
    }
    
    public static int DeleteTaskQuery(int taskId)
    {
        Task task = db.Tasks.FirstOrDefault(t => t.TaskId == taskId);
        if (task != null)
        {
            db.Tasks.Remove(task);
            db.SaveChanges();
            return 1; 
        }
        
        return 0;
    }

    public static int CheckTaskQuery(int userId, int taskId)
    {
        bool checkTask = db.Tasks.Any(t => t.TaskId == taskId && t.Userid == userId);

        if (checkTask)
        {
            return 1;
        }

        return 0;
    }

    public static void EditTaskQuery(int taskId, string name, string description, DateTime time)
    {
        Task task = db.Tasks.FirstOrDefault(t => t.TaskId == taskId);
        if (task != null)
        {
            task.Name = name;
            task.Description = description;
            task.Time = time;
            db.SaveChanges(); 
        }
    }

    public static List<Task> GetTasksForUserQuery(int userId)
    {
        return db.Tasks.Where(t => t.Userid == userId).ToList();
    }

    public static Task GetTask(int taskId)
    {
        return db.Tasks.FirstOrDefault(t => t.TaskId == taskId);
    }

    public static void PrintTask(Task task)
    {
        Console.WriteLine($"\nНомер задачи: {task.TaskId}");
        Console.WriteLine($"Название: {task.Name}");
        Console.WriteLine($"Описание: {task.Description}");
        Console.WriteLine($"Время: {task.Time}");
    }

    public static void PrintNameAndTaskId(Task task)
    {
        Console.WriteLine($"\nID задачи: {task.TaskId} | Название: {task.Name}");
    }
}



