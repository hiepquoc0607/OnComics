using Appwrite;
using Appwrite.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnComics.API.Middleware;
using OnComics.Application.Constants;
using OnComics.Application.Helpers;
using OnComics.Application.Services.Implements;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Implements;
using OnComics.Infrastructure.Repositories.Interfaces;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configue Json Viewer
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
#endregion

#region DBContext
builder.Services.AddDbContext<OnComicsDatabaseContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection")!;

    options.UseMySql(connection, ServerVersion.AutoDetect(connection));
});
#endregion

#region Authentication UI For Swagger
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1"
    });

    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
#endregion

#region Authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Authentication:Jwt:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Jwt:Key"]!))
        };
    });
#endregion

#region Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole(RoleConstant.ADMIN));
    options.AddPolicy("User", policy => policy.RequireRole(RoleConstant.ADMIN, RoleConstant.USER));
});
#endregion

#region Inject Repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IComicRepository, ComicRepository>();
builder.Services.AddScoped<IComicCategoryRepository, ComicCategoryRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IInteractionTypeRepository, InteractionTypeRepository>();
builder.Services.AddScoped<IComicRatingRepository, ComicRatingRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IInteractionRepository, InteractionRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IChapterSourceRepository, ChapterSourceRepository>();
builder.Services.AddScoped<IAttachmentRepsitory, AttachmentRepository>();
#endregion

#region Inject Service
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<IComicService, ComicService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInteractionTypeService, InteractionTypeService>();
builder.Services.AddScoped<IComicRatingService, ComicRatingService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IInteractionService, InteractionService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<IChapterSourceService, ChapterSourceService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IAppwriteService, AppwriteService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
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

#region Inject SMTP
builder.Services
    .Configure<MailHelper>(builder.Configuration.GetSection("EmailSettings"));
#endregion

#region Inject Google
builder.Services
    .Configure<GoogleHelper>(builder.Configuration.GetSection("Authentication:Google"));
#endregion

#region Inject Appwrite
//Configure Appwrite
var appwriteClient = new Client()
    .SetEndpoint(builder.Configuration["Appwrite:Endpoint"]!)
    .SetProject(builder.Configuration["Appwrite:ProjectId"]!)
    .SetKey(builder.Configuration["Appwrite:ApiKey"]!)
    .SetSession(string.Empty);

//Register Appwrite Service 
builder.Services.AddSingleton(appwriteClient);
builder.Services.AddSingleton(sp => new Storage(appwriteClient));
builder.Services
    .Configure<AppwriteHelper>(builder.Configuration.GetSection("Appwrite"));
#endregion

#region Inject Redis
var host = builder.Configuration["Redis:Host"];
var port = builder.Configuration["Redis:Port"] ?? "6379";
var token = builder.Configuration["Redis:Token"];

var conf = new ConfigurationOptions
{
    AbortOnConnectFail = false,
    Ssl = true,
    ConnectTimeout = 10000,
};
conf.EndPoints.Add($"{host}:{port}");

if (!string.IsNullOrEmpty(token))
    conf.Password = token;

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(conf)
);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{host}:{port},token={token},ssl=True,abortConnect=False";
    options.InstanceName = "OnComics:";
});
#endregion

#region Logger
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});
#endregion

#region Model State Error Response
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(apiBehaviorOptions =>
    {
        apiBehaviorOptions.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new
            {
                status = "error",
                statusCode = "400",
                message = "Input Validation Erorr!",
                errors = errors
            };

            var result = new ContentResult
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentType = "application/json",
                Content = JsonSerializer.Serialize(response)
            };

            return result;
        };
    });
#endregion

#region Rate Limiting
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter(policyName: "BasePolicy", options =>
{
    options.PermitLimit = 10;
    options.Window = TimeSpan.FromSeconds(30);
    options.QueueLimit = 5;
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
}));
#endregion

#region HttpClientFactory
builder.Services.AddHttpClient();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    // Hides the "Schemas" section at the bottom
    c.DefaultModelsExpandDepth(-1);
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnComics");
});

app.UseMiddleware<ResponseMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
