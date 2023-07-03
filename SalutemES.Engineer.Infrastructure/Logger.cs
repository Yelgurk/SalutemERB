using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Infrastructure;

public class Logger
{
    private static string FileName => "log.txt";
    public static string? FilePath { private get; set; }
    private static string? FileFullPath { get => FilePath is null ? null : $"{FilePath}\\{FileName}"; }

    public static Logger SetLoggerPath(string FilePath)
    {
        Logger.FilePath = FilePath;
        return new Logger();
    }

    public static Logger? CheckLoggerFileExists()
    {
        if (!File.Exists(FileFullPath!))
            File.Create(FileFullPath!);

        return File.Exists(FileFullPath) ? new Logger() : null;
    }

    public static Logger? WriteLine(string message) => CheckLoggerFileExists()?.Write(message);

    private Logger Write(string message)
    {
        using (StreamWriter writer = new StreamWriter(FileFullPath!, true))
        {
            writer.WriteLine($"[{DateTime.Now.ToShortDateString()} | {DateTime.Now.ToLongTimeString()}]: {message};");
            writer.Close();
            writer.Dispose();
        }

        return this;
    }
}
