using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SkillSnap.Client;
using SkillSnap.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient pointed at the API
builder.Services.AddScoped(_ =>
    new HttpClient { BaseAddress = new Uri("https://localhost:7000/") });

// Application services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<UserSessionService>();

await builder.Build().RunAsync();
