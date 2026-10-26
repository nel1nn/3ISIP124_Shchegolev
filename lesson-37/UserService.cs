using Microsoft.EntityFrameworkCore;

namespace Lesson37;

public class UserService
{
    private readonly UserContext _context;

    public UserService(UserContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.User.OrderBy(user => user.Id).ToListAsync();
    }

    public async Task<User> AddAsync(string fullName, int age)
    {
        User newUser = new User
        {
            FullName = fullName,
            Age = age
        };

        await _context.User.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }
}
