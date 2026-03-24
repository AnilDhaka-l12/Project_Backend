using System.ComponentModel.DataAnnotations;

namespace projectBackend.Model.RequestModel;

public class UserRequestModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Surname is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Surname must be between 2 and 100 characters")]
    public string Surname { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Occupation is required")]
    [StringLength(100, ErrorMessage = "Occupation cannot exceed 100 characters")]
    public string Occupation { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Organization is required")]
    [StringLength(200, ErrorMessage = "Organization cannot exceed 200 characters")]
    public string Organization { get; set; } = string.Empty;
}