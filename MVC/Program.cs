using APP.Authentication.Services;
using APP.Domain;
using APP.Models;
using APP.Services;
using CORE.Authentication.Services;
using CORE.Services;
using CORE.Session.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// Create a builder for the web application and initialize
// configuration, logging, etc.
var builder = WebApplication.CreateBuilder(args);



// --------------------------------------------------------
// Add services to the IoC (Inversion of Control) container
// for Dependency Injections.
// --------------------------------------------------------
// Register the application's DbContext dependency
// injection by sending Db instances to the injected service
// class constructors' DbContext or Db parameter, using SQLite
// with the connection string from appsettings.json.
builder.Services.AddDbContext<DbContext, Db>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString(nameof(Db))));
// nameof(Db) = "Db" which is the class name.

// Register the UserService dependency injection by sending
// IService<UserRequest, UserResponse> instance reference to
// the injected controller class constructor's
// IService<UserRequest, UserResponse> parameter.
// Injection through abstract class or interface is suitable
// for SOLID Principles.
builder.Services
    .AddScoped<IService<UserRequest, UserResponse>, UserService>();

// Register the RoleService dependency injection by sending
// IService<RoleRequest, RoleResponse> instance reference to
// the injected controller class constructor's
// IService<RoleRequest, RoleResponse> parameter.
builder.Services
    .AddScoped<IService<RoleRequest, RoleResponse>, RoleService>();

// Register the StatusService dependency injection by sending
// IService<StatusRequest, StatusResponse> instance reference to
// the injected controller class constructor's
// IService<StatusRequest, StatusResponse> parameter.
builder.Services
    .AddScoped<IService<StatusRequest, StatusResponse>, StatusService>();



// --------------------------
// Authentication and Session
// --------------------------
// Register IHttpContextAccessor as a singleton service.
// Enables access to the current HttpContext from classes other than
// controller classes such as services.
// Required for services like CookieAuthService that need to read
// or modify HTTP context (e.g., authentication, user info).
// Allows constructor injection of IHttpContextAccessor throughout
// the services of the application.
builder.Services.AddHttpContextAccessor();



// --------------
// Authentication
// --------------
// Register CookieAuthService as a scoped dependency for ICookieAuthService.
// Scoped lifetime ensures a new instance per HTTP request, which is
// required for services accessing HttpContext.
// CookieAuthService handles user authentication using cookies, supporting
// sign in and sign out operations.
// This service registration enables constructor injection of
// ICookieAuthService to the controllers or services throughout the application.
builder.Services.AddScoped<ICookieAuthService, CookieAuthService>();



// --------------
// Authentication
// --------------
// Register AuthService as a scoped dependency for IAuthService.
// Scoped lifetime ensures a new instance per HTTP request, which is
// required for services accessing HttpContext.
// AuthService handles user authentication using injected ICookieAuthService
// instance, supporting log in, log out and register operations.
// This service registration enables constructor injection of
// IAuthService to the controllers or services throughout the application.
builder.Services.AddScoped<IAuthService, AuthService>();



// --------------
// Authentication
// --------------
// Configure authentication services using cookie-based authentication.
// - Sets the default authentication scheme to Cookies.
// - Specifies options for cookie authentication:
//   - LoginPath: Path to redirect unauthenticated users
//     (when authentication is required).
//   - AccessDeniedPath: Path to redirect users who are authenticated
//     but lack required permissions.
//   - ExpireTimeSpan: Duration before the authentication cookie expires
//     (here, 1 hour).
//   - SlidingExpiration: Resets the expiration time on each request
//     to keep active sessions alive.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; 
        // changed from /Auth/Login to /Login since route will be changed for action
        options.AccessDeniedPath = "/Login"; 
        // changed from /Auth/Login to /Login since route will be changed for action
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });



// -------
// Session
// -------
// Register session services and configure session options.
// Sets the session idle timeout to 30 minutes (default 20 minutes);
// if no activity occurs within this period, the session expires.
// Enables storing user-specific data (e.g., shopping cart, temporary users)
// across multiple requests during the session lifetime.
// Required for features that rely on session state, such as
// UserSessionService and SessionService.
builder.Services.AddSession(config =>
{
    config.IdleTimeout = TimeSpan.FromMinutes(30);
});



// -------
// Session
// -------
// Register the concrete SessionService as a scoped dependency.
// Concrete classes can also be used for registration, but using an
// abstract base class or interface allows for more flexible
// dependency injection and testing (Also better for SOLID Principles).
// Since session operations are standard and are not expected to change,
// concrete class injection is implemented.
// Scoped lifetime ensures a new instance per HTTP request,
// which is required for services accessing HttpContext.
// SessionService handles session management.
// This service registration enables constructor injection of SessionService
// to the controllers or services throughout the application.
builder.Services.AddScoped<SessionService>();



// -------
// Session
// -------
// Register the concrete UserSessionService as a scoped dependency.
// Scoped lifetime ensures a new instance per HTTP request.
// UserSessionService handles temporary users management.
// This service registration enables constructor injection of UserSessionService
// to the controllers or services throughout the application.
builder.Services.AddScoped<UserSessionService>();



// Add support for controllers and views (MVC pattern).
builder.Services.AddControllersWithViews();

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Use a custom error handler in non-development environments
    // redirecting to Home controller's Error action.
    app.UseExceptionHandler("/Home/Error");

    // Enable HTTP Strict Transport Security (HSTS).
    // The default HSTS value is 30 days. You may want to change this
    // for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable serving static files (e.g., HTML, CSS, JS, images).
app.UseStaticFiles();

// Enable routing capabilities.
app.UseRouting();



// --------------
// Authentication
// --------------
// Enable authentication middleware so that [Authorize] works.
app.UseAuthentication();



// Enable authorization middleware.
app.UseAuthorization();



// -------
// Session
// -------
// Enable session middleware in the HTTP request pipeline.
app.UseSession();



// Configure the default route for controllers for sending requests
// to controllers' actions as
// controller/action/id where id is optional.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start the application and listen for incoming HTTP requests.
app.Run();
