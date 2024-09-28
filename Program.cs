
class Program
{
    public async static Task Main(string[] args)
    {
        await PreReq.Requirements();
        Student student = new Student();
        Faculty faculty = new Faculty();
        Course course = new Course();
        
    int choice;
        while (true)
        {
            Menu.Display();
            while (true) {
                Console.WriteLine("Enter Your Choice: ");
                bool valid = int.TryParse(Console.ReadLine(), out choice);
                if (valid) break; 
                else  Console.WriteLine("Invalid Choice"); 
            }
            switch (choice)
            {
                case 1:
                    //Add Student
                    Console.Clear();
                    while (true)
                    {
                        Console.Write("Is the Student FullTime(1) or PartTime(0) : ");
                        string check = Console.ReadLine().Trim();
                        if (check != "1" && check != "0") Console.WriteLine("Invalid Entry..."
                            + check);
                        else
                        {
                            if (check == "1") await student.AddAsync(1);
                            else if (check == "0") await student.AddAsync(0);
                            break;
                        }
                    }
                    break;
                case 2:
                    //Remove student
                    Console.Clear();
                    await student.RemoveAsync();
                    break;
                case 3:
                    Console.Clear();
                    //display student
                    await DBServices.DisplayDBAsync(student);
                    break;
                case 4:
                    Console.Clear();
                    //add course
                    await course.AddAsync();
                    break;
                case 5:
                    Console.Clear();
                    //remove course
                    await course.RemoveAsync();
                    break;
                case 6:
                    Console.Clear();
                    //display courses
                    await DBServices.DisplayDBAsync(course);
                    break;
                case 7:
                    Console.Clear();
                    //add faculty
                    await faculty.AddAsync();
                    break;
                case 8:
                    Console.Clear();
                    //remove faculty
                    await faculty.RemoveAsync();
                    break;
                case 9:
                    Console.Clear();
                    //display faculty
                    await DBServices.DisplayDBAsync(faculty);
                    break;
                case 10:
                    //calculate fees
                    int id;
                    while (true)
                    {
                        Console.WriteLine("Enter the studentID to calculate Fees: ");
                        bool valid = int.TryParse(Console.ReadLine().Trim(), out id);
                        if (valid) { break; }
                        else Console.WriteLine("Invalid StudentId");
                    }
                    int b = await student.CalculateFeesAsync(id);
                    if (b == 1)
                    {
                        Student f = new FullTimeStudent();
                        await f.CalculateFeesAsync(id);
                    }
                    else if(b == 0)
                    {
                        Student p = new PartTimeStudent();
                        await p.CalculateFeesAsync(id);
                    }
                    break;
                case 11:
                    while (true)
                    {
                        Console.WriteLine("Enter the studentID to calculate Fees: ");
                        bool valid = int.TryParse(Console.ReadLine().Trim(), out id);
                        if (valid) { break; }
                        else Console.WriteLine("Invalid StudentId");
                    }
                    int b1 = await student.CalculateFeesAsync(id);
                    if (b1 == 1)
                    {
                        FullTimeStudent f = new FullTimeStudent();
                        await f.PayFees(id);
                    }
                    else if(b1 == 0)
                    {
                        PartTimeStudent p = new PartTimeStudent();
                        await p.PayFees(id);
                    }
                    break;
                case 12:
                    Console.Clear();
                    //exit
                    Console.WriteLine("You have successfully exited...");
                    return;
                case 707070:
                    await PreReq.DefaultValues(); //Adding Default Values
                    Console.WriteLine("Added Default Values");
                    break;
                case 717171:
                    await PreReq.DropValues(); //Dropping All VAlues
                    return;
                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        }
    }
}
