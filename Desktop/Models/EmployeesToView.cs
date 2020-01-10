using System;


namespace Desktop.Models
{
    public class EmployeesToView
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Security { get; set; }
        public string Position { get; set; }
        public DateTime? Hiring_Time { get; set; }
        public Status Status { get; set; }
        public override string ToString()
        {
            /*string status;
            switch (Status)
            {
                case Status.Indefined:
                    status = "Indefined"
            }*/
                
            return $"{Status}\n{Login}\n{Name} {Surname}\n{Birthday}\n{Security}\n{Position}\n{Hiring_Time}\n";
        }
    }
}