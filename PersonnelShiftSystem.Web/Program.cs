using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PersonnelShiftSystem.Application.Exceptions;
using PersonnelShiftSystem.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc();
builder.Services.AddScoped<PersonnelShiftSystem.Domain.Interfaces.IBaseModel, BaseModel>();
builder.Services.AddScoped<PersonnelShiftSystem.Domain.Interfaces.IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<PersonnelContext>();
//builder.Services.AddDbContext<PersonnelContext>(options =>
//    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: options =>
//    {
//        options.EnableRetryOnFailure();
//    }));


builder.Services.AddScoped(typeof(PersonnelShiftSystem.Domain.Interfaces.IRepository<>), typeof(Repository<>));
builder.Services.AddAutoMapper(x => x.AddProfile<PersonnelShiftSystem.Application.MapperProfile.AutoMapperProfile>(), typeof(Program));
builder.Services.AddCors();
//builder.Services.AddMvc().AddRazorPagesOptions(options => options.Conventions.AddPageRoute("/Login/Index", ""));
builder.Services.AddDetection();
builder.Services.AddAntiforgery(x => x.HeaderName = "XSRF-TOKEN");
builder.Services.AddScoped<PersonnelShiftSystem.Application.Services.LoginService>();
builder.Services.AddScoped<ExceptionHandlers>();

builder.Services.AddSession(options =>
{
    //Configure session options
    options.Cookie.Name = "App.Sessions";
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //IsPersistent �zelli�i true olarak ayarlan�rsa, kullan�c�n�n oturumu taray�c�y� kapatsa bile devam eder
        options.SlidingExpiration = false;//oturum s�resi taray�c� kapat�ld���nda sonlan�r ve oturum kapat�lma s�resi ertelenmez
        options.Cookie.Name = "App";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);//oturum s�resini s�n�rlar
        options.AccessDeniedPath = "/AccessDenied";
        options.LoginPath = "/Index"; // Kimlik do�rulama ba�ar�s�z oldu�unda y�nlendirme yap�lacak sayfa
        options.LogoutPath = "/Index";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.HttpOnly = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = context =>
            {
                // Cookie s�resi bitti�inde otomatik ��k�� yap
                if (context.Properties.ExpiresUtc.HasValue && context.Properties.ExpiresUtc < DateTime.UtcNow)
                {
                    context.HttpContext.Session.Clear();//cookie s�resi tamamlan�nca session'� da sonland�r ve temizle!
                    context.Response.Cookies.Delete("App.Sessions");//cookie s�resi tamamlan�nca cookie taray�c�dan temizlenir!
                    context.Response.Cookies.Delete("App");
                    context.RejectPrincipal();
                    context.ShouldRenew = true;
                }
                return Task.CompletedTask;
            }
        };
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseCookiePolicy();

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403)
    {
        context.Response.Redirect("/AccessDenied");
    }
});

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();