using Colards;
using Colards.Helpers;
using Colards.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(AuthCookie.Name)
    .AddCookie(
            AuthCookie.Name,
            (options) =>
            {
                options.Cookie.Name = AuthCookie.Name;
                options.LoginPath = "/SignIn";
                options.AccessDeniedPath = "/Error";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            }
        );

builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserNameBasedIdProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization();

app.MapRazorPages();
app.MapHub<GameHub>("/Dealer");

app.Run();
