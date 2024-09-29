using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using UMS;
class Course : IUniversity
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int Credits { get; set; }

    public async Task AddAsync()
    {
        while (true) { 
            Console.WriteLine("Enter Course Name: ");
            CourseName = Console.ReadLine();
            if (CourseName.IsNullOrEmpty()) { Console.WriteLine("Invalid Course Name...."); }
            else break;
        }
        while (true) { 
            Console.WriteLine("Enter Course Credits: ");
            bool valid = int.TryParse(Console.ReadLine(), out int cred);
            if (valid) { Credits = cred; break; }
            else Console.WriteLine("Invalid Credits...");
        }
        await DBServices.AddCourseDBAsync(this);
    }

    public async Task RemoveAsync()
    {
        while (true)
        {
            Console.WriteLine("Enter the Id of the Course to to Removed: ");
            bool valid = int.TryParse(Console.ReadLine(), out int id);
            if (valid) { CourseId = id; break; }
            else Console.WriteLine("Invalid CourseID...");
        }
        await DBServices.RemoveDBAsync(this,CourseId);
    }
}

