using System.Security.Claims;
using App5s.Components;
using App5s.Data;
using App5s.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "App5s.Auth";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/acesso-negado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthService>();

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


app.MapPost("/api/auth/login", async (
    [FromForm] string email,
    [FromForm] string senha,
    [FromForm] string? returnUrl,
    [FromServices] AuthService authService,
    HttpContext httpContext) =>
{
    var usuario = await authService.ValidarCredenciaisAsync(email, senha);
    if (usuario is null)
    {
        var redirectErro = string.IsNullOrEmpty(returnUrl) 
            ? "/login?error=true" 
            : $"/login?error=true&returnUrl={Uri.EscapeDataString(returnUrl)}";
        return Results.Redirect(redirectErro);
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

    string destino = !string.IsNullOrWhiteSpace(returnUrl) && returnUrl.StartsWith('/') 
        ? returnUrl 
        : "/";

    return Results.Redirect(destino);
}).DisableAntiforgery();

app.MapGet("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

app.MapGet("/api/debug", async (
    [FromServices] IDbContextFactory<AppDbContext> contextFactory,
    [FromServices] AuthService authService) =>
{
    var diagnostico = new Dictionary<string, object>();

    try
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        bool conectou = await context.Database.CanConnectAsync();
        diagnostico["1_PostgresConectado"] = conectou;

        var admin = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@app5s.local");
        
        if (admin is null)
        {
            diagnostico["2_UsuarioAdmin"] = "NÃO ENCONTRADO no banco app5s_db";
            diagnostico["3_Argon2Teste"] = "Não executado (usuário ausente)";
        }
        else
        {
            diagnostico["2_UsuarioAdmin"] = new
            {
                admin.Id,
                admin.Nome,
                admin.Email,
                admin.Perfil,
                admin.Ativo,
                HashArmazenado = admin.SenhaHash
            };

            
            bool senhaValida = Isopoh.Cryptography.Argon2.Argon2.Verify(admin.SenhaHash, "admin123");
            diagnostico["3_Argon2Teste_SenhaValida"] = senhaValida;
            
            if (!senhaValida)
            {
                
                diagnostico["3_SugestaoNovoHashArgon2"] = authService.GerarHashSenha("admin123");
            }
        }
    }
    catch (Exception ex)
    {
        diagnostico["ERRO_EXCECAO"] = ex.Message;
        if (ex.InnerException is not null)
            diagnostico["INNER_EXCEPTION"] = ex.InnerException.Message;
    }

    return Results.Json(diagnostico, options: new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
});


_ = Task.Run(async () =>
{
    await Task.Delay(1500); // Aguarda o servidor subir
    using var scope = app.Services.CreateScope();
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    try
    {
        await using var db = await factory.CreateDbContextAsync();
        bool canConnect = await db.Database.CanConnectAsync();
        Console.ForegroundColor = canConnect ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"[DEBUG] PostgreSQL Conexão: {(canConnect ? "OK" : "FALHOU")}");

        var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@app5s.local");
        if (user != null)
        {
            bool match = Isopoh.Cryptography.Argon2.Argon2.Verify(user.SenhaHash, "admin123");
            Console.WriteLine($"[DEBUG] Usuário Admin: ENCONTRADO (Id: {user.Id}, Ativo: {user.Ativo})");
            Console.WriteLine($"[DEBUG] Argon2 'admin123': {(match ? "SENHA VÁLIDA (OK)" : "HASH INVÁLIDO")}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[DEBUG] Usuário 'admin@app5s.local' NÃO EXISTE na tabela 'usuarios'.");
        }
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[DEBUG ERRO] Falha ao conectar: {ex.Message}");
        Console.ResetColor();
    }
});