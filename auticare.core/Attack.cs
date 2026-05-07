using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    public class Attack
    {
        public int Id { get; set; }

        public string Location { get; set; }
        public string Trigger { get; set; }

        public int Severity { get; set; }
        public string Duration { get; set; }

        public bool Crying { get; set; }
        public bool Screaming { get; set; }
        public bool Throwing { get; set; }
        public bool Hitting { get; set; }
        public bool Swinging { get; set; }
        public bool Hiding { get; set; }

        public DateTime Date { get; set; }

        public int ChildId { get; set; }
        public virtual Childern Childern { get; set; }
        public int Intensity {  get; set; }
        public bool Rocking {  get; set; }
    }
}

