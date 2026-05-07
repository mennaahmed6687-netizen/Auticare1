using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class AddAttackDto
    {
        public int ChildId { get; set; }

        public string Location { get; set; }
        public string Trigger { get; set; }
        public int Intensity { get; set; }
        public string Duration { get; set; }

        public bool Crying { get; set; }
        public bool Screaming { get; set; }
        public bool ThrowingObjects { get; set; }
        public bool Hitting { get; set; }
        public bool Rocking { get; set; }
        public bool Hiding { get; set; }
    }
}
