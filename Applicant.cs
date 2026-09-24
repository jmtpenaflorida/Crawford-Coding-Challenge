using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Applicant
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("hasResume")]
    public bool HasResume { get; set; }
}

public class ApplicantResult
{
    public string FileName { get; set; }
    public long ProcessingTime { get; set; }
    public List<Applicant> FilteredApplicants { get; set; }
}


public interface IApplicantService
{
    Task<ApplicantResult> ProcessApplicants(IFormFile file);
}

public class ApplicantService : IApplicantService
{
    private readonly IFileProcessor _fileProcessor;

    public ApplicantService(IFileProcessor fileProcessor)
    {
        _fileProcessor = fileProcessor;
    }
    
    public async Task<ApplicantResult> ProcessApplicants(IFormFile file)
    {
        var results = new List<ApplicantResult>();

        var stopwatch = Stopwatch.StartNew();
        
        var applicants = await _fileProcessor.ProcessFile(file);

        var filteredApplicants = applicants.Where(a => a.HasResume).ToList();

        stopwatch.Stop();

        return new ApplicantResult
        {
            FileName = file.FileName,
            ProcessingTime = stopwatch.ElapsedMilliseconds,
            FilteredApplicants = filteredApplicants
        };
    }
}

public interface IFileProcessor
{
    Task<List<Applicant>> ProcessFile(IFormFile file);
}

public class FileProcessor : IFileProcessor
{
    public async Task<List<Applicant>> ProcessFile(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var applicants =
            await JsonSerializer.DeserializeAsync<List<Applicant>>(stream)
            ?? [];

        return applicants;
    }
}