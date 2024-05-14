using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using YYBagProgram.Data;
using YYBagProgram.Service;
using YYBagProgram.Service.Interface;
using YYBagProgram.Models.Mail;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<YYBagProgramContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YYBagProgramContext") ?? throw new InvalidOperationException("Connection string 'YYBagProgramContext' not found.")));


// Add services to the container.
builder.Services.AddControllersWithViews();

//用戶驗證操作機制註冊DI(在controller範圍外使用方式)
builder.Services.AddHttpContextAccessor();

//Sessions=====
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// 添加HttpContextAccessor
builder.Services.AddHttpContextAccessor();


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//自訂用戶登入資訊操作註冊DI
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ICryptographyService, MemberService>();
builder.Services.AddScoped<ISendEmailService, MemberService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ICryptographyService, CryptographyService>();


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
app.UseSession();


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
    name: "CheckMember",
    pattern: "{controller=Member}/{action=CheckMember}/{account}/{password}");

app.MapControllerRoute(
    name: "UpdateMember",
    pattern: "{controller=Member}/{action=UpdateMember}");

app.MapControllerRoute(
    name: "AddtoCart",
    pattern: "{controller=Cart}/{action=AddtoCart}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=home}/{id?}");

app.Run();
