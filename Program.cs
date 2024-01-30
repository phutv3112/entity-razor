using System.Configuration;
using App.Security.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using razorweb.models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetValue<string>("ConnectString:BlogContext");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // string connectString = builder.Configuration.GetConnectionString("ConnectString:BlogContext");
    Console.WriteLine("Connecting to " + connectionString);
    options.UseSqlServer(connectionString);
});
// subscribe Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
// builder.Services.AddDefaultIdentity<AppUser>()
//                 .AddEntityFrameworkStores<BlogContext>()
//                 .AddDefaultTokenProviders();
// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedAccount = true;    // Xác thực số điện thoại

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
});
builder.Services.AddOptions();
var mailsettings = builder.Configuration.GetSection("MailSettings");  // đọc config
builder.Services.Configure<MailSettings>(mailsettings);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

builder.Services.AddAuthentication()
.AddGoogle(options =>
{
    var ggconfig = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = ggconfig["ClientId"];
    options.ClientSecret = ggconfig["ClientSecret"];
    options.CallbackPath = "/gg-login";
}).AddFacebook(options =>
{
    var fbconfig = builder.Configuration.GetSection("Authentication:Facebook");
    options.AppId = fbconfig["AppId"];
    options.AppSecret = fbconfig["AppSecret"];
    options.CallbackPath = "/fb-login";
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowEditRole", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        // policyBuilder.RequireRole("Admin");
        // policyBuilder.RequireRole("Editor");
        policyBuilder.RequireClaim("allow.del", "user", "admin");
    });
    options.AddPolicy("InGenZ", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new GenZRequirement());
    });
    options.AddPolicy("ShowAdminMenu", pb =>
    {
        pb.RequireRole("Admin");
    });
    options.AddPolicy("CanUpdateArticle", pb =>
    {
        pb.Requirements.Add(new ArticleRequirement());
    });
});
builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
