namespace ExcelFileToDB.Services.DatabaseSaverServices
{
    public interface IDatabaseSaverService
    {
        Task SaveDataAsync(List<Dictionary<string, object>> rows, string tableName);
    }
}
