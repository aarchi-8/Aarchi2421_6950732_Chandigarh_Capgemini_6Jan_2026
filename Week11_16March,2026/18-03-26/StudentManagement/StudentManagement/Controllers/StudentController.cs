using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using System.Linq;

public class StudentController : Controller
{
    private readonly StudentDbContext _context;

    public StudentController(StudentDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var students = _context.Students.ToList();
        return View(students);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(student);
    }
    public IActionResult Edit(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null) return NotFound();
        return PartialView("_EditModal", student);
    }

    [HttpPost]
    public IActionResult Edit(Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return PartialView("_EditModal", student);
    }
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var student = _context.Students.Find(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

}
