using ComputerTypingWebApp.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<dbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContextPool<dbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddSession((options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(45);
}));
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAllHeaders", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseStaticFiles();
var provider = new FileExtensionContentTypeProvider();
//provider.Mappings.Add(".exe", "application/octect-stream");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "/wwwroot",
    ContentTypeProvider = provider
});

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");



app.Run();


//StaticFileOptions GetStaticFileOptions()
//{
//    var p = new FileExtensionContentTypeProvider();
//    p.Mappings[".exe"] = "application/octet-stream";
//    return new StaticFileOptions { ContentTypeProvider = p };
//}