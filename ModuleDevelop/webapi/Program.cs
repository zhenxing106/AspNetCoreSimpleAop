
using ModuleLib;

namespace webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var moduleManager = new ModuleManager();
            moduleManager.LoadModules(builder.Services);
            var app = builder.Build();
            moduleManager.Configures(app);
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

        public static void RestartApplication()
        {
            Console.WriteLine("Restarting application...");
            Environment.ExitCode = 0;
            System.Diagnostics.Process.Start(Environment.GetCommandLineArgs()[0]);
            Environment.Exit(Environment.ExitCode);
        }
    }
}
