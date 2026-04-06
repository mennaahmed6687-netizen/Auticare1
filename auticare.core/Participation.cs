using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    public class Participation
    {

        public int Id { get; set; }
        public int ChildId { get; set; }
        public required string Status { get; set; }
    }

}