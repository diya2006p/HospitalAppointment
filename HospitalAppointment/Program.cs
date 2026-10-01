
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Memory caching
builder.Services.AddMemoryCache();

// Response caching
builder.Services.AddResponseCaching();

// Session configuration
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HTTP client for asynchronous requests
builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable caching and session
app.UseResponseCaching();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Doctors}/{action=Index}/{id?}");

app.Run();
