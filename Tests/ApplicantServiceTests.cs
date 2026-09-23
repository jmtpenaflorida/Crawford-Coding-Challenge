using Xunit;

public class ApplicantServiceTests
{
    [Fact]
    public void HasResumeApplicants_ReturnsOnlyApplicantsWithResume()
    {
        // Arrange
        var applicants = new List<Applicant>
        {
            new Applicant { Name = "John Doe", Email = "john.doe@example.com", HasResume = true },
            new Applicant { Name = "Jane Smith", Email = "jane.smith@example.com", HasResume = false },
            new Applicant { Name = "Alice Johnson", Email = "alice.johnson@example.com", HasResume = true }
        };

        var service = new ApplicantService(applicants);

        var result = service.HasResumeApplicants;

        Assert.True(result.All(a => a.HasResume));
        Assert.DoesNotContain(result, a => !a.HasResume);
    }
}