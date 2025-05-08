namespace ExcelFileToDB.Services.ExcelReaderServices
{
    public interface IExcelReaderService
    {
        Task<List<Dictionary<string, object>>> ReadExcelAsync(IFormFile file, string sheetName);
    }
}
