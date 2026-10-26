namespace Lesson23;

public class User
{
    public string FullName { get; set; }
    public string Login { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<string> Hobbies { get; set; }

    public User(string fullName, string login, DateTime dateOfBirth, List<string> hobbies)
    {
        FullName = fullName;
        Login = login;
        DateOfBirth = dateOfBirth;
        Hobbies = hobbies;
    }

    public int GetAge(DateTime today)
    {
        int age = today.Year - DateOfBirth.Year;

        if (DateOfBirth.Date > today.Date.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
