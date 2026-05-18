using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;
using StudentApi.DTOs;

namespace StudentApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents([FromQuery] StudentQueryDto queryDto)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Name))
        {
            var name = queryDto.Name.Trim().ToLower();
            query = query.Where(student => student.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Email))
        {
            var email = queryDto.Email.Trim().ToLower();
            query = query.Where(student => student.Email.ToLower().Contains(email));
        }

        if (queryDto.Age.HasValue)
        {
            query = query.Where(student => student.Age == queryDto.Age.Value);
        }

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderBy(student => student.Id)
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        var response = new PagedResponseDto<Student>
        {
            Items = students,
            Page = queryDto.Page,
            PageSize = queryDto.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent(CreateStudentDto dto)
    {
        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email,
            Age = dto.Age
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, UpdateStudentDto dto)
    {
        var existingStudent = await _context.Students.FindAsync(id);

        if (existingStudent == null)
        {
            return NotFound();
        }

        existingStudent.Name = dto.Name;
        existingStudent.Email = dto.Email;
        existingStudent.Age = dto.Age;

        await _context.SaveChangesAsync();

        return Ok(existingStudent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
