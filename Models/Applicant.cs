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

