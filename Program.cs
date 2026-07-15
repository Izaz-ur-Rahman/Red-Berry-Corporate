using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RedBerryCorporate.Configuration;
using RedBerryCorporate.Data;
using RedBerryCorporate.Helpers;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Interfaces.Blog;
using RedBerryCorporate.Interfaces.Sitemap;
using RedBerryCorporate.Middleware;
using RedBerryCorporate.Models;
using RedBerryCorporate.Repository;
using RedBerryCorporate.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Controllers

builder.Services.AddControllers();

#endregion

#region Database

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region Configuration

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

#endregion

#region Dependency Injection

// Contact
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();

// Blueprint
builder.Services.AddScoped<IBlueprintRepository, BlueprintRepository>();
builder.Services.AddScoped<IBlueprintService, BlueprintService>();

// Blog
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IBlogService, BlogService>();

// Sitemap
builder.Services.AddScoped<ISitemapGenerator, SitemapGenerator>();

// Email
builder.Services.AddScoped<IEmailService, EmailService>();

// JWT Helper
builder.Services.AddScoped<JwtHelper>();

// login
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
// user 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<FileHelper>();
builder.Services.AddHostedService<ScheduledBlogPublisher>();
#endregion

#region JWT Authentication

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RedBerry Corporate API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Token like: Bearer {your token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:8080",
                "https://localhost:8080",
                "https://redberrycorporate.ittcomcentre.net",
                "https://redberrycorporatecms.ittcomcentre.net",
                "http://redberrycorporate.ittcomcentre.net",
                "http://redberrycorporatecms.ittcomcentre.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("ReactPolicy", policy =>
//    {
//        policy
//            .AllowAnyOrigin()
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

#endregion

var app = builder.Build();

#region Middleware Pipeline

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RedBerry Corporate API v1");
    options.RoutePrefix = "swagger";
});
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("ReactPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();