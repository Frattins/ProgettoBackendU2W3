using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProgettoBackendU2W3.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// stringa di connessione utilizzata (il ! serve a esprimere l'esigenza di esistenza del valore)
var conn = builder.Configuration.GetConnectionString("SqlServer")!;

// Autenticazione e autorizzazione
builder.Services
    .AddAuthentication(opt => {
        opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(opt =>
        opt.LoginPath = "/Account/Login"
    )
    ;
builder.Services
    // configurazione della DI per il contesto di persistenza dei dati con SQL Server
    .AddDbContext<DataContext>(opt => opt.UseSqlServer(conn))
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();