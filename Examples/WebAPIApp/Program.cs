
using Microsoft.Extensions.Logging;
using RLoggerLib;
using RLoggerLib.LoggingTargets;

namespace WebAPIApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            RLogger.RegisterMainThread();
            RLogger.Create(LogDatabaseCreationOptions.Default, (logger) =>
            {
                logger.AddDebugLogging()
                    .AddConsoleLogging()
                    .AddTextFileLogging(new TextFileLoggingTargetOptions()
                    {
                        FileNamingConvention = LogFileNamingConvention.CustomDate,
                        CustomName = "WebAPI",
                        DateFormat = "yyyy-MM-dd",
                    });
            });
            // Add services to the container.
            builder.Services.AddSingleton<IRLogger,RLogger>((sp) => RLogger.Instance);
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
