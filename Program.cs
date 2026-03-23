using System.Runtime.Serialization.Json;

namespace ExamWork2
{
    [Serializable]
    class User
    {
        public string Login { get; init; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; }
        public List<Result> Results { get; set; }
        public User(string login, DateTime dateOfBirth, string password)
        {
            Login = login;
            DateOfBirth = dateOfBirth;
            Password = password;
            Results = new List<Result>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is User other) return Login == other.Login && Password == other.Password;
            return false;
        }
    }
    [Serializable]
    class Question
    {
        public string Text { get; set; }
        public List<string> Answers { get; set; }
        public int[] CorrectAnswerIndex { get; set; }
        public Question(string text, List<string> answers, int[] correctAnswerIndex)
        {
            Text = text;
            Answers = answers;
            CorrectAnswerIndex = correctAnswerIndex;
        }
    }
    [Serializable]
    class Test
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; }
        public int TotalQuestions => Questions.Count;
        public List<User> Top20User { get; set; }
        public Test(string title, List<Question> questions)
        {
            Title = title;
            Questions = questions;
            Top20User = new List<User>();
        }
    }
    [Serializable]
    class Result
    {
        public string TakenTestTitle { get; init; }
        public int CorrectAnswers { get; set; }
        public Result(string takenTest, int correctAnswers)
        {
            TakenTestTitle = takenTest;
            CorrectAnswers = correctAnswers;
        }
    }
    internal class Program
    {
        static void PaintMenu(string[] menu, int size, int select = 0)
        {
            Console.Clear();
            Console.WriteLine("------Меню------");
            for (int i = 0; i < size; i++)
            {
                if (i == select)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("=>");
                }
                Console.WriteLine("  " + menu[i]);
                Console.ResetColor();
            }
        }
        static int Menu(string[] textMenu, int sizeMenu, int select)
        {
            int key = 0x28;
            while (true)
            {
                if (key == 0x28 || key == 0x26) PaintMenu(textMenu, sizeMenu, select);
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                key = (int)pressedKey.Key;
                switch (key)
                {
                    case 0xD: return select;
                    case 0x26:
                        if (select == 0) select = sizeMenu;
                        select--;
                        break;
                    case 0x28:
                        select++;
                        if (select == sizeMenu) select = 0;
                        break;
                    default:
                        break;
                }
            }
        }
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        static Question NewQuestion()
        {
            string questionText;
            while (true)
            {
                Console.WriteLine("Введіть текст питання:");
                questionText = Console.ReadLine() ?? "";
                if (questionText != "") break;
                else Console.WriteLine("Текст питання не може бути порожнім. Спробуйте ще раз.");
            }
            List<string> answers = new List<string>();
            while (true)
            {
                Console.WriteLine("Введіть варіант відповіді (або 'стоп' для завершення):");
                string answerText = Console.ReadLine() ?? "";
                if (answerText == "")
                {
                    Console.WriteLine("Варіант відповіді не може бути порожнім. Спробуйте ще раз.");
                    continue;
                }
                if (answerText == "стоп") break;
                answers.Add(answerText!);
            }
            string correctInput;
            while (true)
            {
                Console.WriteLine("Введіть індекси правильних відповідей через кому:");
                correctInput = Console.ReadLine() ?? "";
                if (correctInput == "")
                {
                    Console.WriteLine("Індекси правильних відповідей не можуть бути порожніми. Спробуйте ще раз.");
                    continue;
                }
                else break;
            }
            int[] correctIndexes = correctInput!.Split(',').Select(s => int.Parse(s.Trim()) - 1).ToArray();
            return new Question(questionText!, answers, correctIndexes);
        }
        static List<Question> NewQuestions()
        {
            List<Question> questions = new List<Question>();
            while (true)
            {
                Console.Clear();
                questions.Add(NewQuestion());
                Console.WriteLine("Бажаєте додати ще одне питання? (так/ні)");
                string? choice = Console.ReadLine();
                if (choice?.ToLower() != "так") break;
            }
            return questions;
        }
        static Test CreateRandomTest(List<Test> tests)
        {
            List<Question> questions = new List<Question>();
            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                int testIndex = rand.Next(0, tests.Count);
                Test randomTest = tests[testIndex];
                int questionIndex = rand.Next(0, randomTest.Questions.Count);
                questions.Add(randomTest.Questions[questionIndex]);
            }
            return new Test("Змішана вікторина", questions);
        }

        static void TakeQuiz(User user, Test test)
        {
            int correctAnswers = 0;
            for (int i = 0; i < test.TotalQuestions; i++)
            {
                Question question = test.Questions[i];
                Console.Clear();
                Console.WriteLine($"Питання {i + 1}/{test.TotalQuestions}: {question.Text}");
                for (int j = 0; j < question.Answers.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {question.Answers[j]}");
                }
                string input;
                while (true)
                {
                    Console.WriteLine("Введіть номери правильних відповідей через кому:");
                    input = Console.ReadLine() ?? "";
                    if (input != "") break;
                    else Console.WriteLine("Відповідь не може бути порожньою. Спробуйте ще раз.");
                }
                int[] userAnswers = input.Split(',').Select(s => int.Parse(s.Trim()) - 1).ToArray();
                Array.Sort(userAnswers);
                Array.Sort(question.CorrectAnswerIndex);
                if (userAnswers.SequenceEqual(question.CorrectAnswerIndex)) correctAnswers++;
            }
            Console.WriteLine($"Ви відповіли правильно на {correctAnswers} з {test.TotalQuestions} питань.");
            foreach (var result in user.Results)
            {
                if (result.TakenTestTitle == test.Title)
                {
                    result.CorrectAnswers = correctAnswers;
                    Console.WriteLine("Ваш результат оновлено.");
                    return;
                }
            }
            user.Results.Add(new Result(test.Title, correctAnswers));
            // Виправлена логіка для топ-20
            UpdateTop20(test, user, correctAnswers);

            Console.ReadKey();
        }

        static void UpdateTop20(Test test, User user, int correctAnswers)
        {
            test.Top20User.RemoveAll(u => u.Equals(user));
            int insertIndex = test.Top20User.Count;
            for (int i = 0; i < test.Top20User.Count; i++)
            {
                User topUser = test.Top20User[i];
                Result topResult = topUser.Results.Find(r => r.TakenTestTitle == test.Title)!;
                if (topResult == null) continue;
                if (correctAnswers > topResult.CorrectAnswers)
                {
                    insertIndex = i;
                    break;
                }
                else if (correctAnswers == topResult.CorrectAnswers)
                {
                    if (string.Compare(user.Login, topUser.Login, StringComparison.Ordinal) < 0)
                    {
                        insertIndex = i;
                        break;
                    }
                }
            }
            test.Top20User.Insert(insertIndex, user);
            if (test.Top20User.Count > 20)
                test.Top20User.RemoveRange(20, test.Top20User.Count - 20);

        }

        static void SaveToFileAll(string fileNameUser, string fileNameTest, List<User> users, List<Test> tests)
        {
            DataContractJsonSerializer userSerializer = new DataContractJsonSerializer(typeof(List<User>));
            using (Stream stream = File.Create(fileNameUser))
            {
                userSerializer.WriteObject(stream, users);
            }
            using (StreamWriter writer = new StreamWriter(fileNameTest))
            {
                foreach (var test in tests)
                    writer.WriteLine(test.Title);
            }
            foreach (var test in tests)
            {
                string testFileName = $"{test.Title}_data.json";
                DataContractJsonSerializer testSerializer = new DataContractJsonSerializer(typeof(Test));
                using (Stream stream = File.Create(testFileName))
                {
                    testSerializer.WriteObject(stream, test);
                }
            }
        }
        static List<Test> LoadFromFileTest(string fileName)
        {
            List<Test> tests = new List<Test>();
            if (!File.Exists(fileName))
                return tests;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string testFileName = $"{line}_data.json";
                    if (File.Exists(testFileName))
                    {
                        DataContractJsonSerializer testSerializer = new DataContractJsonSerializer(typeof(Test));
                        using (Stream stream = File.OpenRead(testFileName))
                        {
                            Test? test = testSerializer.ReadObject(stream) as Test;
                            if (test != null)
                                tests.Add(test);
                        }
                    }
                }
            }
            return tests;
        }
        static List<User> LoadFromFileUsers(string fileName)
        {
            List<User> users;
            DataContractJsonSerializer userSerializer = new DataContractJsonSerializer(typeof(List<User>));
            if (File.Exists(fileName))
            {
                using (Stream stream = File.OpenRead(fileName))
                {
                    users = userSerializer.ReadObject(stream) as List<User>;
                }
            }
            else users = new List<User>();
            return users!;
        }
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            List<User> users = LoadFromFileUsers("usersData.json");
            List<Test> tests = LoadFromFileTest("testsData.txt");
            User admin = new User("admin", new DateTime(2008, 10, 6), "admin123");
            string[] mainMenu = { "Увійти", "Зареєструватися", "Вийти" };
            int mainMenuSize = mainMenu.Length;
            string[] userMenu = {
                "Пройти вікторину",
                "Переглянути результати",
                "Переглягнути топ-20",
                "Змінити налаштування",
                "Повернутися назад"
            };
            int userMenuSize = userMenu.Length;
            string[] adminMenu = {
                "Додати вікторину",
                "Редагувати вікторину",
                "Переглянути всі вікторини",
                "Повернутися назад"
            };
            int adminMenuSize = adminMenu.Length;
            int select;
            bool exit = false;
            while (!exit)
            {
                select = 0;
                select = Menu(mainMenu, mainMenuSize, select);
                switch (select)
                {
                    case 0:
                        Console.Clear();
                        // Login
                        Console.WriteLine("Введіть логін:");
                        string login = Console.ReadLine() ?? "";
                        Console.WriteLine("Введіть пароль:");
                        string password = ReadPassword();
                        User? foundUser = users!.Find(u => u.Login == login && u.Password == password);
                        select = 0;
                        bool exitUser = false;
                        if (foundUser != null)
                        {
                            Console.WriteLine($"З поверненням, {foundUser.Login}!");
                            Console.ReadKey();
                            // Proceed to user menu
                            while (!exitUser)
                            {
                                select = Menu(userMenu, userMenuSize, select);
                                switch (select)
                                {
                                    case 0:
                                        // Take quiz
                                        string[] quizMenu = tests.Select(t => t.Title).Append("Змішана вікторина").Append("Повернутись назад").ToArray();
                                        int quizMenuSize = quizMenu.Length;
                                        select = 0;
                                        select = Menu(quizMenu, quizMenuSize, select);
                                        if (select == quizMenuSize - 1) break;
                                        if (select == quizMenuSize - 2) TakeQuiz(foundUser, CreateRandomTest(tests));
                                        else TakeQuiz(foundUser, tests[select]);
                                        break;
                                    case 1:
                                        // View results
                                        Console.Clear();
                                        Console.WriteLine("Ваші результати:");
                                        foreach (var result in foundUser.Results)
                                        {
                                            Console.WriteLine($"Тест: {result.TakenTestTitle}, Правильні відповіді: {result.CorrectAnswers}");
                                        }
                                        Console.ReadKey();
                                        break;
                                    case 2:
                                        // View top-20
                                        string[] quizTopMenu = tests.Select(t => t.Title).Append("Повернутись назад").ToArray();
                                        int quizTopMenuSize = quizTopMenu.Length;
                                        select = 0;
                                        select = Menu(quizTopMenu, quizTopMenuSize, select);
                                        if (select == quizTopMenuSize - 1) break;
                                        Test selectedTest = tests[select];
                                        Console.Clear();
                                        Console.WriteLine($"Топ-20 користувачів для тесту: {selectedTest.Title}");
                                        for (int i = 0; i < selectedTest.Top20User.Count; i++)
                                        {
                                            User topUser = selectedTest.Top20User[i];
                                            Result? topResult = topUser.Results.Find(r => r.TakenTestTitle == selectedTest.Title);
                                            if (topResult != null)
                                            {
                                                Console.WriteLine($"{i + 1}. {topUser.Login} - {topResult.CorrectAnswers} правильних відповідей");
                                            }
                                        }
                                        Console.ReadKey();
                                        break;
                                    case 3:
                                        // Change settings
                                        string[] changeMenu = { "Пароль", "Дата народження", "Повернутися назад" };
                                        int changeMenuSize = changeMenu.Length;
                                        select = 0;
                                        select = Menu(changeMenu, changeMenuSize, select);
                                        switch (select)
                                        {
                                            case 0:
                                                // Change password
                                                while (true)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Введіть новий пароль:");
                                                    string? newPassword = Console.ReadLine();
                                                    if (newPassword is not null)
                                                    {
                                                        foundUser.Password = newPassword;
                                                        Console.WriteLine("Пароль успішно змінено.");
                                                        break;
                                                    }
                                                    else
                                                        Console.WriteLine("Пароль не може бути порожнім.");
                                                }
                                                break;
                                            case 1:
                                                // Change date of birth
                                                while (true)
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Введіть нову дату народження (дд-мм-рррр):");
                                                    string? dobInputChange = Console.ReadLine();
                                                    if (DateTime.TryParse(dobInputChange, out DateTime newDob))
                                                    {
                                                        foundUser.DateOfBirth = newDob;
                                                        Console.WriteLine("Дата народження успішно змінена.");
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Недійсний формат дати.");
                                                    }
                                                }
                                                break;
                                            case 2: break;
                                        }
                                        break;
                                    case 4:
                                        // Go back
                                        exitUser = true;
                                        break;
                                }
                            }

                        }
                        else if (foundUser == null)
                        {
                            if (login == admin.Login && password == admin.Password)
                            {
                                bool exitAdmin = false;
                                while (!exitAdmin)
                                {
                                    select = 0;
                                    select = Menu(adminMenu, adminMenuSize, select);
                                    switch (select)
                                    {
                                        case 0:
                                            // Add test
                                            Console.Clear();
                                            Console.WriteLine("Введіть назву тесту:");
                                            string testTitle = Console.ReadLine() ?? "";
                                            while (testTitle == "")
                                            {
                                                Console.WriteLine("Назва тесту не може бути порожньою. Введіть назву тесту:");
                                                testTitle = Console.ReadLine() ?? "";
                                            }
                                            List<Question> questions = NewQuestions();
                                            tests.Add(new Test(testTitle, questions));
                                            Console.WriteLine("Тест успішно додано!");
                                            Console.ReadKey();
                                            break;
                                        case 1:
                                            // Edit test
                                            string[] editMenu = tests.Select(t => t.Title).Append("Повернутись назад").ToArray();
                                            int editMenuSize = editMenu.Length;
                                            select = 0;
                                            select = Menu(editMenu, editMenuSize, select);
                                            if (select == editMenuSize - 1) break;
                                            Test testToEdit = tests[select];
                                            Console.WriteLine("Що бажаєте відредагувати?");
                                            string[] editOptions = { "Назва тесту", "Питання", "Варіанти відповідей", "Індекси правильних відповідей", "Повернутися назад" };
                                            int editOptionsSize = editOptions.Length;
                                            select = 0;
                                            select = Menu(editOptions, editOptionsSize, select);
                                            switch (select)
                                            {
                                                case 0:
                                                    // Edit test title
                                                    Console.WriteLine("Введіть нову назву тесту:");
                                                    string? newTitle = Console.ReadLine();
                                                    if (newTitle is not null)
                                                    {
                                                        testToEdit.Title = newTitle;
                                                        Console.WriteLine("Назву тесту успішно змінено!");
                                                    }
                                                    else Console.WriteLine("Назва тесту не змінена.");
                                                    Console.ReadKey();
                                                    break;
                                                case 1:
                                                    // Edit question
                                                    Console.Clear();
                                                    Console.WriteLine("Ви б хотіли 1 - додати питання чи 2 - змінити одне з них чи 3 - Повернутись назад?");
                                                    string choice = Console.ReadLine() ?? "3";
                                                    if (choice == "1")
                                                    {
                                                        while (true)
                                                        {
                                                            Console.Clear();
                                                            testToEdit.Questions.Add(NewQuestion());
                                                            Console.WriteLine("Питання успішно додано!");
                                                            Console.WriteLine("Бажаєте додати ще одне питання? (так/ні)");
                                                            string addMore = Console.ReadLine() ?? "ні";
                                                            if (addMore.ToLower() != "так") break;
                                                        }
                                                        break;
                                                    }
                                                    else if (choice == "2")
                                                    {
                                                        for (int i = 0; i < testToEdit.TotalQuestions; i++)
                                                        {
                                                            Console.WriteLine($"Питання {i + 1}: {testToEdit.Questions[i].Text}");
                                                            Console.WriteLine("Введіть новий текст питання (або натисніть Enter, щоб пропустити):");
                                                            string? newQuestionText = Console.ReadLine();
                                                            if (!string.IsNullOrEmpty(newQuestionText))
                                                            {
                                                                testToEdit.Questions[i].Text = newQuestionText;
                                                                Console.WriteLine("Текст питання успішно змінено!");
                                                            }
                                                            else Console.WriteLine("Текст питання не змінено.");
                                                        }
                                                    }
                                                    else break;
                                                    break;
                                                case 2:
                                                    // Edit answers
                                                    for (int i = 0; i < testToEdit.TotalQuestions; i++)
                                                    {
                                                        Question question = testToEdit.Questions[i];
                                                        for (int j = 0; j < question.Answers.Count; j++)
                                                        {
                                                            Console.WriteLine($"Питання {i + 1}, Відповідь {j + 1}: {question.Answers[j]}");
                                                            Console.WriteLine("Введіть новий текст відповіді (або натисніть Enter, щоб пропустити):");
                                                            string? newAnswerText = Console.ReadLine();
                                                            if (!string.IsNullOrEmpty(newAnswerText))
                                                            {
                                                                question.Answers[j] = newAnswerText;
                                                                Console.WriteLine("Текст відповіді успішно змінено!");
                                                            }
                                                        }
                                                    }
                                                    Console.ReadKey();
                                                    break;
                                                case 3:
                                                    // Edit correct answer indexes
                                                    for (int i = 0; i < testToEdit.TotalQuestions; i++)
                                                    {
                                                        Question question = testToEdit.Questions[i];
                                                        Console.WriteLine($"Питання {i + 1}: {question.Text}");
                                                        Console.WriteLine("Введіть нові індекси правильних відповідей через кому (або натисніть Enter, щоб пропустити):");
                                                        string? newCorrectInput = Console.ReadLine();
                                                        if (!string.IsNullOrEmpty(newCorrectInput))
                                                        {
                                                            int[] newCorrectIndexes = newCorrectInput.Split(',').Select(s => int.Parse(s.Trim()) - 1).ToArray();
                                                            question.CorrectAnswerIndex = newCorrectIndexes;
                                                            Console.WriteLine("Індекси правильних відповідей успішно змінено!");
                                                        }
                                                        else Console.WriteLine("Індекси правильних відповідей не змінено.");
                                                    }
                                                    Console.ReadKey();
                                                    break;
                                                case 4: break;
                                            }
                                            break;
                                        case 2:
                                            // View all tests
                                            Console.Clear();
                                            foreach (var test in tests)
                                            {
                                                Console.WriteLine($"Тест: {test.Title}, Питання: {test.TotalQuestions}");
                                            }
                                            Console.ReadKey();
                                            break;
                                        case 3:
                                            // Go back
                                            exitAdmin = true;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Недійсний логін або пароль. Можливо ви ще не реєструвались?");
                                Console.ReadKey();
                            }
                        }
                        break;
                    case 1:
                        // Register
                        Console.Clear();
                        Console.WriteLine("Введіть бажаний логін:");
                        string? regLogin = Console.ReadLine();
                        if (users.Any(u => u.Login == regLogin))
                        {
                            Console.WriteLine("Користувач з таким логіном вже існує. Спробуйте інший логін.");
                            break;
                        }
                        Console.WriteLine("Введіть дату народження (дд-мм-рррр):");
                        string? dobInput = Console.ReadLine();
                        DateTime regDob;
                        while (!DateTime.TryParse(dobInput, out regDob))
                        {
                            Console.WriteLine("Недійсний формат дати. Спробуйте ще раз:");
                            dobInput = Console.ReadLine();
                        }
                        Console.WriteLine("Введіть бажаний пароль:");
                        string? regPassword = Console.ReadLine();
                        users.Add(new User(regLogin!, regDob, regPassword!));
                        Console.WriteLine("Реєстрація успішна! Тепер ви можете увійти.");
                        break;
                    case 2:
                        // Exit
                        exit = true;
                        break;
                }

            }
            SaveToFileAll("usersData.json", "testsData.txt", users!, tests);
        }
    }
}
