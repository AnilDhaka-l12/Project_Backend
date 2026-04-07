namespace ProjectBackend.Model.Dto;

public class UserQueryParams : PaginationParams
{
    public string? Email { get; set; }
    public string? Name { get; set; }
}