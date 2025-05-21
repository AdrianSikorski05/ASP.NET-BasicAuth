using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestFullApiTest;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// === Serilog ===
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting up the application...");
    // === Configuration ===
    var configuration = builder.Configuration;

    // === DI ===
    builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
    builder.Services.AddScoped<IBookRepository, BookRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    //builder.Services.AddScoped<IBookService, BookService>();
    //builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<JwtService>();


    //// === Basic Auth ===
    //builder.Services.AddAuthentication("Basic")
    //    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


    // === JWT Auth ===
    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    //// === Swagger  Basic auth===
    //builder.Services.AddSwaggerGen(c =>
    //{
    //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestFullApiTest", Version = "v1" });

    //    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    //    {
    //        Name = "Authorization",
    //        Type = SecuritySchemeType.Http,
    //        Scheme = "basic",
    //        In = ParameterLocation.Header,
    //        Description = "Wpisz login i has³o (Basic Auth)"
    //    });

    //    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //    {
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "basic"
    //            }
    //        },
    //        new string[] {}
    //    }
    //    });
    //});

    // === MediaR ===
    builder.Services.AddMediatR(typeof(CreateBookCommand).Assembly);

    // === Swagger JWT auth ===
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Book API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Podaj token JWT: Bearer {token}",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = "Bearer",
            Type = SecuritySchemeType.Http
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    //  tu dodaj rejestracjê zamkniêcia aplikacji
    AppDomain.CurrentDomain.ProcessExit += (_, __) =>
    {
        Log.Information("Shutting down the application via ProcessExit...");
        Log.CloseAndFlush();
    };

    app.Run();

}
catch (Exception ex)
{
    Log.Error($"B³¹d w us³udze ogólny: {ex.Message}");
}

