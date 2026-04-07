// Controllers/UserExportController.cs
using Microsoft.AspNetCore.Mvc;
using ProjectBackend.Services.IServices;
using ProjectBackend.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [AdminAuthorize]
    [CheckJwtBlacklist]
    [Route("api/[controller]")]
    public class UserExportController : ControllerBase
    {
        private readonly IUserExportService _userExportService;

        public UserExportController(IUserExportService userExportService)
        {
            _userExportService = userExportService;
        }

        // GET: api/UserExport/csv
        [HttpGet("csv")]
        public async Task<IActionResult> ExportToCsv([FromQuery] string? organization, [FromQuery] bool? isActive)
        {
            byte[] csvData;

            if (organization != null || isActive.HasValue)
            {
                csvData = await _userExportService.ExportFilteredUsersToCsvAsync(organization, isActive);
            }
            else
            {
                csvData = await _userExportService.ExportUsersToCsvAsync();
            }

            var fileName = $"users_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvData, "text/csv", fileName);
        }

        // GET: api/UserExport/excel
        [HttpGet("excel")]
        [AdminAuthorize]
        [CheckJwtBlacklist]
        public async Task<IActionResult> ExportToExcel([FromQuery] string? organization, [FromQuery] bool? isActive)
        {
            byte[] excelData;

            if (organization != null || isActive.HasValue)
            {
                excelData = await _userExportService.ExportFilteredUsersToExcelAsync(organization, isActive);
            }
            else
            {
                excelData = await _userExportService.ExportUsersToExcelAsync();
            }

            var fileName = $"users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: api/UserExport/csv/active
        [HttpGet("csv/active")]
        [AdminAuthorize]
        [CheckJwtBlacklist]
        public async Task<IActionResult> ExportActiveUsersToCsv()
        {
            var csvData = await _userExportService.ExportFilteredUsersToCsvAsync(isActive: true);
            var fileName = $"active_users_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvData, "text/csv", fileName);
        }

        // GET: api/UserExport/excel/organization/{orgName}
        [HttpGet("excel/organization/{orgName}")]
        [AdminAuthorize]
        [CheckJwtBlacklist]
        public async Task<IActionResult> ExportUsersByOrganizationToExcel(string orgName)
        {
            var excelData = await _userExportService.ExportFilteredUsersToExcelAsync(organization: orgName);
            var fileName = $"users_{orgName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
