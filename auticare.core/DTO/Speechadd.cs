using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class Speechadd
    {
        public int ChildId { get; set; }
        public int Score { get; set; } = 0;
        public List<LetterItem> Letters { get; set; } = new List<LetterItem>();
        public string Notes { get; set; }
    }
}
