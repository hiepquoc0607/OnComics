using Microsoft.EntityFrameworkCore;
using OnComics.Library.Model.Data;
using OnComics.Repository.Implement;
using OnComics.Repository.Interface;

var builder = WebApplication.CreateBuilder(args);

#region DBContext
builder.Services.AddDbContext<OnComicsDatabaseContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!);
});
#endregion

// Add services to the container.

#region Inject Repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
#endregion

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
