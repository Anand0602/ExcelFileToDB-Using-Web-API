using ExcelFileToDB.Services.DatabaseSaverServices;
using ExcelFileToDB.Services.ExcelReaderServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;

namespace ExcelFileToDB.Controllers
{
    // This tells ASP.NET Core that this is an API controller,
    // and its routes will be based on the controller name
    [ApiController]
    [Route("[controller]")]
    public class ProcessExcelController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IExcelReaderService _excelReader;
        private readonly IDatabaseSaverService _dbSaver;

        public ProcessExcelController(IConfiguration configuration, IExcelReaderService excelReader, IDatabaseSaverService dbSaver)
        {
            _configuration = configuration;
            _excelReader = excelReader;
            _dbSaver = dbSaver;
        }
        public IActionResult Index()
        {
            WeatherForecast weatherForecast = new WeatherForecast();
            weatherForecast.Date = DateOnly.FromDateTime(DateTime.Now);
            weatherForecast.TemperatureC = 25;
            weatherForecast.Summary = "Sunny";

            return Json(weatherForecast);
        }

        // This POST endpoint handles file uploads
        // It expects two form fields: an Excel file and the sheet name to read

        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel([FromForm] IFormFile file, [FromForm] string sheetName)
        {
            try
            {
                // First, we read the Excel data using our reader service
                var rows = await _excelReader.ReadExcelAsync(file, sheetName);

                // Next, we save the data to the database using our saver service
                // Right now it's hardcoded to save to the "Customers" table
                await _dbSaver.SaveDataAsync(rows, "Customers"); // Replace "Customers" with dynamic if needed
                return Ok(rows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}
