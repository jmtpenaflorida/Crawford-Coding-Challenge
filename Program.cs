using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IFileProcessor, FileProcessor>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<IFileValidator, FileValidator>();

builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });
    
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World inside docker!!!").RequireAuthorization();

app.MapPost("/applicants", async ([FromForm] IFormCollection files, IApplicantService service, IFileValidator validator) =>
{
    var results = new List<object>();

    foreach (var file in files.Files)
    {
        if (!validator.IsValid(file))
        {
            return Results.BadRequest(
                $"File '{file.FileName}' is not a JSON file.");
        }

        results.AddRange(await service.ProcessApplicants(file));
    }

    return Results.Ok(new {
        FilesProcessed = results.Count,
        results
    });
})
.RequireAuthorization()
.DisableAntiforgery();

app.Run();

public interface IFileValidator
{
    bool IsValid(IFormFile file);
}

public class FileValidator : IFileValidator
{
    public bool IsValid(IFormFile file)
    {
        return file.ContentType == "application/json";
    }
}