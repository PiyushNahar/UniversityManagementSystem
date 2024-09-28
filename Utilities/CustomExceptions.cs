using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Utilities
{
    class StudentNotFoundException : Exception
    {
        public StudentNotFoundException(string message) : base(message) { }
    }

    class FacultyNotFoundException : Exception
    {
        public FacultyNotFoundException(string message) : base(message) { }
    }

    class CourseNotFoundException : Exception
    {
        public CourseNotFoundException(string message) : base(message) { }
    }
}
