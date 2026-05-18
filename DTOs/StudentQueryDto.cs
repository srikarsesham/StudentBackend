using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

public class StudentQueryDto
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    public string? Name { get; set; }

    public string? Email { get; set; }

    [Range(1, 120)]
    public int? Age { get; set; }
}
