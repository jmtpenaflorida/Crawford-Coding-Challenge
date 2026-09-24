using System.Text;
using System.Text.Json;
using Moq;
using Xunit;

public class ApplicantServiceTests
{
    [Fact]
    public async Task ProcessApplicants_ReturnsOnlyApplicantsWithResume()
    {
        // Arrange
        var applicants = new List<Applicant>
        {
            new() { Name = "John", Email = "john@example.com", HasResume = true },
            new() { Name = "Jane", Email = "jane@example.com", HasResume = false }
        };

        var mockFileProcessor = new Mock<IFileProcessor>();
        mockFileProcessor.Setup(fp => fp.ProcessFile(It.IsAny<IFormFile>()))
            .ReturnsAsync(applicants);

        var fileProcessor = new FileProcessor();
        var applicantService = new ApplicantService(mockFileProcessor.Object);

        var formFile = CreateFormFile("applicants.json", JsonSerializer.Serialize(applicants));

        // Act
        var result = await applicantService.ProcessApplicants(formFile);

        // Assert
        Assert.Equal("applicants.json", result.FileName);
        Assert.True(result.FilteredApplicants.All(a => a.HasResume));
        Assert.DoesNotContain(result.FilteredApplicants, a => !a.HasResume);
    }

    [Fact]
    public async Task ProcessFile_ReturnsApplicantsFromJsonFile()
    {
        // Arrange
        var applicants = new List<Applicant>
        {
            new() { Name = "John", Email = "john@example.com", HasResume = true },
            new() { Name = "Jane", Email = "jane@example.com", HasResume = false }
        };

        var formFile = CreateFormFile("applicants.json", JsonSerializer.Serialize(applicants));

        var processor = new FileProcessor();

        // Act
        var result = await processor.ProcessFile(formFile);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal("John", result[0].Name);
        Assert.Equal("john@example.com", result[0].Email);
        Assert.True(result[0].HasResume);

        Assert.Equal("Jane", result[1].Name);
        Assert.Equal("jane@example.com", result[1].Email);
        Assert.False(result[1].HasResume);
    }

    private IFormFile CreateFormFile(string fileName, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "files", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/json"
        };
    }
}