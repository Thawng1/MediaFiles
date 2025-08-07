using HD.Station.Home.Abstraction.DependencyInjection;
using HD.Station.Home.SqlServer.Data;
using HD.Station.Home.SqlServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<HomeDbContext>()
    .AddDefaultTokenProviders();


builder.Services
    .AddMvcFeature(builder.Configuration)
    .AddAbstractionFeature(builder.Configuration)
    .UseSqlServer(builder.Configuration);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    string email = "admin@example.com";
    string password = "Admin@123456";

    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        var newUser = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newUser, password);
        if (result.Succeeded)
        {
            Console.WriteLine("✅ Tạo user admin thành công.");
        }
        else
        {
            Console.WriteLine("❌ Lỗi khi tạo user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("⚠️ User đã tồn tại.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/MediaFileView/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MediaFileView}/{action=Index}/{id?}");

app.Run();
