using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    public class SpeechData
    {
        public int Id { get; set; }

        // 🔗 ربط بالطفل
        public int ChildId { get; set; }


        // 🎯 السكور (يتخزن في الداتابيز)
        public int Score { get; set; } = 0;

        // 🔤 الحروف
       

        // ⏱️ تاريخ الجلسة
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Notes { get; set; }
    }
}
