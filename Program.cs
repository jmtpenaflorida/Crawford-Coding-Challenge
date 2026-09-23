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

app.MapPost("/applicants", async ([FromForm] IFormFile file) =>
{
    using var stream = file.OpenReadStream();

    var applicants =
        await JsonSerializer.DeserializeAsync<List<Applicant>>(stream);

    return Results.Ok(new ApplicantService(applicants).HasResumeApplicants);
})
.RequireAuthorization()
.DisableAntiforgery();

app.Run();