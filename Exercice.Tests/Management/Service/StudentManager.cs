using Management.Model;

namespace Management.Service;

public class StudentManager : IStudentManager
{
    private readonly List<Student> _students = new();

    public IReadOnlyList<Student> Students => _students.AsReadOnly();

    public void AddStudent(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        if (_students.Any(s => s.Id == student.Id))
            throw new ArgumentException($"Student with ID {student.Id} already exists");

        _students.Add(student);
    }

    public Student GetStudentById(int id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student == null)
            throw new ArgumentException($"Student with ID {id} not found");

        return student;
    }

    public List<Student> GetStudentsByAge(int minAge, int maxAge)
    {
        if (minAge < 0 || maxAge < 0)
            throw new ArgumentException("Ages cannot be negative");

        if (minAge > maxAge)
            throw new ArgumentException("Minimum age cannot be greater than maximum age");

        return _students
            .Where(s => s.Age >= minAge && s.Age <= maxAge)
            .OrderBy(s => s.Age)
            .ToList();
    }

    public List<Student> GetTopStudents(int count)
    {
        if (count < 0)
            throw new ArgumentException("Count cannot be negative");

        return _students
            .Where(s => s.Grades.Any()) // seulement les étudiants avec des notes
            .OrderByDescending(s => s.AverageGrade)
            .ThenBy(s => s.FullName) // tri secondaire par nom pour lisbilité
            .Take(count)
            .ToList();
    }

    public bool RemoveStudent(int id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student == null)
            return false;

        _students.Remove(student);
        return true;
    }

    public void UpdateStudentGrades(int studentId, List<int> newGrades)
    {
        var student = GetStudentById(studentId);

        if (newGrades != null && newGrades.Any(grade => grade < 0 || grade > 100))
            throw new ArgumentException("Grades must be between 0 and 100");

        student.Grades = newGrades ?? new List<int>();
    }
}