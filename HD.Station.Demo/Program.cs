using HD.Station.Home.Mvc.Features.MediaFiles.Controllers; // ✅ dùng đúng controller chứa View
using HD.Station.Home.Mvc.Features.Service;
using HD.Station.Home.SqlServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<MediaService>();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(MediaFileViewController).Assembly) // ✅ controller dùng View
    .AddRazorRuntimeCompilation()
    .AddRazorOptions(options =>
    {
        // Xoá mặc định và cấu hình lại View search path theo Feature Folder
        options.ViewLocationFormats.Add("/Features/MediaFiles/Views/MediaFileView/{0}.cshtml");
        options.ViewLocationFormats.Add("/Features/MediaFiles/Views/Shared/{0}.cshtml");
        options.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");  // Features/Shared
    });

    // Đăng ký DbContext
    builder.Services.AddDbContext<HomeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký module phụ nếu có
builder.Services
    .AddHomeFeature(builder.Configuration)
    .UseSqlServer(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/MediaFileView/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Định tuyến mặc định gọi đúng controller MVC có View
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MediaFileView}/{action=Index}/{id?}");

app.Run();
