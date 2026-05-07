using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum ResponseLevel
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3
}


public enum InteractionLevel
{
    High,
    Medium,
    Low
}

namespace auticare.core
{
    public class EmotionSession
    {
       
            public int Id { get; set; }

            public string Notes { get; set; }

            // 👈 هيتخزن بالعربي
            public ResponseLevel ResponseLevel { get; set; }

            public string Emotion { get; set; }

            public int Score { get; set; }

            public int ChildId { get; set; }
            public virtual Childern Child { get; set; }

            public DateTime Date { get; set; } = DateTime.Now;

            // 👇 الاختيارات (ممكن تخزينها كنص)
            public string Options { get; set; }
        public InteractionLevel InteractionLevel {  get; set; }

    }
}
