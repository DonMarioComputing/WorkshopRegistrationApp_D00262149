using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkshopRegistrationApp.Models;
using WorkshopRegistrationApp.Services;

namespace WorkshopRegistrationApp.Pages;

//Handles Registrations in a browser
public class RegistrationsModel : PageModel
{
    private readonly RegistrationFileService _registrationFileService;

    public RegistrationsModel(RegistrationFileService registrationFileService)
    {
        _registrationFileService = registrationFileService;
    }

    //Start with an empty list, fill it after reading the file
    public IReadOnlyList<WorkshopRegistration> Registrations { get; private set; }
        = Array.Empty<WorkshopRegistration>();

    //A message if the saved file cannot be opened or parsed
    public string? LoadError { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            //Convert each JSON line in the saved file into a C# object
            Registrations = await _registrationFileService.ReadAllAsync();
        }
        catch (IOException)
        {
            LoadError = "Could not read the registration file. Please try again.";
        }
        catch (UnauthorizedAccessException)
        {
            LoadError = "The application does not have permission to read the file.";
        }
        catch (JsonException)
        {
            LoadError = "The saved registration file contains invalid data.";
        }
    }
}
