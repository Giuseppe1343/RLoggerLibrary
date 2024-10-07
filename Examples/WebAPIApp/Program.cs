using RLoggerLib;
using RLoggerLib.LoggingTargets;

namespace WebAPIApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Register the main thread
            RLogger.RegisterMainThread();

            // Create logger with adding some logging targets
            RLogger.Create((logger) =>
            {
                logger.AddDebugLogging()
                    .AddConsoleLogging()
                    .AddTextFileLogging(TextFileLoggingTargetOptions.Default)
                    .AddMailLogging(new MailLoggingTargetOptions()
                    {
                        MinRequiredSeverity = LogType.Critical, //Default is LogType.Error
                        MailServer = "smtp.gmail.com", // Example mail server
                        MailPort = 587, // google mail port
                        MailTo = new string[] { "RECEIVER1 ADDRESS", "RECEIVER2 ADDRESS" },
                        MailUser = "YOUR MAIL ADDRESS",
                        MailPassword = "YOUR MAIL PASSWORD",
                    })
                    // For Windows Event Logging, you need to run the application as an administrator on Windows.
                    .AddWindowsEventLogging(WindowsEventLoggingTargetOptions.Default);
            });

            // Add logging to the DI container as a singleton service
            builder.Services.AddSingleton<IRLogger, RLogger>((sp) => RLogger.Instance);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
