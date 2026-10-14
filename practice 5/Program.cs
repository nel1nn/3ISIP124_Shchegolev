using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversityManagementSystem
{
    public enum AcademicDegree
    {
        Assistant = 1,
        SeniorLecturer = 2,
        AssociateProfessor = 3,
        Professor = 4
    }

    public abstract class Person
    {
        private string name = string.Empty;
        private int age;
        private string email = string.Empty;

        public int Id { get; }

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя не может быть пустым.");
                name = value.Trim();
            }
        }

        public int Age
        {
            get => age;
            set
            {
                if (value < 16 || value > 100)
                    throw new ArgumentException("Возраст должен быть в диапазоне от 16 до 100 лет.");
                age = value;
            }
        }

        public string Email
        {
            get => email;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains('@') || !value.Contains('.'))
                    throw new ArgumentException("Некорректный формат электронной почты (должен содержать '@' и '.').");
                email = value.Trim();
            }
        }

        protected Person(int id, string name, int age, string email)
        {
            if (id <= 0)
                throw new ArgumentException("ID должен быть положительным числом.");

            Id = id;
            Name = name;
            Age = age;
            Email = email;
        }

        public abstract string RoleName { get; }
        public abstract string GetDetailedInfo();

        public virtual string GetShortSummary()
        {
            return $"[ID: {Id}] ({RoleName}) {Name}, {Age} лет | Email: {Email}";
        }
    }

    public class Student : Person
    {
        private int _studyYear;
        private readonly List<Course> _enrolledCourses = new();

        public int StudyYear
        {
            get => _studyYear;
            set
            {
                if (value < 1 || value > 6)
                    throw new ArgumentException("Курс обучения должен быть от 1 до 6.");
                _studyYear = value;
            }
        }
        public IReadOnlyList<Course> EnrolledCourses => _enrolledCourses.AsReadOnly();

        public override string RoleName => "Студент";

        public Student(int id, string name, int age, string email, int studyYear)
            : base(id, name, age, email)
        {
            StudyYear = studyYear;
        }

        internal void AddCourseInternal(Course course)
        {
            if (!_enrolledCourses.Contains(course))
            {
                _enrolledCourses.Add(course);
            }
        }

        public override string GetDetailedInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--- Профиль студента [ID: {Id}] ---");
            sb.AppendLine($"ФИО:           {Name}");
            sb.AppendLine($"Возраст:       {Age}");
            sb.AppendLine($"Email:         {Email}");
            sb.AppendLine($"Год обучения:  {StudyYear}-й курс");
            sb.AppendLine($"Записан на дисциплин: {_enrolledCourses.Count}");

            if (_enrolledCourses.Any())
            {
                sb.AppendLine("Список изучаемых курсов:");
                foreach (var c in _enrolledCourses)
                {
                    string teacherName = c.Instructor != null ? c.Instructor.Name : "Не назначен";
                    sb.AppendLine($"  • [{c.Id}] {c.Title} ({c.Credits} кред., преп.: {teacherName})");
                }
            }
            else
            {
                sb.AppendLine("  (Студент пока не записан ни на один курс)");
            }

            return sb.ToString();
        }
    }

    public class Teacher : Person
    {
        private string _department = string.Empty;
        private readonly List<Course> _assignedCourses = new();

        public string Department
        {
            get => _department;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название кафедры не может быть пустым.");
                _department = value.Trim();
            }
        }

        public AcademicDegree Degree { get; set; }

        public IReadOnlyList<Course> AssignedCourses => _assignedCourses.AsReadOnly();

        public override string RoleName => "Преподаватель";

        public Teacher(int id, string name, int age, string email, string department, AcademicDegree degree)
            : base(id, name, age, email)
        {
            Department = department;
            Degree = degree;
        }

        internal void AddAssignedCourseInternal(Course course)
        {
            if (!_assignedCourses.Contains(course))
                _assignedCourses.Add(course);
        }

        internal void RemoveAssignedCourseInternal(Course course)
        {
            _assignedCourses.Remove(course);
        }

        public static string FormatDegree(AcademicDegree degree) => degree switch
        {
            AcademicDegree.Assistant => "Ассистент",
            AcademicDegree.SeniorLecturer => "Старший преподаватель",
            AcademicDegree.AssociateProfessor => "Доцент",
            AcademicDegree.Professor => "Профессор",
            _ => "Преподаватель"
        };

        public override string GetDetailedInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--- Профиль преподавателя [ID: {Id}] ---");
            sb.AppendLine($"ФИО:             {Name}");
            sb.AppendLine($"Возраст:         {Age}");
            sb.AppendLine($"Email:           {Email}");
            sb.AppendLine($"Кафедра:         {Department}");
            sb.AppendLine($"Ученое звание:   {FormatDegree(Degree)}");
            sb.AppendLine($"Ведет курсов:    {_assignedCourses.Count}");

            if (_assignedCourses.Any())
            {
                sb.AppendLine("Преподаваемые дисциплины:");
                foreach (var c in _assignedCourses)
                {
                    sb.AppendLine($"  • [{c.Id}] {c.Title} (Студентов: {c.Students.Count}/{c.MaxCapacity})");
                }
            }
            else
            {
                sb.AppendLine("  (На данный момент курсы не назначены)");
            }

            return sb.ToString();
        }
    }

    public class Course
    {
        private string _title = string.Empty;
        private int _credits;
        private int _maxCapacity;
        private readonly List<Student> _students = new();

        public int Id { get; }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название курса не может быть пустым.");
                _title = value.Trim();
            }
        }

        public int Credits
        {
            get => _credits;
            set
            {
                if (value <= 0 || value > 30)
                    throw new ArgumentException("Количество кредитов (зачетных единиц) должно быть от 1 до 30.");
                _credits = value;
            }
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
            set
            {
                if (value <= 0 || value > 500)
                    throw new ArgumentException("Вместимость курса должна быть положительным числом (1 - 500).");
                _maxCapacity = value;
            }
        }

        public Teacher? Instructor { get; private set; }

        public IReadOnlyList<Student> Students => _students.AsReadOnly();

        public Course(int id, string title, int credits, int maxCapacity, Teacher? instructor = null)
        {
            if (id <= 0)
                throw new ArgumentException("ID курса должен быть положительным числом.");

            Id = id;
            Title = title;
            Credits = credits;
            MaxCapacity = maxCapacity;

            if (instructor != null)
            {
                AssignInstructor(instructor);
            }
        }

        public void AssignInstructor(Teacher newInstructor)
        {
            if (newInstructor == null)
                throw new ArgumentNullException(nameof(newInstructor), "Преподаватель не может быть пустым (null).");

            Instructor?.RemoveAssignedCourseInternal(this);

            Instructor = newInstructor;
            newInstructor.AddAssignedCourseInternal(this);
        }

        public bool TryEnrollStudent(Student student, out string message)
        {
            if (student == null)
            {
                message = "Ошибка: объект студента не существует.";
                return false;
            }

            if (_students.Any(s => s.Id == student.Id))
            {
                message = $"Студент {student.Name} уже записан на курс «{Title}».";
                return false;
            }

            if (_students.Count >= MaxCapacity)
            {
                message = $"На курсе «{Title}» нет свободных мест (лимит: {MaxCapacity}).";
                return false;
            }

            _students.Add(student);
            student.AddCourseInternal(this);
            message = $"Студент {student.Name} успешно записан на курс «{Title}».";
            return true;
        }

        public string GetDetailedInfo()
        {
            var sb = new StringBuilder();
            string instructorInfo = Instructor != null
                ? $"{Instructor.Name} ({Teacher.FormatDegree(Instructor.Degree)}, каф. {Instructor.Department})"
                : "Не назначен";

            sb.AppendLine($"--- Информация о курсе [ID: {Id}] ---");
            sb.AppendLine($"Название:       {Title}");
            sb.AppendLine($"Трудоемкость:   {Credits} кред.");
            sb.AppendLine($"Преподаватель:  {instructorInfo}");
            sb.AppendLine($"Заполненность:  {_students.Count} из {MaxCapacity} мест");

            if (_students.Any())
            {
                sb.AppendLine("Список записанных студентов:");
                foreach (var s in _students.OrderBy(st => st.Name))
                {
                    sb.AppendLine($"  • [ID: {s.Id}] {s.Name} ({s.StudyYear}-й курс, {s.Email})");
                }
            }
            else
            {
                sb.AppendLine("  (На этот курс пока не записался ни один студент)");
            }

            return sb.ToString();
        }
    }

    public class UniversityManager
    {
        private readonly List<Person> _people = new();
        private readonly List<Course> _courses = new();

        private int _nextPersonId = 1;
        private int _nextCourseId = 101;

        public IEnumerable<Person> AllPeople => _people;
        public IEnumerable<Student> Students => _people.OfType<Student>();
        public IEnumerable<Teacher> Teachers => _people.OfType<Teacher>();
        public IReadOnlyList<Course> Courses => _courses.AsReadOnly();

        public Student AddStudent(string name, int age, string email, int studyYear)
        {
            if (_people.Any(p => p.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Человек с таким Email уже зарегистрирован в системе.");

            var student = new Student(_nextPersonId++, name, age, email, studyYear);
            _people.Add(student);
            return student;
        }

        public Teacher AddTeacher(string name, int age, string email, string department, AcademicDegree degree)
        {
            if (_people.Any(p => p.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Человек с таким Email уже зарегистрирован в системе.");

            var teacher = new Teacher(_nextPersonId++, name, age, email, department, degree);
            _people.Add(teacher);
            return teacher;
        }

        public Course AddCourse(string title, int credits, int maxCapacity, Teacher? instructor = null)
        {
            if (_courses.Any(c => c.Title.Equals(title.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Курс с таким названием уже существует.");

            var course = new Course(_nextCourseId++, title, credits, maxCapacity, instructor);
            _courses.Add(course);
            return course;
        }

        public Student? FindStudentById(int id) => Students.FirstOrDefault(s => s.Id == id);
        public Teacher? FindTeacherById(int id) => Teachers.FirstOrDefault(t => t.Id == id);
        public Course? FindCourseById(int id) => _courses.FirstOrDefault(c => c.Id == id);

        public void SeedInitialData()
        {
            var t1 = AddTeacher("Смирнов Алексей Викторович", 48, "smirnov@fa.ru", "Программная инженерия", AcademicDegree.Professor);
            var t2 = AddTeacher("Ковалева Елена Сергеевна", 35, "kovaleva@uni.edu", "Прикладная математика", AcademicDegree.AssociateProfessor);

            var s1 = AddStudent("Иванов Иван", 19, "ivanov@fa.ru", 2);
            var s2 = AddStudent("Петрова Анна", 20, "petrova@fa.ru", 3);
            var s3 = AddStudent("Сидоров Максим", 18, "sidorov@fa.ru", 1);

            var c1 = AddCourse("Объектно-ориентированное программирование на C#", 6, 25, t1);
            var c2 = AddCourse("Базы данных и SQL", 4, 20, t1);
            var c3 = AddCourse("Дискретная математика", 5, 30, t2);

            c1.TryEnrollStudent(s1, out _);
            c1.TryEnrollStudent(s2, out _);
            c2.TryEnrollStudent(s2, out _);
            c3.TryEnrollStudent(s1, out _);
            c3.TryEnrollStudent(s3, out _);
        }
    }

    public static class InputHelper
    {
        public static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                PrintError("Значение не может быть пустым или состоять только из пробелов. Повторите ввод.");
            }
        }

        public static string ReadEmail(string prompt)
        {
            while (true)
            {
                string input = ReadNonEmptyString(prompt);
                if (input.Contains('@') && input.Contains('.') && input.Length >= 5)
                    return input;

                PrintError("Некорректный Email (пример: user@domain.com). Повторите ввод.");
            }
        }

        public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int result))
                {
                    if (result >= min && result <= max)
                        return result;

                    PrintError($"Число должно быть в диапазоне от {min} до {max}.");
                }
                else
                {
                    PrintError("Ошибка ввода! Введите корректное целое число.");
                }
            }
        }

        public static AcademicDegree ReadDegree()
        {
            Console.WriteLine("Выберите ученое звание преподавателя:");
            Console.WriteLine("  1. Ассистент");
            Console.WriteLine("  2. Старший преподаватель");
            Console.WriteLine("  3. Доцент");
            Console.WriteLine("  4. Профессор");

            int choice = ReadInt("Ваш выбор (1-4): ", 1, 4);
            return (AcademicDegree)choice;
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Ошибка] {message}");
            Console.ResetColor();
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Успешно] {message}");
            Console.ResetColor();
        }

        public static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n==================== {title.ToUpper()} ====================");
            Console.ResetColor();
        }
    }

    class Program
    {
        private static readonly UniversityManager _uni = new();

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            _uni.SeedInitialData();

            bool isRunning = true;
            while (isRunning)
            {
                ShowMainMenu();
                int choice = InputHelper.ReadInt("\nВыберите пункт меню (0 - 13): ", 0, 13);

                try
                {
                    switch (choice)
                    {
                        case 1: CreateStudentFlow(); break;
                        case 2: ShowStudentDetailsFlow(); break;
                        case 3: EnrollStudentInCourseFlow(); break;
                        case 4: ShowStudentCoursesFlow(); break;
                        case 5: CreateTeacherFlow(); break;
                        case 6: ShowTeacherDetailsFlow(); break;
                        case 7: AssignTeacherToCourseFlow(); break;
                        case 8: CreateCourseFlow(); break;
                        case 9: ShowCourseDetailsFlow(); break;
                        case 10: ShowAllStudentsFlow(); break;
                        case 11: ShowAllTeachersFlow(); break;
                        case 12: ShowAllCoursesFlow(); break;
                        case 13: ShowPolymorphicRegistryFlow(); break;
                        case 0:
                            isRunning = false;
                            Console.WriteLine("\nЗавершение работы системы. До свидания!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    InputHelper.PrintError($"Возникла ошибка: {ex.Message}");
                }

                if (isRunning)
                {
                    Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                    Console.ReadKey(true);
                }
            }
        }

        private static void ShowMainMenu()
        {
            Console.Clear();
            InputHelper.PrintHeader("Система управления университетом");
            Console.WriteLine("СТУДЕНТЫ:");
            Console.WriteLine("  1. Добавить нового студента");
            Console.WriteLine("  2. Просмотреть подробную информацию о студенте");
            Console.WriteLine("  3. Записать студента на курс");
            Console.WriteLine("  4. Показать список курсов конкретного студента");
            Console.WriteLine("\nПРЕПОДАВАТЕЛИ:");
            Console.WriteLine("  5. Добавить нового преподавателя");
            Console.WriteLine("  6. Просмотреть информацию о преподавателе");
            Console.WriteLine("  7. Назначить преподавателя на курс");
            Console.WriteLine("\nКУРСЫ:");
            Console.WriteLine("  8. Создать новый курс");
            Console.WriteLine("  9. Детальная информация о курсе и список записанных студентов");
            Console.WriteLine("\nОБЩИЕ СПИСКИ И ОТЧЕТЫ:");
            Console.WriteLine("  10. Список всех студентов");
            Console.WriteLine("  11. Список всех преподавателей");
            Console.WriteLine("  12. Список всех доступных курсов");
            Console.WriteLine("  13. Единый реестр участников (демонстрация Полиморфизма)");
            Console.WriteLine("\n  0. Выход из программы");
        }

        private static void CreateStudentFlow()
        {
            InputHelper.PrintHeader("Добавление нового студента");
            string name = InputHelper.ReadNonEmptyString("Введите ФИО студента: ");
            int age = InputHelper.ReadInt("Введите возраст (16-100): ", 16, 100);
            string email = InputHelper.ReadEmail("Введите Email студента: ");
            int year = InputHelper.ReadInt("Введите курс обучения (1-6): ", 1, 6);

            var student = _uni.AddStudent(name, age, email, year);
            InputHelper.PrintSuccess($"Студент «{student.Name}» добавлен с ID {student.Id}.");
        }

        private static void ShowStudentDetailsFlow()
        {
            InputHelper.PrintHeader("Информация о студенте");
            if (!EnsureStudentsExist()) return;

            PrintBriefStudentsList();
            int id = InputHelper.ReadInt("\nВведите ID студента: ", 1);
            var student = _uni.FindStudentById(id);

            if (student == null)
            {
                InputHelper.PrintError($"Студент с ID {id} не найден.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine(student.GetDetailedInfo());
        }

        private static void EnrollStudentInCourseFlow()
        {
            InputHelper.PrintHeader("Запись студента на курс");
            if (!EnsureStudentsExist() || !EnsureCoursesExist()) return;

            PrintBriefStudentsList();
            int studentId = InputHelper.ReadInt("\nВведите ID студента: ", 1);
            var student = _uni.FindStudentById(studentId);
            if (student == null)
            {
                InputHelper.PrintError("Студент с указанным ID не найден.");
                return;
            }

            Console.WriteLine();
            PrintBriefCoursesList();
            int courseId = InputHelper.ReadInt("\nВведите ID курса для записи: ", 1);
            var course = _uni.FindCourseById(courseId);
            if (course == null)
            {
                InputHelper.PrintError("Курс с указанным ID не найден.");
                return;
            }

            if (course.TryEnrollStudent(student, out string message))
                InputHelper.PrintSuccess(message);
            else
                InputHelper.PrintError(message);
        }

        private static void ShowStudentCoursesFlow()
        {
            InputHelper.PrintHeader("Курсы студента");
            if (!EnsureStudentsExist()) return;

            PrintBriefStudentsList();
            int id = InputHelper.ReadInt("\nВведите ID студента: ", 1);
            var student = _uni.FindStudentById(id);

            if (student == null)
            {
                InputHelper.PrintError("Студент не найден.");
                return;
            }

            Console.WriteLine($"\nКурсы, на которые записан студент {student.Name} ({student.StudyYear}-й курс):");
            if (!student.EnrolledCourses.Any())
            {
                Console.WriteLine("  Студент пока не записан ни на один курс.");
                return;
            }

            int totalCredits = student.EnrolledCourses.Sum(c => c.Credits);
            foreach (var c in student.EnrolledCourses)
            {
                string teacher = c.Instructor != null ? c.Instructor.Name : "Не назначен";
                Console.WriteLine($"  • [ID: {c.Id}] {c.Title} | {c.Credits} кред. | Преподаватель: {teacher}");
            }
            Console.WriteLine($"Суммарная нагрузка: {totalCredits} зачетных единиц (кредитов).");
        }

        private static void CreateTeacherFlow()
        {
            InputHelper.PrintHeader("Добавление нового преподавателя");
            string name = InputHelper.ReadNonEmptyString("Введите ФИО преподавателя: ");
            int age = InputHelper.ReadInt("Введите возраст (20-100): ", 20, 100);
            string email = InputHelper.ReadEmail("Введите Email преподавателя: ");
            string dept = InputHelper.ReadNonEmptyString("Введите название кафедры: ");
            AcademicDegree degree = InputHelper.ReadDegree();

            var teacher = _uni.AddTeacher(name, age, email, dept, degree);
            InputHelper.PrintSuccess($"Преподаватель «{teacher.Name}» добавлен с ID {teacher.Id}.");
        }

        private static void ShowTeacherDetailsFlow()
        {
            InputHelper.PrintHeader("Информация о преподавателе");
            if (!EnsureTeachersExist()) return;

            PrintBriefTeachersList();
            int id = InputHelper.ReadInt("\nВведите ID преподавателя: ", 1);
            var teacher = _uni.FindTeacherById(id);

            if (teacher == null)
            {
                InputHelper.PrintError($"Преподаватель с ID {id} не найден.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine(teacher.GetDetailedInfo());
        }

        private static void AssignTeacherToCourseFlow()
        {
            InputHelper.PrintHeader("Назначение преподавателя на курс");
            if (!EnsureTeachersExist() || !EnsureCoursesExist()) return;

            PrintBriefTeachersList();
            int teacherId = InputHelper.ReadInt("\nВведите ID преподавателя: ", 1);
            var teacher = _uni.FindTeacherById(teacherId);
            if (teacher == null)
            {
                InputHelper.PrintError("Преподаватель не найден.");
                return;
            }

            Console.WriteLine();
            PrintBriefCoursesList();
            int courseId = InputHelper.ReadInt("\nВведите ID курса: ", 1);
            var course = _uni.FindCourseById(courseId);
            if (course == null)
            {
                InputHelper.PrintError("Курс не найден.");
                return;
            }

            course.AssignInstructor(teacher);
            InputHelper.PrintSuccess($"Преподаватель {teacher.Name} назначен ведущим курса «{course.Title}».");
        }

        private static void CreateCourseFlow()
        {
            InputHelper.PrintHeader("Создание нового курса");
            string title = InputHelper.ReadNonEmptyString("Введите название курса: ");
            int credits = InputHelper.ReadInt("Введите количество кредитов (1-30): ", 1, 30);
            int capacity = InputHelper.ReadInt("Введите максимальное количество мест (1-500): ", 1, 500);

            Teacher? selectedTeacher = null;
            if (_uni.Teachers.Any())
            {
                Console.WriteLine("\nЖелаете сразу назначить преподавателя на курс?");
                PrintBriefTeachersList();
                int teacherId = InputHelper.ReadInt("Введите ID преподавателя (или 0, чтобы пропустить): ", 0);
                if (teacherId != 0)
                {
                    selectedTeacher = _uni.FindTeacherById(teacherId);
                    if (selectedTeacher == null)
                        InputHelper.PrintError("Преподаватель с таким ID не найден. Курс будет создан без преподавателя.");
                }
            }

            var course = _uni.AddCourse(title, credits, capacity, selectedTeacher);
            InputHelper.PrintSuccess($"Курс «{course.Title}» успешно создан с ID {course.Id}.");
        }

        private static void ShowCourseDetailsFlow()
        {
            InputHelper.PrintHeader("Детальная информация о курсе");
            if (!EnsureCoursesExist()) return;

            PrintBriefCoursesList();
            int id = InputHelper.ReadInt("\nВведите ID курса: ", 1);
            var course = _uni.FindCourseById(id);

            if (course == null)
            {
                InputHelper.PrintError("Курс с указанным ID не найден.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine(course.GetDetailedInfo());
        }

        private static void ShowAllStudentsFlow()
        {
            InputHelper.PrintHeader("Список всех студентов университета");
            if (!EnsureStudentsExist()) return;

            Console.WriteLine($"{"ID",-5} | {"ФИО студента",-30} | {"Возраст",-7} | {"Курс",-6} | {"Дисциплин",-9} | {"Email"}");
            Console.WriteLine(new string('-', 90));

            foreach (var s in _uni.Students.OrderBy(st => st.StudyYear).ThenBy(st => st.Name))
            {
                Console.WriteLine($"{s.Id,-5} | {s.Name,-30} | {s.Age,-7} | {s.StudyYear,-6} | {s.EnrolledCourses.Count,-9} | {s.Email}");
            }
        }

        private static void ShowAllTeachersFlow()
        {
            InputHelper.PrintHeader("Список всех преподавателей университета");
            if (!EnsureTeachersExist()) return;

            Console.WriteLine($"{"ID",-5} | {"ФИО преподавателя",-30} | {"Звание",-22} | {"Кафедра",-24} | {"Курсов"}");
            Console.WriteLine(new string('-', 95));

            foreach (var t in _uni.Teachers.OrderBy(tc => tc.Name))
            {
                Console.WriteLine($"{t.Id,-5} | {t.Name,-30} | {Teacher.FormatDegree(t.Degree),-22} | {t.Department,-24} | {t.AssignedCourses.Count}");
            }
        }

        private static void ShowAllCoursesFlow()
        {
            InputHelper.PrintHeader("Список всех доступных курсов");
            if (!EnsureCoursesExist()) return;

            Console.WriteLine($"{"ID",-5} | {"Название дисциплины",-38} | {"Кред.",-5} | {"Места",-9} | {"Преподаватель"}");
            Console.WriteLine(new string('-', 95));

            foreach (var c in _uni.Courses.OrderBy(cr => cr.Id))
            {
                string instructor = c.Instructor != null ? c.Instructor.Name : "Не назначен";
                string places = $"{c.Students.Count}/{c.MaxCapacity}";
                Console.WriteLine($"{c.Id,-5} | {c.Title,-38} | {c.Credits,-5} | {places,-9} | {instructor}");
            }
        }

        private static void ShowPolymorphicRegistryFlow()
        {
            InputHelper.PrintHeader("Единый реестр всех людей (Полиморфный вызов Person)");
            foreach (Person person in _uni.AllPeople.OrderBy(p => p.Id))
            {
                // Работа со всеми объектами через базовый класс Person
                Console.WriteLine(person.GetDetailedInfo());
            }
        }

        private static bool EnsureStudentsExist()
        {
            if (!_uni.Students.Any())
            {
                InputHelper.PrintError("В системе пока нет ни одного студента.");
                return false;
            }
            return true;
        }

        private static bool EnsureTeachersExist()
        {
            if (!_uni.Teachers.Any())
            {
                InputHelper.PrintError("В системе пока нет ни одного преподавателя.");
                return false;
            }
            return true;
        }

        private static bool EnsureCoursesExist()
        {
            if (!_uni.Courses.Any())
            {
                InputHelper.PrintError("В системе пока нет ни одного курса.");
                return false;
            }
            return true;
        }

        private static void PrintBriefStudentsList()
        {
            Console.WriteLine("Доступные студенты:");
            foreach (var s in _uni.Students)
                Console.WriteLine($"  [ID: {s.Id}] {s.Name} ({s.StudyYear}-й курс)");
        }

        private static void PrintBriefTeachersList()
        {
            Console.WriteLine("Доступные преподаватели:");
            foreach (var t in _uni.Teachers)
                Console.WriteLine($"  [ID: {t.Id}] {t.Name} ({Teacher.FormatDegree(t.Degree)}, каф. {t.Department})");
        }

        private static void PrintBriefCoursesList()
        {
            Console.WriteLine("Доступные курсы:");
            foreach (var c in _uni.Courses)
            {
                string teacher = c.Instructor != null ? c.Instructor.Name : "Не назначен";
                Console.WriteLine($"  [ID: {c.Id}] {c.Title} (Мест: {c.Students.Count}/{c.MaxCapacity}, Преп.: {teacher})");
            }
        }
    }
}