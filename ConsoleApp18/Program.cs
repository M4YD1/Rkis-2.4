using Task = ConsoleApp18.Models.Task;
class Program
{
    private static void Main()
    {
        int userId = 0;
        bool usersMenu = true;
        while (usersMenu)
        {
            Console.WriteLine("1) Авторизация " +
                              "\n2) Регистрация " +
                              "\n\nВыберите функцию: ");
            int menu = Convert.ToInt32(Console.ReadLine());
            switch (menu)
            {
                case 1:
                    userId = UserLogin();
                    usersMenu = false;
                    break;
                case 2:
                    userId = SignUpUser();
                    usersMenu = false;
                    break;
                case 3:
                    usersMenu = false;
                    break;
                default:
                    Console.WriteLine("Ошибка ");
                    break;
            }
        }
        if (userId != 0)
        {
            TasksMenu(userId);
        }
    }

    private static int UserLogin()
    {
        string username = GetInput("Введите имя пользователя: ");
        string password = GetInput("Введите пароль: ");
        return DataBaseRequests.UserSignIn(username, password);
    }

    private static int SignUpUser()
    {
        string username = GetInput("Создайте имя пользователя: ");
        string password = GetInput("Создайте пароль: ");
        string confirmPassword = GetInput("Повторно введите пароль: ");
        if (password != confirmPassword)
        {
            Console.WriteLine("Пароли не совпадают");
            return SignUpUser();
        }
        return DataBaseRequests.SignUpUserQuery(username, password);
    }

    private static void TasksMenu(int userId)
    {
        Console.WriteLine("\nАвторизация прошла успешно");
        bool menu1 = true;
        while (menu1)
        {
            int choice = GetMenuChoice("1) Просмотреть задачи" +
                                       "\n2) Добавить задачу" +
                                       "\n3) Удалить задачу" +
                                       "\n4) Редактировать задачу" +
                                       "\n5) Выход" +
                                       "\n\nВыберите функцию:");
            switch (choice)
            {
                case 1:
                    ViewTasks(userId);
                    break;
                case 2:
                    AddTask(userId);
                    break;
                case 3:
                    DeleteTask(userId);
                    break;
                case 4:
                    EditTask(userId);
                    break;
                case 5:
                    menu1 = false;
                    break;
                default:
                    Console.WriteLine("Ошибка");
                    break;
            }
        }
    }

