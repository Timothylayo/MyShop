using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyShop.Client.Services.AuthenticationService;
using MyShop.Client.Services.ProductAdminServices;
using MyShop.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7036") });

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IClientService, ClientService>()
                .AddScoped<IAdminService, AdminService>()
                .AddScoped<AuthenticationServices>();

await builder.Build().RunAsync();
