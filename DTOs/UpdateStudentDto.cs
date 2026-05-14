using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

public class UpdateStudentDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Range(1, 120)]
    public int Age { get; set; }
}
