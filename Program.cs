using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });
    
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World inside docker!!!").RequireAuthorization();

app.MapPost("/applicants", async ([FromForm] IFormCollection files) =>
{
    var results = new List<object>();

    foreach (var file in files.Files)
    {
        var stopwatch = Stopwatch.StartNew();

        using var stream = file.OpenReadStream();

        var applicants =
            await JsonSerializer.DeserializeAsync<List<Applicant>>(stream)
            ?? [];

        var filteredApplicants = new ApplicantService(applicants).HasResumeApplicants;

        stopwatch.Stop();

        results.AddRange(new
        {
            file.FileName,
            stopwatch.ElapsedMilliseconds,
            filteredApplicants
        });
    }

    return Results.Ok(new {
        FilesProcessed = results.Count,
        results
    });
})
.RequireAuthorization()
.DisableAntiforgery();

app.Run();