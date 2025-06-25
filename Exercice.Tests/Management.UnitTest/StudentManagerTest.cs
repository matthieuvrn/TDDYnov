using Management.Model;
using Management.Service;
using Xunit;
using FluentAssertions;

namespace Management.UnitTest;

public class StudentManagerTest
{
    private readonly StudentManager _manager = new();

    #region AddStudent Tests

    [Fact]
    public void AddStudent_ValidStudent_AddsToCollection()
    {
        // Arrange
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };

        // Act
        _manager.AddStudent(student);

        // Assert
        _manager.Students.Should().HaveCount(1);
        _manager.Students.Should().Contain(student);
    }

    [Fact]
    public void AddStudent_NullStudent_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _manager.AddStudent(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddStudent_DuplicateId_ThrowsArgumentException()
    {
        // Arrange
        var student1 = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
        var student2 = new Student { Id = 1, FirstName = "Jane", LastName = "Smith", Age = 22 };
        _manager.AddStudent(student1);

        // Act & Assert
        var act = () => _manager.AddStudent(student2);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*already exists*");
    }

    [Fact]
    public void AddStudent_MultipleValidStudents_AddsAllToCollection()
    {
        // Arrange
        var students = new[]
        {
            new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 },
            new Student { Id = 2, FirstName = "Jane", LastName = "Smith", Age = 22 },
            new Student { Id = 3, FirstName = "Bob", LastName = "Johnson", Age = 19 }
        };

        // Act
        foreach (var student in students)
            _manager.AddStudent(student);

        // Assert
        _manager.Students.Should().HaveCount(3);
        _manager.Students.Should().SatisfyRespectively(
            first => first.Id.Should().Be(1),
            second => second.Id.Should().Be(2),
            third => third.Id.Should().Be(3)
        );
    }

    #endregion

    #region GetStudentById Tests

    [Fact]
    public void GetStudentById_ExistingId_ReturnsCorrectStudent()
    {
        // Arrange
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
        _manager.AddStudent(student);

        // Act
        var result = _manager.GetStudentById(1);

        // Assert
        result.Should().Be(student);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public void GetStudentById_NonExistingId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _manager.GetStudentById(999);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*not found*");
    }

    #endregion

    #region GetStudentsByAge Tests

    [Fact]
    public void GetStudentsByAge_ValidRange_ReturnsCorrectStudents()
    {
        // Arrange
        _manager.AddStudent(new Student { Id = 1, Age = 18, FirstName = "John", LastName = "Ynov" });
        _manager.AddStudent(new Student { Id = 2, Age = 20, FirstName = "Doe", LastName = "Ynov" });
        _manager.AddStudent(new Student { Id = 3, Age = 25, FirstName = "Paul", LastName = "Ynov" });

        // Act
        var result = _manager.GetStudentsByAge(18, 22);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(s => s.Age >= 18 && s.Age <= 22);
    }

    [Fact]
    public void GetStudentsByAge_EmptyCollection_ReturnsEmpty()
    {
        // Act
        var result = _manager.GetStudentsByAge(18, 22);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetStudentsByAge_NoMatchingAge_ReturnsEmpty()
    {
        // Arrange
        _manager.AddStudent(new Student { Id = 1, Age = 15, FirstName = "Young", LastName = "Student" });
        _manager.AddStudent(new Student { Id = 2, Age = 30, FirstName = "Old", LastName = "Student" });

        // Act
        var result = _manager.GetStudentsByAge(18, 22);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetStudentsByAge_InvalidRange_ThrowsArgumentException()
    {
        // Act & Assert
        var act1 = () => _manager.GetStudentsByAge(25, 20);
        act1.Should().Throw<ArgumentException>();

        var act2 = () => _manager.GetStudentsByAge(-1, 20);
        act2.Should().Throw<ArgumentException>();

        var act3 = () => _manager.GetStudentsByAge(20, -1);
        act3.Should().Throw<ArgumentException>();
    }

    #endregion

    #region GetTopStudents Tests

    [Fact]
    public void GetTopStudents_ValidCount_ReturnsTopStudentsSortedByAverage()
    {
        // Arrange
        _manager.AddStudent(new Student
        {
            Id = 1,
            FirstName = "Alice",
            LastName = "Johnson",
            Age = 20,
            Grades = new List<int> { 85, 90, 95 } // moyenne: 90
        });
        _manager.AddStudent(new Student
        {
            Id = 2,
            FirstName = "Bob",
            LastName = "Smith",
            Age = 21,
            Grades = new List<int> { 95, 100, 90 } // moyenne: 95
        });
        _manager.AddStudent(new Student
        {
            Id = 3,
            FirstName = "Charlie",
            LastName = "Brown",
            Age = 19,
            Grades = new List<int> { 70, 75, 80 } // moyenne: 75
        });

        // Act
        var result = _manager.GetTopStudents(2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().SatisfyRespectively(
            first => {
                first.FirstName.Should().Be("Bob");
                first.AverageGrade.Should().Be(95);
            },
            second => {
                second.FirstName.Should().Be("Alice");
                second.AverageGrade.Should().Be(90);
            }
        );
        result.Should().OnlyContain(s => s.Grades.Count > 0);
    }

    [Fact]
    public void GetTopStudents_CountZero_ReturnsEmpty()
    {
        // Arrange
        _manager.AddStudent(new Student
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Age = 20,
            Grades = new List<int> { 85, 90 }
        });

        // Act
        var result = _manager.GetTopStudents(0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetTopStudents_CountGreaterThanAvailable_ReturnsAllStudentsWithGrades()
    {
        // Arrange
        _manager.AddStudent(new Student
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Age = 20,
            Grades = new List<int> { 85, 90 }
        });
        _manager.AddStudent(new Student
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            Age = 21,
            Grades = new List<int>() // pas de notes
        });

        // Act
        var result = _manager.GetTopStudents(5);

        // Assert
        result.Should().HaveCount(1); // juste lui avec des notes
        result.Should().OnlyContain(s => s.Grades.Count > 0);
    }

    [Fact]
    public void GetTopStudents_NegativeCount_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _manager.GetTopStudents(-1);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region RemoveStudent Tests

    [Fact]
    public void RemoveStudent_ExistingId_RemovesStudentAndReturnsTrue()
    {
        // Arrange
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
        _manager.AddStudent(student);

        // Act
        var result = _manager.RemoveStudent(1);

        // Assert
        result.Should().BeTrue();
        _manager.Students.Should().BeEmpty();
    }

    [Fact]
    public void RemoveStudent_NonExistingId_ReturnsFalse()
    {
        // Act
        var result = _manager.RemoveStudent(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RemoveStudent_FromMultipleStudents_RemovesOnlySpecified()
    {
        // Arrange
        var student1 = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
        var student2 = new Student { Id = 2, FirstName = "Jane", LastName = "Smith", Age = 22 };
        _manager.AddStudent(student1);
        _manager.AddStudent(student2);

        // Act
        var result = _manager.RemoveStudent(1);

        // Assert
        result.Should().BeTrue();
        _manager.Students.Should().HaveCount(1);
        _manager.Students.First().Id.Should().Be(2);
    }

    #endregion

    #region UpdateStudentGrades Tests

    [Fact]
    public void UpdateStudentGrades_ValidGrades_UpdatesSuccessfully()
    {
        // Arrange
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
        _manager.AddStudent(student);
        var newGrades = new List<int> { 85, 90, 95 };

        // Act
        _manager.UpdateStudentGrades(1, newGrades);

        // Assert
        var updatedStudent = _manager.GetStudentById(1);
        updatedStudent.Grades.Should().BeEquivalentTo(newGrades);
        updatedStudent.AverageGrade.Should().Be(90);
    }

    [Fact]
    public void UpdateStudentGrades_InvalidGrades_ThrowsArgumentException()
    {
        // Arrange
        var student = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
        _manager.AddStudent(student);

        // Act & Assert
        var act1 = () => _manager.UpdateStudentGrades(1, new List<int> { -1, 50, 100 });
        act1.Should().Throw<ArgumentException>();

        var act2 = () => _manager.UpdateStudentGrades(1, new List<int> { 50, 101, 80 });
        act2.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateStudentGrades_NonExistingStudent_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _manager.UpdateStudentGrades(999, new List<int> { 85, 90 });
        act.Should().Throw<ArgumentException>();
    }

    #endregion

}