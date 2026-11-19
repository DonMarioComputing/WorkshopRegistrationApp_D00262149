using System.ComponentModel.DataAnnotations;

namespace WorkshopRegistrationApp.Models;

//Represents the values that the visitor typea into the HTML form.
//Data annotations are checked by ASP.NET core on theserver during POST.
//HTML also provides validation in the browser but browser checks alone are not enough because users can bypass them.
public class RegistrationFormInput
{
    [Display(Name = "First name")]
    [Required(ErrorMessage = "Please enter your first name.")]
    [StringLength(40, MinimumLength = 2,
        ErrorMessage = "Your first name must be between 2 and 40 characters.")]
    [RegularExpression(@"^[\p{L}][\p{L} '\-]*$",
        ErrorMessage = "Use letters, spaces, apostrophes or hyphens only.")]
    public string? FirstName { get; set; }

    //int? allows an empty form value to be detected by
    //The 16-110 age range is an application rule chosen for this example
    [Display(Name = "Age")]
    [Required(ErrorMessage = "Please enter your age.")]
    [Range(16, 110, ErrorMessage = "Age must be between 16 and 110.")]
    public int? Age { get; set; }

    [Display(Name = "Workshop")]
    [Required(ErrorMessage = "Please choose a workshop.")]
    public string? Workshop { get; set; }

    //Checkbox gives TRUE when selected and FALSE when not selected
    [Display(Name = "I require a laptop")]
    public bool NeedsLaptop { get; set; }
}
