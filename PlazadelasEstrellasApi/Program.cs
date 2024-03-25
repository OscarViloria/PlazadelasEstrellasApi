
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 string _MyCors = "MyCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _MyCors,
                   builder =>
                   {
                       builder.WithOrigins("https://plazadelasestrellas.sissamx.com.mx/");
                       builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "plazadelasestrellas.sissamx.com.mx")
                                .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                       builder.WithOrigins("http://localhost:*/");
                       builder.WithOrigins("http://192.168.15.61/");
                       builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                                  .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                   });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


string connectionString = builder.Configuration.GetConnectionString("PlazaEstrellasDB");

builder.Services.AddDbContext<PlazadelasEstrellasApi.Models.PlazaEstrellasDBContext>(opt =>
           opt.UseMySql(connectionString, ServerVersion.Parse("11.4.0-mariadb"))
           // The following three options help with debugging, but should
           // be changed or removed for production.
           //.LogTo(Console.WriteLine, LogLevel.Information)
           //.EnableSensitiveDataLogging()
           //.EnableDetailedErrors()
           );
builder.Services.AddControllers();

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
