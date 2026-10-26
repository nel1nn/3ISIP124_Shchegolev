namespace Lesson37;

public static class FileService
{
    public static async Task WriteAsync(string path, string text)
    {
        await File.WriteAllTextAsync(path, text);
    }

    public static async Task<string> ReadAsync(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл не найден.");
        }

        return await File.ReadAllTextAsync(path);
    }
}
