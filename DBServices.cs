using Microsoft.Data.SqlClient;
using System.IO;
using System.Text;
using UMS;

class DBServices
{
    public async static Task AddFacultyDBAsync(Faculty faculty)
    {
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmdInsertFaculty = new SqlCommand("INSERT INTO FACULTY VALUES(@name,@dept); Select @identityvalue = Scope_Identity();", conn);
            cmdInsertFaculty.Parameters.AddWithValue("@name", faculty.FacultyName);
            cmdInsertFaculty.Parameters.AddWithValue("@dept", faculty.Department);
            SqlParameter idparam = new SqlParameter("@identityvalue", System.Data.SqlDbType.Int);
            idparam.Direction = System.Data.ParameterDirection.Output;
            cmdInsertFaculty.Parameters.Add(idparam);
            conn.Open();
            await cmdInsertFaculty.TryExecuteNonQueryAsync();
            conn.Close();
            faculty.FacultyId = Convert.ToInt32(cmdInsertFaculty.Parameters["@identityvalue"].Value);

            SqlCommand cmdAddressFaculty = new SqlCommand("INSERT INTO ADDRESSFACULTY VALUES (@street,@city,@state,@pincode,@id)", conn);
            cmdAddressFaculty.Parameters.AddWithValue("@street", faculty.Street);
            cmdAddressFaculty.Parameters.AddWithValue("@city", faculty.City);
            cmdAddressFaculty.Parameters.AddWithValue("@state", faculty.State);
            cmdAddressFaculty.Parameters.AddWithValue("@pincode", faculty.Pincode);
            cmdAddressFaculty.Parameters.AddWithValue("@id", faculty.FacultyId);
            conn.Open();
            await cmdAddressFaculty.TryExecuteNonQueryAsync();
            conn.Close();

            Course course = new Course();
            var result = await DBServices.DisplayDBAsync("Course");
            while (true)
            {
                try
                {
                    Console.WriteLine("Choose the Course the Faculty will be teaching\nEnter the CourseID: ");
                    bool valid = int.TryParse(Console.ReadLine().Trim(), out int ID);
                    if (valid && result.Contains(ID))
                    {
                        course.CourseId = ID;
                        break;
                    }
                    else
                    {
                        throw new CourseNotFoundException("Invalid Course...");
                    }
                }
                catch (CourseNotFoundException ex) { Console.WriteLine(ex.Message); }
            }
            SqlCommand cmdInsertFC = new SqlCommand("Insert Into FacultyCourse Values (@FacultyId,@CourseId)", conn);
            cmdInsertFC.Parameters.AddWithValue("@FacultyId", faculty.FacultyId);
            cmdInsertFC.Parameters.AddWithValue("@CourseId", course.CourseId);
            conn.Open();
            await cmdInsertFC.TryExecuteNonQueryAsync();
            conn.Close();
            Console.WriteLine("Faculty ID generated: " + faculty.FacultyId);
            Console.WriteLine("Faculty will be teaching Course with CourseID: " + course.CourseId);
        }
    }

    public async static Task AddCourseDBAsync(Course course)
    {
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmdInsertCourse = new SqlCommand("INSERT INTO Course VALUES(@name,@credits); select @identityvalue = SCOPE_IDENTITY();", conn);
            cmdInsertCourse.Parameters.AddWithValue("@name", course.CourseName);
            cmdInsertCourse.Parameters.AddWithValue("@credits", course.Credits);
            SqlParameter idparam = new SqlParameter("@identityvalue", System.Data.SqlDbType.Int);
            idparam.Direction = System.Data.ParameterDirection.Output;
            cmdInsertCourse.Parameters.Add(idparam);
            conn.Open();
            await cmdInsertCourse.TryExecuteNonQueryAsync();
            conn.Close();
            course.CourseId = Convert.ToInt32(cmdInsertCourse.Parameters["@identityvalue"].Value);
            Console.WriteLine("CourseID generated: " + course.CourseId);
        }
    }

    public async static Task AddStudentDBAsync(Student student)
    {
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmdInsertStudent = new SqlCommand();
            cmdInsertStudent.Connection = conn;
            cmdInsertStudent.CommandText = "INSERT INTO STUDENT(StudentNAme,Email,DOB,Enrolldate,Isfulltime) VALUES (@name,@email,@dob,@enroll,@isFullTime); " +
                "Select @identityvalue = Scope_Identity();";
            cmdInsertStudent.Parameters.AddWithValue("@name", student.StudentName);
            cmdInsertStudent.Parameters.AddWithValue("@email", student.Email);
            cmdInsertStudent.Parameters.AddWithValue("@dob", student.DateOfBirth.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdInsertStudent.Parameters.AddWithValue("@enroll", student.EnrollmentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdInsertStudent.Parameters.AddWithValue("@isFullTime", student.IsFullTime);
            SqlParameter idparam = new SqlParameter("@identityvalue", System.Data.SqlDbType.Int);
            idparam.Direction = System.Data.ParameterDirection.Output;
            cmdInsertStudent.Parameters.Add(idparam);
            //Console.WriteLine(cmdInsertStudent.CommandText);
            conn.Open();
            await cmdInsertStudent.TryExecuteNonQueryAsync();
            conn.Close();
            student.StudentId = Convert.ToInt32(cmdInsertStudent.Parameters["@identityvalue"].Value);

            SqlCommand cmdAddressStudent = new SqlCommand("INSERT INTO ADDRESSSTUDENT VALUES (@street,@city,@state,@pincode,@id)", conn);
            cmdAddressStudent.Parameters.AddWithValue("@street", student.Street);
            cmdAddressStudent.Parameters.AddWithValue("@city", student.City);
            cmdAddressStudent.Parameters.AddWithValue("@state", student.State);
            cmdAddressStudent.Parameters.AddWithValue("@pincode", student.Pincode);
            cmdAddressStudent.Parameters.AddWithValue("@id", student.StudentId);
            conn.Open();
            await cmdAddressStudent.TryExecuteNonQueryAsync();
            conn.Close();
            Console.WriteLine("Student ID Generated: " + student.StudentId);
        }
    }

    public static async Task RemoveDBAsync(object source, int Id)
    {
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmdRemove = new SqlCommand(); cmdRemove.Connection = conn;
            if (source.ToString().ToUpper() == "STUDENT")
                cmdRemove.CommandText = "DELETE FROM STUDENT WHERE StudentID = @id";
            else if (source.ToString().ToUpper() == "FACULTY")
                cmdRemove.CommandText = "DELETE FROM FACULTY WHERE FacultyID = @id";
            else if (source.ToString().ToUpper() == "COURSE")
                cmdRemove.CommandText = "DELETE FROM COURSE WHERE CourseID = @id";
            cmdRemove.Parameters.AddWithValue("@id", Id);
            try
            {
                conn.Open();
                int affected = await cmdRemove.TryExecuteNonQueryAsync();
                
                try
                {
                    if (affected == 0)
                    {
                        if (source.ToString().ToUpper() == "STUDENT")
                            throw new StudentNotFoundException("Student Not Found");
                        else if (source.ToString().ToUpper() == "FACULTY")
                            throw new FacultyNotFoundException("Faculty Not Found");
                        else if (source.ToString().ToUpper() == "COURSE")
                            throw new CourseNotFoundException("Course Not Found");
                    }
                    else Console.WriteLine($"Removed {source} with {source}ID " + Id);
                }
                catch (StudentNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (FacultyNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (CourseNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch(SqlException e)
            { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
        }
    }

    public async static Task<List<int>> DisplayDBAsync(object source)
    {
        Student student = new Student();
        Faculty faculty = new();
        Course course = new();
        Dictionary<string, List<string>> display = new Dictionary<string, List<string>>();
        List<int> result = new List<int>();
        display.Add("Student".ToUpper(), new List<string> {
            "Select Student.StudentId,StudentNAME,Email,DOB,ENROLLDATE,(Street + ', ' + City +', '+ State +'- ' + Pincode) as Address from STUDENT, AddressStudent where STUDENT.StudentID = AddressStudent.StudentId" ,
            $"{"StudentID",-10}\t" + $"{"StudentName",-16}\t" + $"{"Email",-25}\t" + $"{"DOB",-12}\t" +$"{"StudentAddress",-40}"+$"{"EnrollDate"}"    
        });
        display.Add("Faculty".ToUpper(), new List<string> {
        "Select Faculty.FacultyId,FacultyName,Department, (Street + ', ' + City +', '+ State +'- ' + Pincode) as Address,Course.CourseId,CourseName,Credits from Faculty, FacultyCourse, Course, AddressFaculty where Faculty.FacultyId = FacultyCourse.FacultyId and Course.CourseId = FacultyCourse.CourseId and AddressFaculty.FacultyId = Faculty.FacultyId",
        $"{"FacultyID",-10}\t" +
                  $"{"FacultyName",-20}\t" +
                  $"{"FacultyAddress",-40}" +
                  $"{"Department",-20}\t" +
                  $"{"CourseId",10}\t" +
                  $"{"CourseName",-30}\t"
        });
        display.Add("Course".ToUpper(), new List<string> {
        "Select * from Course",
        $"{"CourseID",-10}\t" +
                  $"{"CourseName",-30}\t" +
                  $"{"Credits"}"
        });

        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmd = new SqlCommand(display[source.ToString().ToUpper()][0],conn);
            Console.WriteLine(display[source.ToString().ToUpper()][1]);
            Console.WriteLine("-".Replicate(150));
            try
            {
                conn.Open();
                SqlDataReader reader = await cmd.TryExecuteReaderAsync();
                while (reader.Read())
                {

                    if (source.ToString().ToUpper() == "STUDENT")
                    {
                        Console.WriteLine($"{reader["StudentId"],-10}\t" +
                              $"{reader["StudentName"],-16}\t" +
                              $"{reader["Email"],-25}\t" +
                              $"{((DateTime)reader["DOB"]).ToString("yyyy-MM-dd"),-12}\t" +
                              $"{reader["Address"],-40}" +
                              $"{((DateTime)reader["EnrollDate"]).ToString("yyyy-MM-dd")}"
                              );
                    }
                    else if (source.ToString().ToUpper() == "FACULTY")
                    {
                        Console.WriteLine($"{reader["FacultyId"],-10}\t" +
                                      $"{reader["FacultyName"],-20}\t" +
                                      $"{reader["Address"],-40}" +
                                      $"{reader["Department"],-20}\t" +
                                      $"{reader["CourseId"],10}\t" +
                                      $"{reader["CourseName"],-30}\t");
                    }
                    else if (source.ToString().ToUpper() == "COURSE")
                    {
                        Console.WriteLine($"{reader["CourseId"],-10}\t" +
                                          $"{reader["CourseName"],-30}\t" +
                                          $"{reader["Credits"]}");
                        result.Add(Convert.ToInt32(reader["CourseID"]));
                    }
                }
                reader.Close();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
    }

    private async static Task<string> GenerateRandomPassword(int length)
    {
        const string allCharacters = "0123456789";
        Random random = new Random();
        List<string> ExistingTrans = new List<string>();
        StringBuilder passwordBuilder = new StringBuilder(length);
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmdcheck = new SqlCommand("Select TransId from [Transaction]",conn);
            try
            {
                conn.Open();
                SqlDataReader reader = await cmdcheck.TryExecuteReaderAsync();
                while (reader.Read())
                {
                    ExistingTrans.Add(reader["transId"].ToString());
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally
            {
                conn.Close();
            }
        }
        string p;
        while (true)
        {
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(allCharacters.Length);
                passwordBuilder.Append(allCharacters[index]);
            }
            p = passwordBuilder.ToString();
            if (!ExistingTrans.Contains(p)) break;
        }
        return p;
    }
    public async static Task<string> PaymentAsync(int id,int amount,CreditCardPaymentGateway card)
    {
        string TransId = await GenerateRandomPassword(10);
        using (SqlConnection conn = new SqlConnection(Helper.ConnStr)) { 
            SqlCommand cmd = new SqlCommand("INSERT INTO [TRANSACTION] VALUES(@tid,@sid,@name,@number,@amount,'Success')",conn);
            cmd.Parameters.AddWithValue("@tid",TransId);
            cmd.Parameters.AddWithValue("@sid",id);
            cmd.Parameters.AddWithValue("@name",card.CardHolderName);
            cmd.Parameters.AddWithValue("@number",Convert.ToInt32(card.CardNumber));
            cmd.Parameters.AddWithValue("@amount",amount);
            SqlCommand cmdUpdatePay = new SqlCommand("UPDATE STUDENT SET PAYMENTSTATUS = 'PAID' WHERE STUDENTID = @id",conn);
            cmdUpdatePay.Parameters.AddWithValue("@id", id);
            try
            {
                conn.Open();
                await cmd.TryExecuteNonQueryAsync();
                await cmdUpdatePay.TryExecuteNonQueryAsync();
            }
            catch (SqlException ex) { Console.WriteLine(ex.Message); }
            finally
            {
                conn.Close();
            }
        }
        return TransId;
    }

    public static async Task<bool> CheckIfPaid(int id)
    {
        using(SqlConnection conn  = new SqlConnection(Helper.ConnStr))
        {
            SqlCommand cmdifpaid = new SqlCommand("SELECT PAYMENTSTATUS FROM STUDENT where StudentId = @id",conn);
            cmdifpaid.Parameters.AddWithValue("@id", id);
            try
            {
                conn.Open();
                SqlDataReader reader = await cmdifpaid.TryExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["PAYMENTSTATUS"].ToString().ToUpper() == "PAID") return true;
                }
                reader.Close();
            }
            catch (SqlException ex) { Console.WriteLine(ex.Message); }
            finally {conn.Close(); }
        }
        return false;
    }
}