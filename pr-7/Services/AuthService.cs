using System.Security.Cryptography;
using Marketplace.Data;
using Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Services;

// Регистрация и вход пользователя
public class AuthService
{
    private readonly MarketplaceContext _context;

    public AuthService(MarketplaceContext context)
    {
        _context = context;
    }

    // Регистрирует пользователя. Возвращает null и текст ошибки, если не вышло
    public (User? User, string? Error) Register(string nickname, string password, string passwordConfirm)
    {
        nickname = nickname.Trim();

        if (nickname.Length < 3)
            return (null, "Ник должен содержать минимум 3 символа.");

        if (password.Length < 4)
            return (null, "Пароль должен содержать минимум 4 символа.");

        // Подтверждение пароля: второй ввод должен совпасть с первым
        if (password != passwordConfirm)
            return (null, "Пароли не совпадают.");

        if (_context.User.Any(user => user.Nickname == nickname))
            return (null, "Пользователь с таким ником уже существует.");

        string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        var newUser = new User
        {
            Nickname = nickname,
            PasswordSalt = salt,
            PasswordHash = HashPassword(password, salt),
            CreatedAt = DateTime.Now
        };

        _context.User.Add(newUser);
        _context.SaveChanges();

        return (newUser, null);
    }

    // Вход в уже существующий аккаунт. Возвращает null при неверных данных
    public User? Login(string nickname, string password)
    {
        User? user = _context.User.FirstOrDefault(u => u.Nickname == nickname.Trim());

        if (user == null)
            return null;

        string hash = HashPassword(password, user.PasswordSalt);
        return hash == user.PasswordHash ? user : null;
    }

    // Хеширование пароля алгоритмом PBKDF2 с солью
    private static string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password, saltBytes, 100_000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(hash);
    }
}
