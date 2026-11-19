namespace WorkshopRegistrationApp.Models;

//main C# Object representing one successful registration
//and has fields suited to the workshop registration form
public class WorkshopRegistration
{
    //Each registration gets a unique identifier
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Workshop { get; set; } = string.Empty;
    public bool NeedsLaptop { get; set; }

    //Submission time in UTC
    public DateTimeOffset SubmittedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    //The JSON file reader can reconstruct saved objects
    public WorkshopRegistration() { }

    //Called from OnPostAsync() after the submitted values pass validation
    public WorkshopRegistration(string firstName, int age,
        string workshop, bool needsLaptop)
    {
        FirstName = firstName;
        Age = age;
        Workshop = workshop;
        NeedsLaptop = needsLaptop;
    }

    //Display object details as text
    public override string ToString()
    {
        string laptopText = NeedsLaptop ? "Yes" : "No";
        return $"Name: {FirstName}, Age: {Age}, Workshop: {Workshop}, " +
               $"Laptop needed: {laptopText}";
    }
}
