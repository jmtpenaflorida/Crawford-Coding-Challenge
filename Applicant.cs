public class Applicant
{
    public string Name { get; set; }
    public string Email { get; set; }
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