using Microsoft.Extensions.Caching.Memory;
using MyWeatherHub;
using MyWeatherHub.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.AddServiceDefaults();


builder.Services.AddHttpClient<NwsManager>(c =>
{

    c.BaseAddress = new("https+http://nwsapi");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<MyWeatherHub.Components.App>().AddInteractiveServerRenderMode();

app.Run();
