# ExcelFileToDB API

This is an ASP.NET Core Web API that allows users to upload Excel files and save their data to a SQL database.

## Features

* Upload `.xlsx` files via HTTP POST
* Read data from a specified worksheet
* Save data to a predefined database table (currently "Customers")

## Technologies Used

* ASP.NET Core
* EPPlus for Excel file handling
* Dependency Injection for services
* SQL Server (or compatible database)

## Endpoints

### `GET /ProcessExcel`

Returns a sample weather forecast JSON object — mainly for testing.

### `POST /ProcessExcel/upload`

Uploads and processes an Excel file.

#### Form Parameters:

| Name        | Type        | Description                       |
| ----------- | ----------- | --------------------------------- |
| `file`      | `IFormFile` | The Excel file (.xlsx) to upload  |
| `sheetName` | `string`    | The name of the worksheet to read |

#### Example using `curl`:

```bash
curl -X POST http://localhost:5235/ProcessExcel/upload \
  -F "file=@./sample.xlsx" \
  -F "sheetName=Sheet1"
```

## Project Structure

```
/Controllers
    ProcessExcelController.cs  # API endpoint for file processing

/Services
    IExcelReaderService.cs     # Interface for reading Excel files
    IDatabaseSaverService.cs   # Interface for saving data to DB
    ExcelReaderService.cs      # Implementation of Excel reading
    DatabaseSaverService.cs    # Implementation of DB saving

/Models
    WeatherForecast.cs         # Sample model for test endpoint
```

## Configuration

Make sure your `appsettings.json` or environment variables contain a valid database connection string.

## Notes

* Only `.xlsx` files are supported.
* Worksheet name must exactly match what's in the Excel file.
* Data is currently saved to the `Customers` table (hardcoded).

## Setup Instructions

1. Clone the repository:

   ```bash
   git clone <repo-url>
   cd ExcelFileToDB
   ```

2. Restore NuGet packages:

   ```bash
   dotnet restore
   ```

3. Build and run the project:

   ```bash
   dotnet run
   ```

4. The API should be available at `http://localhost:5235`

## Future Enhancements

* Allow clients to specify the target table name
* Add Excel header validation
* Return more detailed error responses
