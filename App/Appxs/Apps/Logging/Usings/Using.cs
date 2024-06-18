using Microsoft.Extensions.Configuration;

using Serilog;

namespace App.Appxs.Apps.Logging.Usings;

public class LogParameter
{
    public string Name { get; set; }
    public object Value { get; set; }
    public string Type { get; set; }
}

public class LogDetail
{
    public string FullName { get; set; }
    public string MethodName { get; set; }
    public string User { get; set; }
    public List<LogParameter> Parameters { get; set; }
}

public class LogDetailWithException : LogDetail
{
    public string ExceptionMessage { get; set; }
}

public class FileLogConfiguration
{
    public string FolderPath { get; set; }
}

public static class SerilogMessages
{
    public static string NullOptionsMessage =>
        "You have sent a blank value! Something went wrong. Please try again.";
}

public class FileLogger : ILogger
{
    private IConfiguration _configuration;

    public FileLogger(IConfiguration configuration)
    {
        _configuration = configuration;

        FileLogConfiguration logConfig = configuration.GetSection("SeriLogConfigurations:FileLogConfiguration")
                                                      .Get<FileLogConfiguration>() ??
                                         throw new Exception(SerilogMessages.NullOptionsMessage);

        string logFilePath = string.Format("{0}{1}", Directory.GetCurrentDirectory() + logConfig.FolderPath, ".txt");

        Logger = new LoggerConfiguration()
                 .WriteTo.File(
                     logFilePath,
                     rollingInterval: RollingInterval.Day,
                     retainedFileCountLimit: null,
                     fileSizeLimitBytes: 5000000,
                     outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                 .CreateLogger();
    }
}

public abstract class ILogger
{
    protected Serilog.ILogger Logger { get; set; }

    public void Verbose(string message) => Logger.Verbose(message);
    public void Fatal(string message) => Logger.Fatal(message);
    public void Info(string message) => Logger.Information(message);
    public void Warn(string message) => Logger.Warning(message);
    public void Debug(string message) => Logger.Debug(message);
    public void Error(string message) => Logger.Error(message);
}