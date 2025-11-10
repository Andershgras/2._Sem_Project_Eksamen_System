using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.EFservices;

namespace _2._Sem_Project_Eksamen_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<EksamensDBContext>();
            // Muliggør dependency injection for ICRUD interface, hvor funktionaliteten referers til EFEksamenService
            builder.Services.AddTransient<ICRUD<Exam>, EFEksamenService>();
            builder.Services.AddScoped<ICRUD<Room>, EFRoomService>();
            builder.Services.AddScoped<ICRUDAsync<Student>, EFStudentService>();
            builder.Services.AddScoped<ICRUD<Class>, EFHoldService>();
            builder.Services.AddScoped<ICRUD<TeachersToExam>, EFTeachersToExamService>();
            builder.Services.AddScoped<ICRUD<Teacher>, EFUnderviserService>();
            builder.Services.AddScoped<ICRUDAsync<Teacher>, EFUnderviserService>();
            builder.Services.AddScoped<IStudentsToExams, EFStudentsToExamService>();
            builder.Services.AddScoped<IRoomsToExams, EFRoomsToExamService>();

            // In Program.cs or Startup.cs
            builder.Services.AddScoped<ICRUDAsync<Student>, EFStudentService>();
           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
