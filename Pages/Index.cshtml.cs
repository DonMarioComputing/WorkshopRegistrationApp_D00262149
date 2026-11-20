using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkshopRegistrationApp.Models;
using WorkshopRegistrationApp.Services;

namespace WorkshopRegistrationApp.Pages;

//Page model for Index.cshtml and handles GET and POST requests
public class IndexModel : PageModel
{
    private readonly RegistrationFileService _registrationFileService;

    //Only workshop choices the application will accept
    //Checking them on the server prevents someone from submitting a fake choice
    private static readonly HashSet<string> AllowedWorkshops = new(StringComparer.Ordinal)
    {
        "Web Development",
        "C# Fundamentals",
        "Database Basics",
        "Cloud Foundation",
        "Database Administration",
        "Algorithms"
    };

    //ASP.NET provides the file service registered in Program.cs
    public IndexModel(RegistrationFileService registrationFileService)
    {
        _registrationFileService = registrationFileService;
    }

    //[BindProperty] connects the HTML form fields to this C# object:
    [BindProperty]
    public RegistrationFormInput Input { get; set; } = new();

    //Set only after a successful save, the .cshtml page then shows it
    public WorkshopRegistration? SavedRegistration { get; private set; }

    //HTTP GET: show the empty registration form
    public void OnGet() { }

    //HTTP POST: runs when a visitor clicks Submit registration
    public async Task<IActionResult> OnPostAsync()
    {
        //Remove extra spaces from user entered text
        Input.FirstName = Input.FirstName?.Trim();
        Input.Workshop = Input.Workshop?.Trim();

        //Reject workshop values not found in the dropdown list
        //DataAnnotations already check name, age and required fields
        if (!string.IsNullOrEmpty(Input.Workshop) &&
            !AllowedWorkshops.Contains(Input.Workshop))
        {
            ModelState.AddModelError("Input.Workshop",
                "Please select one of the available workshops.");
        }

        //If any input is invalid show the SAME page with errors
        //Nothing is saved when ModelState.IsValid is false
        if (!ModelState.IsValid)
        {
            return Page();
        }

        //Make a object using the submitted form values
        //! is safe after successful validation: name or workshop can't be null, and Age has a value
        //because [Required] checked it.
        var registration = new WorkshopRegistration(
            Input.FirstName!,
            Input.Age!.Value,
            Input.Workshop!,
            Input.NeedsLaptop);

        //APPEND the new object to the file, never overwrite old ones.
        try
        {
            await _registrationFileService.SaveAsync(registration);
        }
        catch (IOException)
        {
            ModelState.AddModelError(string.Empty,
                "Your registration could not be saved. Please try again.");
            return Page();
        }
        catch (UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty,
                "The application cannot write to the registration file.");
            return Page();
        }

        //Make the saved details available to the confirmation view
        SavedRegistration = registration;

        //Clear the form so the next participant can register
        ModelState.Clear();
        Input = new RegistrationFormInput();

        return Page();
    }
}
