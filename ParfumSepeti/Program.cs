using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Data;
using ParfumSepeti.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


// dbcontext
var connectionString = builder.Configuration.GetConnectionString("local");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


// identity
builder.Services.AddIdentity<Kullanici, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


// seed
using (var scope = app.Services.CreateScope())
{
    // inject services
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<Kullanici>>();

    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // seed role
    var role = await roleManager.FindByNameAsync("admin");

    if (role == null)
    {
        var result = await roleManager.CreateAsync(new IdentityRole("admin"));

        if (!result.Succeeded)
            throw new Exception(string.Join('\n',
                                result.Errors.Select(e => e.Description)));
    }

    // seed admin
    var admin = await userManager.FindByNameAsync("admin");

    if (admin == null)
    {
        admin = new Kullanici
        {
            UserName = "admin",
            Email = "admin@parfumsepeti.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        var password = config["password"] ??
            throw new Exception("Put 'password' into environment variables!");

        var result = await userManager.CreateAsync(admin, password);

        if (!result.Succeeded)
            throw new Exception(string.Join('\n',
                                result.Errors.Select(e => e.Description)));
    }

    if (!await userManager.IsInRoleAsync(admin, "admin"))
    {
        var result = await userManager.AddToRoleAsync(admin, "admin");

        if (!result.Succeeded)
            throw new Exception(string.Join('\n',
                                result.Errors.Select(e => e.Description)));
    }
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
