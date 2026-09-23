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

public class ApplicantService
{
    private readonly List<Applicant> _applicants;

    public ApplicantService(List<Applicant> applicants)
    {
        _applicants = applicants;
    }

    public List<Applicant> HasResumeApplicants
    {
        get { return _applicants.Where(a => a.HasResume).ToList(); }
    }
}