using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Library.Model.Data;
using OnComics.Library.Utils.Utils;
using OnComics.Repository.Implement;
using OnComics.Repository.Interface;
using OnComics.Service.Implement;
using OnComics.Service.Interface;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configue Json Viewer
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.DefaultBufferSize = 32 * 1024;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });

#region DBContext
builder.Services.AddDbContext<OnComicsDatabaseContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!);
});
#endregion

#region Inject Repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
#endregion

#region Inject Service
builder.Services.AddScoped<IAccountService, AccountService>();
#endregion

#region Inject Utils
builder.Services.AddTransient<Util>();
#endregion

#region Inject Mapster
//Configure Mapster
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(AppDomain.CurrentDomain.GetAssemblies());

//Register Mapster Service 
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnComics"));
}

app.UseSwagger();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnComics"));

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