    private static int GetMenuChoice(string menuText)
    {
        while (true)
        {
            Console.WriteLine(menuText);
            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                return choice;
            }
            else
            {
                Console.WriteLine("Ошибка");
            }
        }
    }

    private static void ViewTasks(int userId)
    {
        var allTasks = DataBaseRequests.GetTasksForUserQuery(userId);
        if (allTasks.Count == 0)
        {
            Console.WriteLine("\nЗадачи отсутствуют");
        }
        else
        {
            ViewTaskOptions(allTasks);
        }
    }

    private static void ViewTaskOptions(List<Task> tasks)
    {
        bool print = true;
        while (print)
        {
            int choice = GetMenuChoice("1) Просмотреть актуальные задачи" +
                                       "\n2) Вывести список прошедших задач" +
                                       "\n3) Вывести все задачи" +
                                       "\n4) Выйти" +
                                       "\n\nВыберите нужное действие:");
            switch (choice)
            {
                case 1:
                    ViewPresentTasks(tasks);
                    break;
                case 2:
                    ViewPastTasks(tasks);
                    break;
                case 3:
                    ViewAllTasks(tasks);
                    break;
                case 4:
                    print = false;
                    break;
                default:
                    Console.WriteLine("Ошибка");
                    break;
            }
        }
    }

    private static void ViewPresentTasks(List<Task> tasks)
    {
        var presentTasks = tasks.FindAll(task => DateTime.Now.Date <= task.Time.Date &&  
                                                    (task.Time.Date > DateTime.Now.Date || 
                                                     task.Time.TimeOfDay > DateTime.Now.TimeOfDay));
        if (presentTasks.Count == 0)
        {
            Console.WriteLine("\nАктуальные задачи отсутствуют");
        }
        else
        {
            ViewTaskDetails(presentTasks);
        }
    }

    private static void ViewPastTasks(List<Task> tasks)
    {
        var pastTasks = tasks.FindAll(task =>
            task.Time.Date < DateTime.Today ||
            (task.Time.Date == DateTime.Today &&
             task.Time.TimeOfDay < DateTime.Now.TimeOfDay));
        if (pastTasks.Count == 0)
        {
            Console.WriteLine("\nЗадачи, которые уже прошли отсутствуют");
        }
        else
        {
            ViewTaskDetails(pastTasks);
        }
    }

    private static void ViewAllTasks(List<Task> tasks)
    {
        ViewTaskDetails(tasks);
    }

    private static void ViewTaskDetails(List<Task> tasks)
    {
        foreach (var task in tasks)
        {
            DataBaseRequests.PrintTask(task);
        }
    }

    private static void AddTask(int userId)
    {
        string name = GetInput("Введите название задачи: ");
        string description = GetInput("Введите описание задачи: ");
        string dueDateString = GetInput("Введите дату и время (ДД.ММ.ГГГГ ЧЧ:ММ): ");
        if (DateTime.TryParseExact(dueDateString, "dd.MM.yyyy HH:mm", null,
            System.Globalization.DateTimeStyles.None, out DateTime dueDate))
        {
            if (dueDate < DateTime.Now)
            {
                Console.WriteLine("\nДата не должна быть раньше текущей даты");
                AddTask(userId);
            }
            else
            {
                DataBaseRequests.AddTaskQuery(userId, name, description, dueDate);
            }
        }
        else
        {
            Console.WriteLine("\nОшибка");
            AddTask(userId);
        }
    }

    private static void DeleteTask(int userId)
    {
        var tasks = DataBaseRequests.GetTasksForUserQuery(userId);
        foreach (var task in tasks)
        {
            DataBaseRequests.PrintNameAndTaskId(task);
        }
        int taskId = GetMenuChoice("\nВведите номер задачи, которую хотите удалить: ");
        if (taskId == 0)
        {
            return;
        }
        int checkTask = DataBaseRequests.CheckTaskQuery(userId, taskId);
        if (checkTask == 0)
        {
            Console.WriteLine("\nЗадача недоступна");
        }
        else
        {
            DataBaseRequests.DeleteTaskQuery(taskId);
        }
    }

    private static void EditTask(int userId)
    {
        var tasks = DataBaseRequests.GetTasksForUserQuery(userId);
        foreach (var task in tasks)
        {
            DataBaseRequests.PrintNameAndTaskId(task);
        }
        int taskId = GetMenuChoice("\nВведите id задачи, которую хотите отредактировать: ");
        if (taskId == 0)
        {
            return;
        }
        int checkTask = DataBaseRequests.CheckTaskQuery(userId, taskId);
        if (checkTask == 0)
        {
            Console.WriteLine("\nЗадача недоступна");
        }
        else
        {
            var task = DataBaseRequests.GetTask(taskId);
            string nameEdit = task.Name;
            string descriptionEdit = task.Description;
            DateTime timeEdit = task.Time;
            EditTaskDetails(taskId, nameEdit, descriptionEdit, timeEdit);
        }
    }

    private static void EditTaskDetails(int taskId, string nameEdit, string descriptionEdit, DateTime timeEdit)
    {
        bool edit = true;
        while (edit)
        {
            int choice = GetMenuChoice("1) Изменить название задачи" +
                                       "\n2) Изменить описание задачи" +
                                       "\n3) Изменить дедлайн задачи" +
                                       "\n4) Выйти" +
                                       "\n\nВыберите нужное действие:");
            switch (choice)
            {
                case 1:
                    nameEdit = GetInput("Введите название задачи: ");
                    DataBaseRequests.EditTaskQuery(taskId, nameEdit, descriptionEdit, timeEdit);
                    edit = false;
                    break;
                case 2:
                    descriptionEdit = GetInput("Введите описание задачи: ");
                    DataBaseRequests.EditTaskQuery(taskId, nameEdit, descriptionEdit, timeEdit);
                    edit = false;
                    break;
                case 3:
                    while (true)
                    {
                        string dueDateString = GetInput("Введите дату и время (ДД.ММ.ГГГГ ЧЧ:ММ): ");
                        if (DateTime.TryParseExact(dueDateString, "dd.MM.yyyy HH:mm", null,
                            System.Globalization.DateTimeStyles.None, out DateTime dueDate))
                        {
                            if (dueDate < DateTime.Now)
                            {
                                Console.WriteLine("\nДата не должна быть раньше текущей даты");
                                continue;
                            }
                            timeEdit = dueDate;
                            DataBaseRequests.EditTaskQuery(taskId, nameEdit, descriptionEdit, timeEdit);
                            edit = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\nОшибка");
                        }
                    }
                    break;
                case 4:
                    edit = false;
                    break;
                default:
                    Console.WriteLine("\nОшибка");
                    break;
            }
        }
    }

    private static string GetInput(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
        }
    }
}