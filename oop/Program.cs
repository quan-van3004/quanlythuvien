using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
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
            Console.WriteLine($"Tên: {Name}, Tuổi: {Age}, Địa chỉ: {Address}");
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
            Console.WriteLine($"Môn học: {Subject}, Giữa kỳ: {MidtermScore}, Cuối kỳ: {FinalScore}, Điểm cuối: {CalculateFinalScore():F2}");
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
            Console.WriteLine($"Mã học sinh: {StudentId}, Lớp: {Class}");
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

        public void UpdateScore(string subject, double midtermScore, double finalScore)
        {
            Score score = Scores.Find(s => s.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
            if (score != null)
            {
                score.MidtermScore = midtermScore;
                score.FinalScore = finalScore;
                Console.WriteLine("Điểm môn học đã được cập nhật.");
            }
            else
            {
                Console.WriteLine("Không tìm thấy môn học này.");
            }
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
            Console.WriteLine("=== Hệ thống quản lý học sinh ===");
            if (isTeacher)
            {
                Console.WriteLine("1. Thêm học sinh");
                Console.WriteLine("2. Thêm hoặc sửa điểm");
                Console.WriteLine("3. Sửa thông tin của học sinh");
                Console.WriteLine("4. Tìm kiếm và xem điểm của học sinh");
                Console.WriteLine("5. danh sách học sinh");
                Console.WriteLine("6. Thoát");
            }
            else
            {
                Console.WriteLine("1. Xem điểm");
                Console.WriteLine("2. Thoát");
            }
            Console.Write("Chọn một tùy chọn: ");
        }

    }

    public class StudentSearch
    {
        public static Student SearchStudent(List<Student> students, string input)
        {
            return students.Find(s => s.StudentId == input || s.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
        }

        public static List<Student> SearchByClass(List<Student> students, string className)
        {
            return students.FindAll(s => s.Class.Equals(className, StringComparison.OrdinalIgnoreCase));
        }

        public static List<Student> SearchByScore(List<Student> students, double minScore)
        {
            return students.FindAll(s => s.CalculateAverageScore() >= minScore);
        }
    }

    public class Program
    {
        static List<Student> students = new List<Student>();
        static string studentFilePath = "students.json";
        static string loginFilePath = "login.json";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            List<LoginInfo> loginInfos = FileHandler.LoadLoginData(loginFilePath);
            string role = Login(loginInfos);
            students = FileHandler.LoadDataFromFile(studentFilePath); // Tải dữ liệu từ file khi khởi động

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
                            Console.WriteLine("Tùy chọn không hợp lệ, vui lòng thử lại.");
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
                            AddOrUpdateScore();
                            break;
                        case "3":
                            UpdateStudentInfo();
                            break;
                        case "4":
                            SearchStudentAndDisplayScores();
                            break;
                        case "5":
                            SummarizeByClass();
                            break;
                        case "6":
                            return;
                        default:
                            Console.WriteLine("Tùy chọn không hợp lệ, vui lòng thử lại.");
                            break;
                    }
                }
            }

        }

        public static string Login(List<LoginInfo> loginInfos)
        {
            Console.Write("Nhập tên đăng nhập: ");
            string username = Console.ReadLine();
            Console.Write("Nhập mật khẩu: ");
            string password = Console.ReadLine();

            LoginInfo? loginInfo = loginInfos.Find(info => info.Username == username && info.Password == password);
            if (loginInfo != null)
            {
                return loginInfo.Role;
            }

            Console.WriteLine("Tên đăng nhập hoặc mật khẩu không đúng.");
            return string.Empty; // Trả về chuỗi rỗng nếu không đăng nhập thành công
        }

        public static void SearchStudentAndDisplayScores()
        {
            students = FileHandler.LoadDataFromFile(studentFilePath);
            Console.Write("Nhập mã học sinh hoặc tên học sinh: ");
            string input = Console.ReadLine();
            Student student = StudentSearch.SearchStudent(students, input);

            if (student != null)
            {
                student.DisplayInfo();
                foreach (Score score in student.Scores)
                {
                    score.DisplayScore();
                }
            }
            else
            {
                Console.WriteLine("Không tìm thấy học sinh.");
            }
        }

        public static void AddStudent()
        {
            Console.Write("Nhập tên học sinh: ");
            string name = Console.ReadLine();
            Console.Write("Nhập tuổi: ");
            if (!int.TryParse(Console.ReadLine(), out int age))
            {
                Console.WriteLine("Tuổi không hợp lệ. Vui lòng nhập lại.");
                return;
            }
            Console.Write("Nhập địa chỉ: ");
            string address = Console.ReadLine();
            Console.Write("Nhập mã học sinh: ");
            string studentId = Console.ReadLine();
            Console.Write("Nhập lớp: ");
            string className = Console.ReadLine();

            Student student = new Student(name, age, address, studentId, className);
            students.Add(student);
            FileHandler.SaveDataToFile(studentFilePath, students);
            Console.WriteLine("Đã thêm học sinh thành công!");
        }
        public static void SummarizeByClass()
        {
            // Nhóm học sinh theo lớp
            var studentsByClass = new Dictionary<string, List<Student>>();

            foreach (var student in students)
            {
                if (!studentsByClass.ContainsKey(student.Class))
                {
                    studentsByClass[student.Class] = new List<Student>();
                }
                studentsByClass[student.Class].Add(student);
            }

            // Hiển thị kết quả tổng kết
            foreach (var entry in studentsByClass)
            {
                Console.WriteLine($"Lớp: {entry.Key}");
                foreach (var student in entry.Value)
                {
                    Console.WriteLine($"  - {student.Name}, Mã học sinh: {student.StudentId}");
                }
            }
        }


        public static void AddOrUpdateScore()
        {
            Console.Write("Nhập mã học sinh: ");
            string studentId = Console.ReadLine();
            Student student = StudentSearch.SearchStudent(students, studentId);
            if (student == null)
            {
                Console.WriteLine("Không tìm thấy học sinh.");
                return;
            }

            Console.Write("Nhập tên môn học: ");
            string subject = Console.ReadLine();
            Console.Write("Nhập điểm giữa kỳ: ");
            if (!double.TryParse(Console.ReadLine(), out double midtermScore))
            {
                Console.WriteLine("Điểm giữa kỳ không hợp lệ. Vui lòng nhập lại.");
                return;
            }
            Console.Write("Nhập điểm cuối kỳ: ");
            if (!double.TryParse(Console.ReadLine(), out double finalScore))
            {
                Console.WriteLine("Điểm cuối kỳ không hợp lệ. Vui lòng nhập lại.");
                return;
            }

            if (student.ScoreExists(subject))
            {
                student.UpdateScore(subject, midtermScore, finalScore);
            }
            else
            {
                Score score = new Score(subject, midtermScore, finalScore);
                student.AddScore(score);
                Console.WriteLine("Đã thêm điểm môn học.");
            }

            FileHandler.SaveDataToFile(studentFilePath, students);
        }

        public static void UpdateStudentInfo()
        {
            Console.Write("Nhập mã học sinh: ");
            string studentId = Console.ReadLine();
            Student student = StudentSearch.SearchStudent(students, studentId);
            if (student == null)
            {
                Console.WriteLine("Không tìm thấy học sinh.");
                return;
            }

            Console.Write("Nhập tên mới (bỏ trống nếu không thay đổi): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName)) student.Name = newName;

            Console.Write("Nhập tuổi mới (bỏ trống nếu không thay đổi): ");
            string ageInput = Console.ReadLine();
            if (int.TryParse(ageInput, out int newAge)) student.Age = newAge;

            Console.Write("Nhập địa chỉ mới (bỏ trống nếu không thay đổi): ");
            string newAddress = Console.ReadLine();
            if (!string.IsNullOrEmpty(newAddress)) student.Address = newAddress;

            Console.Write("Nhập lớp mới (bỏ trống nếu không thay đổi): ");
            string newClass = Console.ReadLine();
            if (!string.IsNullOrEmpty(newClass)) student.Class = newClass;

            FileHandler.SaveDataToFile(studentFilePath, students);
            Console.WriteLine("Đã cập nhật thông tin học sinh thành công!");
        }
    }
}
