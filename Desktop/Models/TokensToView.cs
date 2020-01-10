using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Models
{
    public class TokensToView
    {
        public string Token { get; set; }
        public DateTime? Creation_Time { get; set; }
        public DateTime? End_time { get; set; }
        public override string ToString()
        {
            return $"Created {Creation_Time}\nWill end {End_time}";
        }
    }
}
