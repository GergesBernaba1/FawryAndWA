

using Serilog;

Log.Logger = new LoggerConfiguration()
           .WriteTo.Console()
           .WriteTo.File("Logs/app-.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.AddSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();


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
