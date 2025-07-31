using HD.Station.Home.Mvc.Features.MediaFiles.Controllers; // Để hỗ trợ AddApplicationPart nếu cần
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Gọi các extension method đã tách riêng cho từng layer
builder.Services
    .AddMvcFeature(builder.Configuration)     // từ HD.Station.Home.Mvc.DependencyInjection
    .UseSqlServer(builder.Configuration);     // từ HD.Station.Home.SqlServer.DependencyInjection

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/MediaFileView/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ✅ Routing mặc định cho controller có View
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MediaFileView}/{action=Index}/{id?}");

app.Run();
