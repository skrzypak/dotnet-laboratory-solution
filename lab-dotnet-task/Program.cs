using lab_dotnet_task;
using lab_dotnet_task.Interfaces;
using lab_dotnet_task.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dodanie kontekstu bazy danych
builder.Services.AddDbContext<DatabaseContext>();

// HttpContext - mozna wszczykiwac do serwisów
builder.Services.AddScoped<IHeaderContextService, HeaderContextService>();
builder.Services.AddHttpContextAccessor();

// TODO: Dodaj schemat uwierzytelnienia i autoryzacji
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            // Sposob aby zwrocic kod 401 (Unauthorized) zamiast 404 (NotFound)
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.Headers["Location"] = context.RedirectUri;
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

// Rejestracja serwisow
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddSingleton<IWebSocketService, WebSocketService>();

// TODO: W razie potrzeby dodaj i zarejestruj kolejne serwisy

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// TODO: Dodanie parametow ciastek aby nie dalo sie m.in pobrac ich za pomoca skryptow JS
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
});

// Middleware - wylapuje brak autoryzacji (401) i zwraca json
#region 401 Middleware
app.Use(async (context, next) =>
{
    await next();
    switch (context.Response.StatusCode)
    {
        case (int)HttpStatusCode.Unauthorized:
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"loggedin\":false}");
            break;
    }
});
#endregion

// TODO: Dodaj middleware odpowiedzialne za obsluge uwierzytelnienia i autoryzacji
app.UseAuthentication();
app.UseAuthorization();

// TODO: Dodaj obsluge plikow statycznych
app.UseDefaultFiles();
app.UseStaticFiles();

// TODO: Dodaj obslugue polaczen ws
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(3600)
});


app.MapControllers();

app.Run();
