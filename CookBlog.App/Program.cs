using Blazored.LocalStorage;
using CookBlog.App;
using CookBlog.App.Data;
using CookBlog.App.Handlers;
using CookBlog.App.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiUri = new Uri(builder.Configuration["ApiUrl"]!);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddTransient<ValidateHeaderHandler>();
builder.Services.AddHttpClient<ICategoryDataService, CategoryDataService>(ConfigureClient)
    .AddHttpMessageHandler<ValidateHeaderHandler>();
builder.Services.AddHttpClient<ITagDataService, TagDataService>(ConfigureClient)
    .AddHttpMessageHandler<ValidateHeaderHandler>();
builder.Services.AddHttpClient<IPostDataService, PostDataService>(ConfigureClient)
    .AddHttpMessageHandler<ValidateHeaderHandler>();
builder.Services.AddHttpClient<IUserDataService, UserDataService>(ConfigureClient)
    .AddHttpMessageHandler<ValidateHeaderHandler>();
builder.Services.AddHttpClient<ICommentDataService, CommentDataService>(ConfigureClient)
    .AddHttpMessageHandler<ValidateHeaderHandler>();

await builder.Build().RunAsync();

void ConfigureClient(HttpClient client) 
    => client.BaseAddress = apiUri;
return;

