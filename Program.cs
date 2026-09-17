using System.Security.Claims;
using App5s.Components;
using App5s.Data;
using App5s.Services;
using App5s.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Isopoh.Cryptography.Argon2;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "App5s.Auth";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/acesso-negado";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<LogService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();


app.MapPost("/auth/login", async (
    HttpContext httpContext,
    [FromForm] string email,
    [FromForm] string senha,
    AuthService authService) =>
{
    var usuario = await authService.ValidarCredenciaisAsync(email, senha);

    if (usuario == null)
    {
        return Results.Redirect("/login?error=true");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new(ClaimTypes.Name, usuario.Nome),
        new(ClaimTypes.Email, usuario.Email),
        new(ClaimTypes.Role, usuario.Perfil)
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties
    {
        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
    };

    await httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

    if (usuario.Perfil.Equals("Admin", StringComparison.OrdinalIgnoreCase))
    {
        return Results.Redirect("/admin/usuarios");
    }

    return Results.Redirect("/auditorias");
}).DisableAntiforgery();

app.MapGet("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
