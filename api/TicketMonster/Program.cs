using System.Text;
using MassTransit;
using MassTransit.SqlTransport.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TicketMonster.Consumers;
using TicketMonster.Domain;
using TicketMonster.Repositories;
using TicketMonster.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Manager", Version = "v1" });
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT desta forma: Bearer {seu token}"
            };
            c.AddSecurityDefinition("Bearer", securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });
        });
builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? "Secret Key não encontrada")
                )
            };
        });

builder.Services.AddOptions<SqlTransportOptions>()
    .Configure(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("DefaultConnection");
        options.Schema = "masstransit";
    });

builder.Services.AddPostgresMigrationHostedService(x =>
{
    x.CreateDatabase = false;
    x.CreateSchema = true;
    x.CreateInfrastructure = true;
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StripeWebhookConsumer>();

    x.UsingPostgres((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
        cfg.AutoStart = true; 
    });
});

builder.Services.AddAuthorizationBuilder();
builder.Services.AddIdentityCore<UserEntity>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ShowService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
        builder
            .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5125")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
});

var app = builder.Build();

app.UseCors("AllowReactApp");

app.UseSwagger();
app.UseSwaggerUI(); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
