
using AuthorizationAPI.Repository;
using AuthorizationAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration["ConnectionStrings:DatabaseConnection"];
// Add services to the container.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Models.Users.AedsystemContext>(option => option.UseSqlServer(conn), ServiceLifetime.Singleton);
builder.Services.AddDbContextFactory<Models.ChargedHours.AedsystemContext>(option => option.UseSqlServer(conn));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher, EncryptPassword>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalHost");

app.UseAuthorization();

app.MapControllers();

app.Run();
