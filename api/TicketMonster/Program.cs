using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketMonster.Domain;
using TicketMonster.Repositories;
using TicketMonster.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorizationBuilder();
builder.Services.AddIdentityCore<UserEntity>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ShowService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); 
app.UseHttpsRedirection();
app.MapIdentityApi<UserEntity>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
