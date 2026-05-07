using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class CreateAudioSessionDto
    {
        public int?ChildId;
        public string?Notes { get; set; }
        [Required]
        public ResponseSpeed ResponseSpeed { get; set; }
        [Range(0, 100)]
        public int Score { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
