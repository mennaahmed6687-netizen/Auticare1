using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class scoredto
    {
        
            public int Id { get; set; }
            public int Value { get; set; }

            public int ChildId { get; set; }
            public Childern Child { get; set; }
        
    }
}
