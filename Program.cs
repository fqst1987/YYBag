using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Policy;
using YYBagProgram.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<YYBagProgramContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YYBagProgramContext") ?? throw new InvalidOperationException("Connection string 'YYBagProgramContext' not found.")));


// Add services to the container.
builder.Services.AddControllersWithViews();

//用戶驗證操作機制註冊DI(在controller範圍外使用方式)
builder.Services.AddHttpContextAccessor();

//自訂用戶登入資訊操作註冊DI
//builder.Services.AddScoped<>();

//全域範圍的驗證機制組態設定(全環境cookie套用)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    //未登入時會自動轉到此網頁
    option.LoginPath = new PathString("/Member/NoLogin");

    //未授權腳色時會自動移轉到此網頁
    option.AccessDeniedPath = new PathString("/Home/NoRole");

    //登入後60分鐘後會失效
    option.ExpireTimeSpan = TimeSpan.FromHours(1);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//用戶登入驗證操作機制使用
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "GetData",
    pattern: "{controller=Products}/{action=GetData}/{strID}/{strColor}");

app.MapControllerRoute(
    name: "GetDataRemain",
    pattern: "{controller=Products}/{action=GetDataRemain}/{strID}/{strColor}/{ProductStatus}");

app.MapControllerRoute(
    name: "GetImgUrl",
    pattern: "{controller=CarouselSetting}/{action=GetImgUrl}/{strBagsId}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=homepage}");

app.Run();
