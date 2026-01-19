//using July_Team.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using System.Globalization;
//using Microsoft.AspNetCore.Localization;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using System.Threading.Tasks; 

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//#region connectDB
//// 🛑 تم تعديل GetConnectionString لتتوافق مع الاسم الذي استخدمتهِ في كودكِ
//builder.Services.AddDbContext<AppDbContext>(option =>
//option.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
//#endregion

//#region Languages
//builder.Services.AddLocalization(option => option.ResourcesPath = "Resourses");
//builder.Services.Configure<RequestLocalizationOptions>(option =>
//{
//    var supported = new[] {
//        new CultureInfo("en"),
//        new CultureInfo("ar")

//};
//    option.DefaultRequestCulture = new RequestCulture("en");
//    option.SupportedCultures = supported;
//    option.SupportedUICultures = supported;
//});
//#endregion

//#region Identity
//builder.Services.AddIdentity<IdentityUser, IdentityRole>(

//    Option =>
//    {
//        Option.Password.RequiredLength = 8;
//        Option.Password.RequireNonAlphanumeric = true;
//        Option.Password.RequireUppercase = true;
//        //Option.Password.RequireLowercase = true;
//        Option.User.RequireUniqueEmail = true;
//    }

//    )
//    // 🛑 تأكدي من وجود هذا لتمكين الصلاحيات (Roles)
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>();

//builder.Services.AddSession();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddMemoryCache();

//builder.Services.ConfigureApplicationCookie(
//    option =>
//    {
//        option.AccessDeniedPath = "/User/AccessDenied";
//        option.Cookie.Name = "Cookie";
//        option.Cookie.HttpOnly = true;
//        option.ExpireTimeSpan = TimeSpan.FromMinutes(28);
//        option.LoginPath = "/User/Login";
//        option.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

//    }
//    );
//#endregion

//builder.Services.AddHttpContextAccessor();

//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//var app = builder.Build();


//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

//    // إنشاء الأدوار (Admin, Trainer, Member)
//    string[] roleNames = { "Admin", "Trainer", "Member" };
//    foreach (var roleName in roleNames)
//    {
//        if (!await roleManager.RoleExistsAsync(roleName))
//        {
//            await roleManager.CreateAsync(new IdentityRole(roleName));
//        }
//    }

//    // إنشاء مستخدم Admin افتراضي
//    var adminUser = new IdentityUser { UserName = "admin@kitspace.com", Email = "admin@kitspace.com", EmailConfirmed = true };
//    var user = await userManager.FindByEmailAsync(adminUser.Email);

//    if (user == null)
//    {
//        var createAdmin = await userManager.CreateAsync(adminUser, "AdminP@ss123"); // ⚠️ غيري كلمة المرور!
//        if (createAdmin.Succeeded)
//        {
//            await userManager.AddToRoleAsync(adminUser, "Admin");
//        }
//    }
//}
//// ----------------------------------------------------

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseSession();
//app.UseRouting();

//// 🛑 ترتيب الـ Middleware هنا صحيح جداً
////app.UseAuthentication();

////app.UseAuthorization();


//var localizationoption = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
//app.UseRequestLocalization(localizationoption!.Value);

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();


using July_Team.Models;
using July_Team.Services;
using July_Team.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region connectDB
// Use PostgreSQL for Replit environment
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = databaseUrl;
if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var database = uri.AbsolutePath.TrimStart('/');
    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
    var sslMode = query["sslmode"] ?? "disable";
    var npgsqlSslMode = sslMode == "disable" ? "Disable" : "Require";
    connectionString = $"Host={host};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode={npgsqlSslMode}";
}
builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseNpgsql(connectionString));
#endregion

#region Languages
builder.Services.AddLocalization(option => option.ResourcesPath = "Resourses");
builder.Services.Configure<RequestLocalizationOptions>(option =>
{
    var supported = new[] {
        new CultureInfo("en"),
        new CultureInfo("ar")

};
    option.DefaultRequestCulture = new RequestCulture("en");
    option.SupportedCultures = supported;
    option.SupportedUICultures = supported;
});
#endregion

#region Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(

    Option =>
    {
        Option.Password.RequiredLength = 8;
        Option.Password.RequireNonAlphanumeric = true;
        Option.Password.RequireUppercase = true;
        //Option.Password.RequireLowercase = true;
        Option.User.RequireUniqueEmail = true;
    }

    )
    // 🛑 تأكدي من وجود هذا لتمكين الصلاحيات (Roles)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.ConfigureApplicationCookie(
    option =>
    {
        option.AccessDeniedPath = "/User/AccessDenied";
        option.Cookie.Name = "Cookie";
        option.Cookie.HttpOnly = true;
        option.ExpireTimeSpan = TimeSpan.FromMinutes(28);
        option.LoginPath = "/User/Login";
        option.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

    }
    );
#endregion

builder.Services.AddHttpContextAccessor();
// الكود المعدل
builder.Services.AddSession(options =>
{
    // 1. زيادة مدة مهلة الخمول (اختياري ولكن موصى به)
    options.IdleTimeout = TimeSpan.FromDays(14); // اجعل الجلسة نشطة لمدة 14 يومًا طالما هناك تفاعل

    // 2. جعل كوكى الجلسة مستمرة (هذه هي الخطوة الأهم)
    // هذا السطر يخبر المتصفح بالاحتفاظ بالكوكى حتى بعد إغلاقه
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;

    // 🛑 أضيفي هذا السطر فقط 🛑
    options.Cookie.MaxAge = TimeSpan.FromDays(14);
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "July Team API", Version = "v1" });
});

#region Services Registration
// Register generic repository for dependency injection
// This allows services to request IRepository<T> and receive Repository<T>
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register application services
// Scoped lifetime means one instance per HTTP request
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "July Team API V1");
        c.RoutePrefix = "swagger";
    });
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // إنشاء الأدوار (Admin, Trainer, Member)
    string[] roleNames = { "Admin", "Trainer", "Member" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // إنشاء مستخدم Admin افتراضي
    var adminUser = new IdentityUser { UserName = "admin@kitspace.com", Email = "admin@kitspace.com", EmailConfirmed = true };
    var user = await userManager.FindByEmailAsync(adminUser.Email);

    if (user == null)
    {
        var createAdmin = await userManager.CreateAsync(adminUser, "AdminP@ss123"); // ⚠️ غيري كلمة المرور!
        if (createAdmin.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
// ----------------------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

// 🛑 ترتيب الـ Middleware هنا صحيح جداً
app.UseAuthentication();

app.UseAuthorization();


var localizationoption = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationoption!.Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();