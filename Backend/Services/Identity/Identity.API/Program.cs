using Identity.API.Data;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Auth;
using Shared.Common.Constants;
using Shared.Common.Swagger;

var builder = WebApplication.CreateBuilder(args);

// ── Database (InMemory — replace with SQL Server/PostgreSQL in production) ────
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("IdentityDb"));

// ── ASP.NET Core Identity ─────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.Password.RequiredLength = 6;
    o.Password.RequireNonAlphanumeric = false;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    o.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── JWT & Authorization ───────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();

// ── Swagger + Controllers ─────────────────────────────────────────────────────
builder.Services.AddSwaggerWithJwt("Identity API");
builder.Services.AddControllers();

var app = builder.Build();

// ── Seed roles and default admin user ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in new[] { Roles.User, Roles.Admin, Roles.Manager })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    const string adminEmail = "admin@flightbooking.com";
    if (await userMgr.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail, Email = adminEmail,
            FirstName = "System", LastName = "Admin", EmailConfirmed = true
        };
        await userMgr.CreateAsync(admin, "Admin@123456");
        await userMgr.AddToRoleAsync(admin, Roles.Admin);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
