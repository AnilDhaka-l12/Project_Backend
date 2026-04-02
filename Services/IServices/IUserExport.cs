using projectBackend.Model.Dto;

namespace projectBackend.Services.IServices
{
    public interface IUserExportService
    {
        Task<byte[]> ExportUsersToCsvAsync();
        Task<byte[]> ExportUsersToExcelAsync();
        Task<byte[]> ExportFilteredUsersToCsvAsync(string? organization = null, bool? isActive = null);
        Task<byte[]> ExportFilteredUsersToExcelAsync(string? organization = null, bool? isActive = null);
    }
}