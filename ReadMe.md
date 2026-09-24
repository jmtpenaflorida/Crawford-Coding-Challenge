# Secure File API

A simple ASP.NET Core Minimal API that accepts JSON applicant files, validates the uploaded files, processes the applicant data, and returns applicants that have a resume.

## Requirements

- Docker
- .NET 10 SDK (only required if running outside Docker)

## Build and Run with Docker

From the project root, where the `Dockerfile` is located:

### Build the Docker image

    docker build -t secure-file-api .

### Run the API

    docker run --rm -p 8080:8080 secure-file-api

The API will be available at:

    http://localhost:8080

## API Authentication

The API requires an API key.

Include the following HTTP header in requests:

    X-API-Key: my-secret-key

## Process Applicant Files

### Endpoint

    POST /applicants

The endpoint accepts one or more JSON files using `multipart/form-data`.

### Example JSON File

Create a file named `applicants.json`:

    [
      {
        "name": "John",
        "email": "john@example.com",
        "hasResume": true
      },
      {
        "name": "Jane",
        "email": "jane@example.com",
        "hasResume": false
      }
    ]

### Upload a Single File

    curl -i \
      -H "X-API-Key: my-secret-key" \
      -F "files=@applicants.json" \
      http://localhost:8080/applicants

### Upload Multiple Files

    curl -i \
      -H "X-API-Key: my-secret-key" \
      -F "files=@applicants1.json" \
      -F "files=@applicants2.json" \
      http://localhost:8080/applicants

### Example Response

    [
      {
        "fileName": "applicants.json",
        "processingTimeMs": 3,
        "applicants": [
          {
            "name": "John",
            "email": "john@example.com",
            "hasResume": true
          }
        ]
      }
    ]

Only applicants with `hasResume: true` are returned.

## Running Tests

From the project root:

    dotnet test

This runs the unit tests for the file processor and file validation.

## Running Without Docker

To run the API directly using the .NET SDK:

    dotnet restore
    dotnet build
    dotnet run

ASP.NET Core will display the URL where the API is listening.

## Project Structure

    SecureFileApi/
    ├── Program.cs
    ├── Authentication/
        ├── ApiKeyAuthenticationHandler.cs
    ├── Models
        ├── Applicant.cs
        ├── ApplicantResult.cs
    ├── Services
        ├── ApplicantService.cs
        ├── FileProcessor.cs
    ├── FileValidator.cs
    ├── Dockerfile
    └── Tests/
        ├── FileProcessorTests.cs
        └── FileValidatorTests.cs

## Notes

- Only JSON files are supported.
- The API requires an `X-API-Key` header.
- Multiple files can be uploaded in a single request.
- Uploaded files are processed in memory and are not permanently stored.
- Processing time is reported for each uploaded file.