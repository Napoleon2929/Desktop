using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desktop.Models;

namespace Desktop
{
    class Search
    {
        private List<EmployeesToView> _foundItems;
        public List<EmployeesToView> FoundItems { get => _foundItems; }
        public Search()
        {
            _foundItems = new List<EmployeesToView>();
        }
        public Search(IEnumerable<EmployeesToView> items)
        {
            _foundItems = new List<EmployeesToView>(items);
        }
    }
}
