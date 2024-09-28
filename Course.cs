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

    //public async Task<List<int>> Display()
    //{

    //    using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
    //    {
    //        SqlCommand cmdDisplay = new SqlCommand();
    //        cmdDisplay.Connection = conn;
    //        cmdDisplay.CommandText = "Select * from Course";
    //        conn.Open();
    //        SqlDataReader reader = await cmdDisplay.ExecuteReaderAsync();
    //        List<int> result = new List<int>();
    //        Console.WriteLine($"{"CourseID",-10}\t" +
    //              $"{"CourseName",-30}\t" +
    //              $"{"Credits"}");
    //        Console.WriteLine("-------------------------------------------------------");

    //        while (reader.Read())
    //        {
    //            Console.WriteLine($"{reader["CourseId"],-10}\t" +
    //                              $"{reader["CourseName"],-30}\t" +
    //                              $"{reader["Credits"]}");
    //            result.Add(Convert.ToInt32(reader["CourseID"]));
    //        }
    //        reader.Close();
    //        conn.Close();

    //        return result;
    //    }
    //}
}

