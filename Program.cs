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
    var results = new List<Applicant>();

    foreach (var file in files.Files)
    {
        using var stream = file.OpenReadStream();

        var applicants =
            await JsonSerializer.DeserializeAsync<List<Applicant>>(stream)
            ?? [];

        results.AddRange(new ApplicantService(applicants).HasResumeApplicants);
    }

    return Results.Ok(results);
})
.RequireAuthorization()
.DisableAntiforgery();

app.Run();