using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using UMS.Utilities;

class Student : IUniversity, IAddress
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int IsFullTime { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }

    private bool Validate(string email)
    {
        List<string> ValidEmails = new List<string>() { "gmail.com", "outlook.com", "yahoo.com" };
        if (email.IsNullOrEmpty() || !email.Contains("@"))
            return false;
        string[] part = email.Split("@");
        if (!ValidEmails.Contains(part[1])) 
            return false;
        return true;
    }

    public async Task AddAsync(int isFullTime)
    {
        
        Console.Clear();
        IsFullTime = isFullTime;
        Console.WriteLine("Enter details of Student to be Added: ");
        while (true) { 
            Console.Write("Enter Student Name: ");
            StudentName = Console.ReadLine().Trim();
            if (StudentName.IsNullOrEmpty()) Console.WriteLine("Inavlid Name...");
            else break;
        }
        while (true)
        {
            //Console.WriteLine("Valid date Formats:\ndd/mm/yyyy\tyyyy/mm/dd\ndd-mm-yyyy\tyyyy-mm-dd\ndd.mm.yyyy\tyyyy.mm.dd\n");
            Console.Write("Enter Student DOB :");
            bool valid = DateTime.TryParse(Console.ReadLine().Trim(), out DateTime parsedDate);
            if (valid && parsedDate<DateTime.Now)
            {
                if ((DateTime.Now.Year - parsedDate.Year) <= 3)
                    Console.WriteLine("Student too Young...");
                else if((DateTime.Now.Year - parsedDate.Year) >=100)
                    Console.WriteLine("Student Too Old...");
                else { 
                    DateOfBirth = parsedDate;
                    break;
                }
            }
            else
            {
                Console.WriteLine("Invalid Date ...");
            }
        }
        while (true)
        {
            Console.Write("Enter Student Email (): ");
            Email = Console.ReadLine().Trim();

            if (Validate(Email)) break;
            else Console.WriteLine("Inavlid Email...");
        }
        Console.Write("Enter Student Address: ");
        while (true)
        {
            Console.Write("Street: ");
            Street = Console.ReadLine().Trim();
            if (Street.IsNullOrEmpty()) Console.WriteLine("Invalid street...");
            else break;
        }
        while (true)
        {
            Console.Write("City: ");
            City = Console.ReadLine().Trim();
            if (City.IsNullOrEmpty()) Console.WriteLine("Invalid City...");
            else break;
        }
        while (true)
        {
            Console.Write("State: ");
            State = Console.ReadLine().Trim();
            if (State.IsNullOrEmpty()) Console.WriteLine("Invalid State...");
            else break;
        }
        while (true)
        {
            Console.Write("Pincode: ");
            Pincode = Console.ReadLine().Trim();
            if (Pincode.IsNullOrEmpty()) Console.WriteLine("Invalid Pincode...");
            else break;
        }
        
        EnrollmentDate = DateTime.Now;
        await DBServices.AddStudentDBAsync(this);
    }

    public async Task RemoveAsync()
    {
        while (true)
        {
            Console.Write("Enter the StudentId of the student to to Removed: ");
            bool valid = int.TryParse(Console.ReadLine().Trim(), out int id);
            if (valid) { StudentId = id; break; }
            else Console.WriteLine("Invalid StudentId");
        }
        await DBServices.RemoveDBAsync(this,StudentId);
    }

    public virtual async Task<int> CalculateFeesAsync(int id)
    {
        
        int count = 0;
        int isFulltime = 2;
        StudentId = id;
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmd = new SqlCommand("Select * from Student where StudentID = @id", conn);
            cmd.Parameters.AddWithValue("@id",StudentId);
            conn.Open();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                count++;
                isFulltime = Convert.ToInt32(reader["IsFulltime"]);
            }
            try { 
                if(count == 0)
                {
                    throw new StudentNotFoundException("Student Not Found...");
                }
            }
            catch(StudentNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return 2;
            }
            return isFulltime;
        }
    }

    public async Task PayFees(int amount,int id)
    {
        if (await DBServices.CheckIfPaid(id))
        {
            Console.WriteLine("Fees Already Paid");
            return;
        }
        //string 
        StudentId=id;
        CreditCardPaymentGateway card= new CreditCardPaymentGateway();
        Console.WriteLine("Amount To be paid: "+amount);
            Console.WriteLine("Enter Credit Card Details: ");
        while (true)
        {
            Console.Write("XXXX XXXX XXXX ");
            card.CardNumber = Console.ReadLine();
            if (card.CardNumber.Length != 4 || !card.CardNumber.All(char.IsDigit))
            {
                Console.WriteLine("Invalid Card...");
            }
            else break;
        }
        while (true)
        {
            Console.WriteLine("Enter Holder Name: ");
            card.CardHolderName = Console.ReadLine();
            if (card.CardHolderName.IsNullOrEmpty()) Console.WriteLine("Invalid Name...");
            else break;
        }

        Console.Write("Enter Expiration Date (MM/YY): ");
        card.ExpirationDate = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(card.ExpirationDate) || !card.ExpirationDate.IsValidExpirationDate())
        {
            Console.WriteLine("Invalid Expiration Date. Please enter in MM/YY format.");
            Console.Write("Enter Expiration Date (MM/YY): ");
            card.ExpirationDate = Console.ReadLine();
        }

        Console.Write("Enter CVV (3 digits): ");
        card.CVV = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(card.CVV) || card.CVV.Length != 3 || !card.CVV.All(char.IsDigit))
        {
            Console.WriteLine("Invalid CVV. Please enter a valid 3-digit CVV.");
            Console.Write("Enter CVV (3 digits): ");
            card.CVV = Console.ReadLine();
        }
        Thread.Sleep(250);
        Console.WriteLine("Processing Payemnt...");
        string transId = await DBServices.PaymentAsync(StudentId, amount,card);
        Console.WriteLine("TransactionID: "+transId);
        Thread.Sleep(500);
        Console.WriteLine("Payement Success");
    }   
}
