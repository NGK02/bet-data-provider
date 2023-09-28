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

builder.Services.AddHostedService<RecurringSportService>();

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContextPool<BetDataDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ISportRepository, SportRepository>();

builder.Services.AddScoped<IMatchRepository, MatchRepository>();

builder.Services.AddScoped<IMatchService, MatchService>();

builder.Services.AddScoped<IExternalDataService, ExternalDataService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

//var mapperConfig = new MapperConfiguration(mc =>
//{
//    mc.AddProfile(new AutoMapperProfile());
//});
//IMapper mapper = mapperConfig.CreateMapper();
//builder.Services.AddSingleton(mapper);

// not working as expected
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();