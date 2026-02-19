using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;                 // PasswordHasher
using Microsoft.EntityFrameworkCore;

using Resort.Applications.Places;
using Resort.Domain.Bookings;
using Resort.Domain.Places;
using Resort.Domain.Users;                           // <-- add
using Resort.Infrastructure.Database;
using Resort.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC
builder.Services.AddControllersWithViews();

// Repos you already have
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<Resort.Domain.Places.IPlaceRepository, Resort.Infrastructure.Repositories.PlaceRepository>();
builder.Services.AddScoped<Resort.Applications.Places.IPlaceQueryService, Resort.Infrastructure.Repositories.PlaceQueryService>();

// ✅ Users repo + password hasher
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ✅ Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Welcome}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.Run();
