using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Name.Models
{
    [Serializable]
    public abstract class Person : ISerializable
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public Person() { }

        public Person(string name, int age, string address)
        {
            Name = name;
            Age = age;
            Address = address;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Age", Age);
            info.AddValue("Address", Address);
        }

        protected Person(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Age = info.GetInt32("Age");
            Address = info.GetString("Address");
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"Name: {Name}, Age: {Age}, Address: {Address}");
        }
    }

    [Serializable]
    public class Student : Person, ISerializable
    {
        public string StudentId { get; set; }
        public string Class { get; set; }
        public List<Score> Scores { get; set; }

        public Student() { }

        public Student(string name, int age, string address, string studentId, string className)
            : base(name, age, address)
        {
            StudentId = studentId;
            Class = className;
            Scores = new List<Score>();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StudentId", StudentId);
            info.AddValue("Class", Class);
            info.AddValue("Scores", Scores);
        }

        protected Student(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            StudentId = info.GetString("StudentId");
            Class = info.GetString("Class");
            Scores = (List<Score>)info.GetValue("Scores", typeof(List<Score>));
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Student ID: {StudentId}, Class: {Class}");
        }

        public double CalculateAverageScore()
        {
            double total = 0;
            foreach (Score score in Scores)
            {
                total += score.CalculateFinalScore();
            }
            return Scores.Count > 0 ? total / Scores.Count : 0;
        }

        public void AddScore(Score score)
        {
            Scores.Add(score);
        }

        public bool ScoreExists(string subject)
        {
            return Scores.Exists(s => s.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Serializable]
    public class Score : ISerializable
    {
        public string Subject { get; set; }
        public double MidtermScore { get; set; }
        public double FinalScore { get; set; }

        public Score() { }

        public Score(string subject, double midtermScore, double finalScore)
        {
            Subject = subject;
            MidtermScore = midtermScore;
            FinalScore = finalScore;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Subject", Subject);
            info.AddValue("MidtermScore", MidtermScore);
            info.AddValue("FinalScore", FinalScore);
        }

        protected Score(SerializationInfo info, StreamingContext context)
        {
            Subject = info.GetString("Subject");
            MidtermScore = info.GetDouble("MidtermScore");
            FinalScore = info.GetDouble("FinalScore");
        }

        public double CalculateFinalScore()
        {
            return (MidtermScore * 0.4) + (FinalScore * 0.6);
        }

        public void DisplayScore()
        {
            Console.WriteLine($"Subject: {Subject}, Midterm: {MidtermScore}, Final: {FinalScore}, Final Score: {CalculateFinalScore():F2}");
        }
    }

    public class FileHandler
    {
        public static void SaveDataToFile(string filePath, List<Student> students)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(students, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<Student> LoadDataFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Student>();

            string jsonString = File.ReadAllText(filePath);
            List<Student>? students = JsonSerializer.Deserialize<List<Student>>(jsonString);
            return students ?? new List<Student>();
        }

        public static void SaveLoginData(string filePath, List<LoginInfo> loginInfos)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(loginInfos, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<LoginInfo> LoadLoginData(string filePath)
        {
            if (!File.Exists(filePath)) return new List<LoginInfo>();

            string jsonString = File.ReadAllText(filePath);
            List<LoginInfo>? loginInfos = JsonSerializer.Deserialize<List<LoginInfo>>(jsonString);
            return loginInfos ?? new List<LoginInfo>();
        }
    }

    [Serializable]
    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public LoginInfo() { }

        public LoginInfo(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }

    public class Menu
    {
        public static void ShowMenu(bool isTeacher)
        {
            Console.WriteLine("=== Student Management System ===");
            if (isTeacher)
            {
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. Add Score");
                Console.WriteLine("3. Display All Students");
                Console.WriteLine("4. Save to File");
                Console.WriteLine("5. Load from File");
                Console.WriteLine("6. Search and View Student's Scores");
                Console.WriteLine("7. Exit");
            }
            else
            {
                Console.WriteLine("1. View Scores");
                Console.WriteLine("2. Exit");
            }
            Console.Write("Choose an option: ");
        }
    }

    public class Program
    {
        static List<Student> students = new List<Student>();
        static string studentFilePath = "students.json";
        static string loginFilePath = "login.json";

        static void Main(string[] args)
        {
            List<LoginInfo> loginInfos = FileHandler.LoadLoginData(loginFilePath);
            string role = Login(loginInfos);

            if (role == "student")
            {
                while (true)
                {
                    Menu.ShowMenu(false);
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            SearchStudentAndDisplayScores();
                            break;
                        case "2":
                            return;
                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
            }
            else if (role == "teacher")
            {
                while (true)
                {
                    Menu.ShowMenu(true);
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            AddStudent();
                            break;
                        case "2":
                            AddScore();
                            break;
                        case "3":
                            DisplayAllStudents();
                            break;
                        case "4":
                            SaveToFile();
                            break;
                        case "5":
                            LoadFromFile();
                            break;
                        case "6":
                            SearchStudentAndDisplayScores();
                            break;
                        case "7":
                            return;
                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
            }
        }

        public static string Login(List<LoginInfo> loginInfos)
        {
            Console.Write("Enter your username: ");
            string username = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            LoginInfo? user = loginInfos.Find(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                Console.WriteLine($"Welcome, {user.Username}!");
                return user.Role;
            }

            Console.WriteLine("Invalid username or password. Exiting...");
            Environment.Exit(0);
            return "";
        }

        public static void SearchStudentAndDisplayScores()
        {
            Console.Write("Enter student ID or name: ");
            string input = Console.ReadLine();

            Student student = students.Find(s => s.StudentId == input || s.Name.Equals(input, StringComparison.OrdinalIgnoreCase));

            if (student != null)
            {
                Console.WriteLine("Student found!");
                student.DisplayInfo();

                if (student.Scores.Count > 0)
                {
                    foreach (Score score in student.Scores)
                    {
                        score.DisplayScore();
                    }
                    Console.WriteLine($"Average Score: {student.CalculateAverageScore():F2}");
                }
                else
                {
                    Console.WriteLine("This student has no scores.");
                }
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        public static void AddStudent()
        {
            Console.Write("Enter student name: ");
            string name = Console.ReadLine();
            Console.Write("Enter student age: ");
            int age = int.Parse(Console.ReadLine());
            Console.Write("Enter student address: ");
            string address = Console.ReadLine();
            Console.Write("Enter student ID: ");
            string studentId = Console.ReadLine();
            Console.Write("Enter student class: ");
            string className = Console.ReadLine();

            Student newStudent = new Student(name, age, address, studentId, className);
            students.Add(newStudent);
            Console.WriteLine("Student added successfully!");
        }

        public static void AddScore()
        {
            Console.Write("Enter student ID or name: ");
            string input = Console.ReadLine();

            Student student = students.Find(s => s.StudentId == input || s.Name.Equals(input, StringComparison.OrdinalIgnoreCase));

            if (student != null)
            {
                Console.Write("Enter subject: ");
                string subject = Console.ReadLine();

                if (!student.ScoreExists(subject))
                {
                    Console.Write("Enter midterm score: ");
                    double midtermScore = double.Parse(Console.ReadLine());
                    Console.Write("Enter final score: ");
                    double finalScore = double.Parse(Console.ReadLine());

                    Score newScore = new Score(subject, midtermScore, finalScore);
                    student.AddScore(newScore);
                    Console.WriteLine("Score added successfully!");
                }
                else
                {
                    Console.WriteLine("Score for this subject already exists.");
                }
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        public static void DisplayAllStudents()
        {
            if (students.Count > 0)
            {
                foreach (Student student in students)
                {
                    student.DisplayInfo();
                }
            }
            else
            {
                Console.WriteLine("No students available.");
            }
        }

        public static void SaveToFile()
        {
            FileHandler.SaveDataToFile(studentFilePath, students);
            Console.WriteLine("Data saved successfully!");
        }

        public static void LoadFromFile()
        {
            students = FileHandler.LoadDataFromFile(studentFilePath);
            Console.WriteLine("Data loaded successfully!");
        }
    }
}
