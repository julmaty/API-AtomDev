using API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5001");

var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
bool isDevelopment = string.Equals(
           "Development",
            envName,
            StringComparison.OrdinalIgnoreCase);

//if (isDevelopment)
//{
//builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
//}
//else
//{
builder.Services.AddDbContext<ApplicationContext>(options => options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 36))));
//}


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");
//app.MapGet("/", (ApplicationContext db) => db.Users.ToList());

app.Run();

