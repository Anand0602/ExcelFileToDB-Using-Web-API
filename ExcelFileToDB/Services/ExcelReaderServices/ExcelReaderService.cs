using OfficeOpenXml;

namespace ExcelFileToDB.Services.ExcelReaderServices
{
    public class ExcelReaderService : IExcelReaderService
    {
        public async Task<List<Dictionary<string, object>>> ReadExcelAsync(IFormFile file, string sheetName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var rows = new List<Dictionary<string, object>>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet == null)
                throw new ArgumentException("Sheet not found.");

            int colCount = worksheet.Dimension.Columns;
            int rowCount = worksheet.Dimension.Rows;

            var headers = new List<string>();
            for (int col = 1; col <= colCount; col++)
                headers.Add(worksheet.Cells[1, col].Text);

            // Add audit fields
            headers.Add("Created By");
            headers.Add("Updated By");
            headers.Add("Created At");
            headers.Add("Updated At");

            for (int row = 2; row <= rowCount; row++)
            {
                var rowDict = new Dictionary<string, object>();
                for (int col = 1; col <= colCount; col++)
                {
                    rowDict[headers[col - 1]] = worksheet.Cells[row, col].Text;
                }

                rowDict["Created By"] = "Admin";
                rowDict["Updated By"] = "Admin";
                rowDict["Created At"] = DateTime.Now;
                rowDict["Updated At"] = DateTime.Now;

                rows.Add(rowDict);
            }

            return rows;
        }
    }
}
