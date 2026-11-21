using WorkshopRegistrationApp.Services;

//Create the ASP.NET Core application builder
var builder = WebApplication.CreateBuilder(args);

//Add support for Razor Pages
builder.Services.AddRazorPages();

//Register one file service for the application. Pages can ask ASP.NET
//to give them this service through their constructors
builder.Services.AddSingleton<RegistrationFileService>();

//Build the web application
var app = builder.Build();

//Show an error page outside development mode
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//Encrypts information in transit redirecting HTTP to HTTPS
app.UseHttpsRedirection();

//Allow the browser to load files in wwwroot,CSS
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

//Match incoming URLs to Razor Pages including Index and Registrations
app.MapRazorPages();

//Start the web server
app.Run();
