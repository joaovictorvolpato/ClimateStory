using ClimateStory.Models;
using ClimateStory.Services;
using Microsoft.EntityFrameworkCore;
using ClimateStory.Models;
using ClimateStory.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the UserService for Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddHttpClient<FecthData>();

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