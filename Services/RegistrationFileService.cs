using System.Text.Json;
using WorkshopRegistrationApp.Models;

namespace WorkshopRegistrationApp.Services;

//Saving and reading workshop registrations from a text file
public class RegistrationFileService
{
    private readonly string _filePath;

    //Prevent two requests in this one running application from writing at the same time 
    //and accidentally mixing lines
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public RegistrationFileService(IWebHostEnvironment environment)
    {
        //ContentRootPath is the application's project folder in VS
        string dataFolder = Path.Combine(environment.ContentRootPath, "App_Data");

        //Create App_Data if it does not yet exist
        Directory.CreateDirectory(dataFolder);
        _filePath = Path.Combine(dataFolder, "registrations.jsonl");
    }

    //Keeping the old registrations and adding a new line
    public async Task SaveAsync(WorkshopRegistration registration)
    {
        string oneLine = JsonSerializer.Serialize(registration);

        await _fileLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_filePath,
                oneLine + Environment.NewLine);
        }
        finally
        {
            //Release the lock even if the disk write fails
            _fileLock.Release();
        }
    }

    //Converting saved JSON text back into C# objects
    public async Task<IReadOnlyList<WorkshopRegistration>> ReadAllAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
            {
                //New application has no saved registrations yet
                return Array.Empty<WorkshopRegistration>();
            }

            string[] lines = await File.ReadAllLinesAsync(_filePath);
            var registrations = new List<WorkshopRegistration>();

            foreach (string line in lines)
            {
                //Ignore blank lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                //Each valid line represents one previously saved object
                WorkshopRegistration? item =
                    JsonSerializer.Deserialize<WorkshopRegistration>(line);

                if (item is not null)
                    registrations.Add(item);
            }

            //Show newest submissions at the top of the history page
            return registrations
                .OrderByDescending(item => item.SubmittedAtUtc)
                .ToList();
        }
        finally
        {
            _fileLock.Release();
        }
    }
}
