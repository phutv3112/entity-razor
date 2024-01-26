using System.Configuration;
using Microsoft.EntityFrameworkCore;
using razorweb.models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetValue<string>("ConnectString:BlogContext");
builder.Services.AddDbContext<BlogContext>(options =>
{
    // string connectString = builder.Configuration.GetConnectionString("ConnectString:BlogContext");
    Console.WriteLine("Connecting to " + connectionString);
    options.UseSqlServer(connectionString);
});

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
