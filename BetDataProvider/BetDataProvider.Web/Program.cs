using BetDataProvider.Business.Services;
using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess;
using BetDataProvider.DataAccess.Repositories;
using BetDataProvider.DataAccess.Repositories.Contracts;
using BetDataProvider.Web.AutoMapperProfiles;
using BetDataProvider.Web.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddDbContextPool<BetDataDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});

builder.Services.AddScoped<ISportRepository, SportRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IExternalDataService, ExternalDataService>();
builder.Services.AddScoped<IXmlHandler, XmlHandler>();
builder.Services.AddHostedService<RecurringSportService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();