using OfficeOpenXml;

namespace ProjectBackend.Config.Licensing;

public static class ExcelLicenseConfig
{
    private static bool _isInitialized = false;

    public static void Initialize()
    {
        if (_isInitialized) return;

        // For non-commercial personal use
        ExcelPackage.License.SetNonCommercialPersonal("Pravij");

        // OR for non-commercial organization:
        // ExcelPackage.License.SetNonCommercialOrganization("Your Organization Name");

        _isInitialized = true;
    }
}
