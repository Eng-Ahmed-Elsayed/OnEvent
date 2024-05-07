using System.Text.Json.Serialization;
using DataAccess.Data;
using DataAccess.UnitOfWork.Classes;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using OnEvent.MappingProfiles;
using Utility.Communication.Mail;
using Utility.Communication.MailTemplates;
using Utility.FileManager;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Sort helper
builder.Services.AddScoped(typeof(ISortHelper<>), typeof(SortHelper<>));
// UoW
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Email service
//var emailConfig = builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
var emailConfig = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddTransient<IEmailService, SmtpEmailService>();
builder.Services.AddSingleton<IMailTemplate, MailTemplate>();


// File manager service
builder.Services.AddScoped<IFileManagerService, FileManagerService>();

// Auto mapper
builder.Services.AddAutoMapper(typeof(GeneralProfile));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    //Preserve references and handle circular references
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;

});
;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
            name: "areaRoute",
            pattern: "{area:exists}/{controller=Events}/{action=Index}/{id?}");
// Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();
