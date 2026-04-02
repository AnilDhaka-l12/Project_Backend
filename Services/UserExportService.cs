using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using projectBackend.Model.Entities;
using projectBackend.Model.Dto;
using projectBackend.Repositories.Interfaces;
using projectBackend.Services.IServices;

namespace projectBackend.Services
{
    public class UserExportService : IUserExportService
    {
        private readonly IUserRepository _userRepository;

        public UserExportService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // CSV Export - All Users
        public async Task<byte[]> ExportUsersToCsvAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return GenerateCsv(users);
        }

        // Excel Export - All Users
        public async Task<byte[]> ExportUsersToExcelAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return GenerateExcel(users);
        }

        // CSV Export - Filtered Users
        public async Task<byte[]> ExportFilteredUsersToCsvAsync(string? organization = null, bool? isActive = null)
        {
            IEnumerable<User> users = await _userRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(organization))
            {
                users = users.Where(u => u.Organization == organization);
            }
            
            if (isActive.HasValue)
            {
                users = users.Where(u => u.IsActive == isActive.Value);
            }
            
            return GenerateCsv(users);
        }

        // Excel Export - Filtered Users
        public async Task<byte[]> ExportFilteredUsersToExcelAsync(string? organization = null, bool? isActive = null)
        {
            IEnumerable<User> users = await _userRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(organization))
            {
                users = users.Where(u => u.Organization == organization);
            }
            
            if (isActive.HasValue)
            {
                users = users.Where(u => u.IsActive == isActive.Value);
            }
            
            return GenerateExcel(users);
        }

        // Private helper methods
        private byte[] GenerateCsv(IEnumerable<User> users)
        {
            var exportUsers = users.Select(u => new UserExportDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = $"{u.Name} {u.Surname}",  // Combine Name and Surname
                Organization = u.Organization,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });

            var csvBuilder = new StringBuilder();
            
            // Add headers
            csvBuilder.AppendLine("Id,Email,FullName,Organization,IsActive,CreatedAt,UpdatedAt");
            
            // Add data rows
            foreach (var user in exportUsers)
            {
                csvBuilder.AppendLine($"\"{user.Id}\",\"{user.Email}\",\"{user.Username}\",\"{user.Organization}\",{user.IsActive},{user.CreatedAt:yyyy-MM-dd HH:mm:ss},{(user.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")}");
            }
            
            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        private byte[] GenerateExcel(IEnumerable<User> users)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users");
            
            // Headers with styling
            var headers = new[] { "ID", "Email", "Full Name", "Organization", "Is Active", "Created At", "Updated At" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
            
            // Data rows
            int row = 2;
            foreach (var user in users)
            {
                worksheet.Cells[row, 1].Value = user.Id;
                worksheet.Cells[row, 2].Value = user.Email;
                worksheet.Cells[row, 3].Value = $"{user.Name} {user.Surname}";  // Combine Name and Surname
                worksheet.Cells[row, 4].Value = user.Organization;
                worksheet.Cells[row, 5].Value = user.IsActive ? "Yes" : "No";
                worksheet.Cells[row, 6].Value = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[row, 7].Value = user.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                row++;
            }
            
            // Auto-fit columns
            worksheet.Cells[1, 1, row - 1, 7].AutoFitColumns();
            
            return package.GetAsByteArray();
        }
    }
}