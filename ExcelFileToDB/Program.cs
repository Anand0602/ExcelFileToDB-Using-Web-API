
using ExcelFileToDB;
using ExcelFileToDB.Services.DatabaseSaverServices;
using ExcelFileToDB.Services.ExcelReaderServices;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
builder.Services.AddScoped<IDatabaseSaverService, DatabaseSaverService>();


// For Noncommercial personal use
ExcelPackage.License.SetNonCommercialPersonal("Anand");

var app = builder.Build();



// Configure the HTTP request pipeline.

app.UseAuthorization();
app.UseMiddleware<ApiKeyMiddleware>();



app.MapControllers();

app.Run();
