using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using UMS;

class Faculty : IUniversity, IAddress
{
    public int FacultyId { get; set; }
    public string FacultyName { get; set; }
    public string Department { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }

    public async Task AddAsync()
    {
        
        while (true)
        {
            Console.WriteLine("Enter Faculty Name: ");
            FacultyName = Console.ReadLine().Trim();
            if (FacultyName.IsNullOrEmpty()) Console.WriteLine("Invalid Name...");
            else break;
        }
        while (true)
        {
            Console.WriteLine("Enter Faculty Department: ");
            Department = Console.ReadLine().Trim();
            if (Department.IsNullOrEmpty()) Console.WriteLine("Invalid Department...");
            else break;
        }
        Console.WriteLine("Enter Faculty Address: ");
        while (true)
        {
            Console.WriteLine("Street: ");
            Street = Console.ReadLine().Trim();
            if (Street.IsNullOrEmpty()) Console.WriteLine("Invalid street...");
            else break;
        }
        while (true)
        {
            Console.WriteLine("City: ");
            City = Console.ReadLine().Trim();
            if (City.IsNullOrEmpty()) Console.WriteLine("Invalid City...");
            else break;
        }
        while (true)
        {
            Console.WriteLine("State: ");
            State = Console.ReadLine().Trim();
            if (State.IsNullOrEmpty()) Console.WriteLine("Invalid State...");
            else break;
        }
        while (true)
        {
            Console.WriteLine("Pincode: ");
            Pincode = Console.ReadLine().Trim();
            if (Pincode.IsNullOrEmpty()) Console.WriteLine("Invalid Pincode...");
            else break;
        }

        await DBServices.AddFacultyDBAsync(this);
    }

    public async Task RemoveAsync()
    {
        while (true)
        {
            Console.WriteLine("Enter the FacultyId of the Faculty to to Removed: ");
            bool valid = int.TryParse(Console.ReadLine().Trim(), out int id);
            if (valid) { FacultyId = id; break; }
            else Console.WriteLine("Invalid FacultyID");
        }
        await DBServices.RemoveDBAsync(this,FacultyId);
    }
}
