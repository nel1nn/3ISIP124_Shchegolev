namespace Lesson36;

public static class FileManager
{
    public static string DefaultFolder =>
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public static string Read(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл не найден.");
        }

        using (StreamReader reader = new StreamReader(path))
        {
            return reader.ReadToEnd();
        }
    }

    public static string Create(string folder, string fileName, string content)
    {
        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, EnsureExtension(fileName));

        if (File.Exists(path))
        {
            throw new IOException("Файл с таким именем уже существует.");
        }

        File.WriteAllText(path, content);
        return Path.GetFullPath(path);
    }

    public static void Append(string path, string text)
    {
        using (StreamWriter writer = new StreamWriter(path, append: true))
        {
            writer.WriteLine(text);
        }
    }

    public static void Overwrite(string path, string text)
    {
        File.WriteAllText(path, text);
    }

    public static string Rename(string path, string newName)
    {
        string folder = Path.GetDirectoryName(Path.GetFullPath(path))!;
        string newPath = Path.Combine(folder, EnsureExtension(newName, Path.GetExtension(path)));

        if (File.Exists(newPath))
        {
            throw new IOException("Файл с таким именем уже существует.");
        }

        File.Move(path, newPath);
        return newPath;
    }

    public static string Move(string path, string newFolder)
    {
        Directory.CreateDirectory(newFolder);

        string newPath = Path.Combine(Path.GetFullPath(newFolder), Path.GetFileName(path));

        if (File.Exists(newPath))
        {
            throw new IOException("В выбранной папке уже есть файл с таким именем.");
        }

        File.Move(path, newPath);
        return newPath;
    }

    private static string EnsureExtension(string fileName, string defaultExtension = ".txt")
    {
        fileName = fileName.Trim();

        if (Path.HasExtension(fileName))
        {
            return fileName;
        }

        return fileName + (string.IsNullOrEmpty(defaultExtension) ? ".txt" : defaultExtension);
    }
}
