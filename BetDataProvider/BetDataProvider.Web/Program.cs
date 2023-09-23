using BetDataProvider.Business.Services;
using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess;
using BetDataProvider.DataAccess.Repositories;
using BetDataProvider.DataAccess.Repositories.Contracts;
using BetDataProvider.Web.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<RecurringSportService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<BetDataDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ISportRepository, SportRepository>();

builder.Services.AddScoped<ISportService, SportService>();
builder.Services.AddScoped<IXmlHandler, XmlHandler>();

// not working as expected
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();