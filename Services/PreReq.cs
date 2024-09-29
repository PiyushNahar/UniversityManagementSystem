using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

class PreReq
{
    //private readonly ILogger<PreReq> _logger;
    //public PreReq(ILogger<PreReq> logger)
    //{
    //    _logger = logger;
    //}
    public async static Task Requirements()
    {
        try { 
            using (SqlConnection conn = new SqlConnection(Helper.ConnStrNoDB))
            {
                SqlCommand cmdDB = new SqlCommand();
                cmdDB.CommandText = "IF NOT EXISTS (SELECT NAME FROM SYS.DATABASES WHERE NAME LIKE 'UMS') BEGIN CREATE DATABASE UMS; END";
                cmdDB.Connection = conn;
                conn.Open();
                //_logger.LogInformation();
                await cmdDB.TryExecuteNonQueryAsync();
                conn.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        using (SqlConnection conn= new SqlConnection(Helper.ConnStr)) {

            SqlCommand cmdStudent = new SqlCommand();
            cmdStudent.CommandText = "IF NOT EXISTS " +
                "(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'STUDENT' AND TABLE_SCHEMA = 'dbo')" +
                "BEGIN " +
                "CREATE TABLE STUDENT (" +
                "StudentID INT PRIMARY KEY IDENTITY(1,1)," +
                "StudentNAME VARCHAR(50)," +
                "EMAIL VARCHAR(50)," +
                "DOB DATE," +
                "ENROLLDATE DATETIME," +
                "IsFullTime BIT," +
                "paymentstatus varchar(10));" +
                "END";
            cmdStudent.Connection = conn;
            SqlCommand cmdFaculty = new SqlCommand();
            cmdFaculty.CommandText = "IF NOT EXISTS " +
                "(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'Faculty' AND TABLE_SCHEMA = 'dbo')" +
                "BEGIN " + "CREATE TABLE Faculty (\r\n    FacultyId INT PRIMARY KEY IDENTITY(1,1),\r\n    FacultyName NVARCHAR(100) NOT NULL,\r\n    Department NVARCHAR(100) NOT NULL\r\n); END";
            cmdFaculty.Connection = conn;
            SqlCommand cmdCourse = new SqlCommand();
            cmdCourse.CommandText = "IF NOT EXISTS " +
                "(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'Course' AND TABLE_SCHEMA = 'dbo')" +
                "BEGIN " + "CREATE TABLE Course (\r\n    CourseId INT PRIMARY KEY IDENTITY(1,1),\r\n    CourseName NVARCHAR(100) NOT NULL,\r\n    Credits INT NOT NULL\r\n); END";
            cmdCourse.Connection = conn;
            SqlCommand cmdCoursefaculty = new SqlCommand("IF NOT EXISTS " +
                "(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'FACULTYCOURSE' AND TABLE_SCHEMA = 'dbo')" +
                "BEGIN " + "CREATE TABLE FacultyCourse (\r\n    FacultyId INT,\r\n    CourseId INT,\r\n    PRIMARY KEY (FacultyId, CourseId),\r\n    FOREIGN KEY (FacultyId) REFERENCES Faculty(FacultyId) \r\n        ON DELETE CASCADE, \r\n    FOREIGN KEY (CourseId) REFERENCES Course(COurseId) \r\n        ON DELETE CASCADE\r\n); END",conn);
            SqlCommand cmdAddressStudent = new SqlCommand("IF NOT EXISTS" +
                "(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'ADDRESSStudent' AND TABLE_SCHEMA = 'dbo')" +
                "BEGIN " +
                "CREATE TABLE ADDRESSStudent (" +
                "STREET VARCHAR(30), " +
                "CITY VARCHAR(20)," +
                "STATE VARCHAR(20)," +
                "PINCODE VARCHAR(10)," +
                "StudentId Int," +
                "Constraint FK_AddressStudent_Student FOREIGN KEY (StudentId) references Student(StudentId) ON DELETE CASCADE);" +
                "END", conn);
            SqlCommand cmdAddressFaculty = new SqlCommand("IF NOT EXISTS" +
                "(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'ADDRESSFaculty' AND TABLE_SCHEMA = 'dbo')" +
                "BEGIN " +
                "CREATE TABLE ADDRESSFaculty (" +
                "STREET VARCHAR(30), " +
                "CITY VARCHAR(20)," +
                "STATE VARCHAR(20)," +
                "PINCODE VARCHAR(10)," +
                "FacultyId int ," +
                "Constraint FK_AddressFaculty_Faculty Foreign Key (FacultyId) References Faculty(FacultyId) ON DELETE CASCADE);" +
                "END", conn);
            SqlCommand cmdTransaction = new SqlCommand("IF NOT EXISTS" +
                "(SELECT * FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'TRANSACTION' AND TABLE_SCHEMA = 'dbo')" +
                "BEGIN " +
                "CREATE TABLE [TRANSACTION] (" +
                "TRANSID VARCHAR(50) PRIMARY KEY," +
                "STUDENTID INT NULL," +   
                "CARDHOLDERNAME VARCHAR(50)," +
                "CARDNUMBER INT," +    
                "AMOUNT INT," +
                "PAYEMENTSTATUS VARCHAR(10)," +
                "CONSTRAINT FK_TRANSACTION_STUDENT FOREIGN KEY (STUDENTID) REFERENCES STUDENT(STUDENTID) ON DELETE SET NULL);" +
                "END", conn);

            conn.Open();
            await cmdCourse.TryExecuteNonQueryAsync();
            await cmdStudent.TryExecuteNonQueryAsync();
            await cmdFaculty.TryExecuteNonQueryAsync();
            await cmdCoursefaculty.TryExecuteNonQueryAsync();
            await cmdAddressStudent.TryExecuteNonQueryAsync();
            await cmdAddressFaculty.TryExecuteNonQueryAsync();
            await cmdTransaction.TryExecuteNonQueryAsync();
            conn.Close();
        }
    }

    public async static Task DefaultValues()
    {
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            string insertQueries =
                    "INSERT INTO STUDENT (StudentNAME, EMAIL, DOB, ENROLLDATE, IsFullTime, paymentstatus) VALUES " +
                    "('John Doe', 'john.doe@gmail.com', '2000-05-15', '2019-09-01', 1, 'Paid'), " +
                    "('Jane Smith', 'jane.smith@yahoo.com', '1999-10-25', '2018-09-01', 1, 'Paid'), " +
                    "('Mark Johnson', 'mark.johnson@outlook.com', '1998-07-12', '2020-09-01', 0, 'Pending'), " +
                    "('Emily Davis', 'emily.davis@gmail.com', '2001-01-30', '2021-09-01', 1, 'Paid'), " +
                    "('Michael Brown', 'michael.brown@yahoo.com', '1997-03-22', '2017-09-01', 0, 'Pending'); " +

                    "INSERT INTO Faculty (FacultyName, Department) VALUES " +
                    "('Dr. Alice Carter', 'Computer Science'), " +
                    "('Dr. Bob Thomas', 'Mathematics'), " +
                    "('Dr. Carol Lee', 'Physics'), " +
                    "('Dr. David Moore', 'Chemistry'), " +
                    "('Dr. Eve Wilson', 'Biology'); " +

                    "INSERT INTO Course (CourseName, Credits) VALUES " +
                    "('Computer Science 101', 3), " +
                    "('Mathematics 201', 4), " +
                    "('Physics 301', 3), " +
                    "('Chemistry 401', 4), " +
                    "('Biology 101', 3); " +

                    "INSERT INTO FacultyCourse (FacultyId, CourseId) VALUES " +
                    "(1, 1), " +
                    "(2, 2), " +
                    "(3, 3), " +
                    "(4, 4), " +
                    "(5, 5); " +

                    "INSERT INTO ADDRESSStudent (STREET, CITY, STATE, PINCODE, StudentId) VALUES " +
                    "('123 Elm St', 'Springfield', 'IL', '62701', 1), " +
                    "('456 Oak St', 'Riverdale', 'NY', '10471', 2), " +
                    "('789 Maple St', 'Gotham', 'NY', '10001', 3), " +
                    "('101 Pine St', 'Metropolis', 'KS', '66502', 4), " +
                    "('202 Cedar St', 'Star City', 'CA', '90210', 5); " +

                    "INSERT INTO ADDRESSFaculty (STREET, CITY, STATE, PINCODE, FacultyId) VALUES " +
                    "('987 Elm St', 'Springfield', 'IL', '62701', 1), " +
                    "('654 Oak St', 'Riverdale', 'NY', '10471', 2), " +
                    "('321 Maple St', 'Gotham', 'NY', '10001', 3), " +
                    "('210 Pine St', 'Metropolis', 'KS', '66502', 4), " +
                    "('303 Cedar St', 'Star City', 'CA', '90210', 5); " +

                    "INSERT INTO [TRANSACTION] (TRANSID, STUDENTID, CARDHOLDERNAME, CARDNUMBER, AMOUNT, PAYEMENTSTATUS) VALUES " +
                    "('TRX1001', 1, 'John Doe', 3456, 5000, 'Success'), " +
                    "('TRX1002', 2, 'Jane Smith', 3457, 4500, 'Pending'), " +
                    "('TRX1003', 3, 'Mark Johnson', 3458, 3000, 'Failed'), " +
                    "('TRX1004', 4, 'Emily Davis', 3459, 7000, 'Success'), " +
                    "('TRX1005', 5, 'Michael Brown', 3460, 2500, 'Pending');";

            SqlCommand cmdInsert = new SqlCommand(insertQueries, conn);
            conn.Open();
            await cmdInsert.TryExecuteNonQueryAsync();
            conn.Close();
        }
    }

    public static async Task DropValues()
    {
        using(SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            try
            {
                SqlCommand sqlDelete = new SqlCommand("Drop Table AddressStudent;" +
                "Drop Table AddressFaculty;" +
                "Drop table FacultyCourse;" +
                "Drop Table Course;" +
                "Drop Table Faculty;" +
                "drop table [Transaction];" +
                "Drop Table Student;", conn);
                conn.Open();
                await sqlDelete.TryExecuteNonQueryAsync();
                conn.Close();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}