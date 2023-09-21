using BetDataProvider.Business.Services;
using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess;
using BetDataProvider.DataAccess.Repositories;
using BetDataProvider.DataAccess.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BetDataDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ISportRepository, SportRepository>();

builder.Services.AddScoped<ISportService, SportService>();

var app = builder.Build();

app.Run();
