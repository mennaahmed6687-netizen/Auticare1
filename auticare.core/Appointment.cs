using auticare.core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;



namespace auticare.core
{

    public class Appointment
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsNotified { get; set; }

        public string ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Parent? Parent { get; set; }
    }
}
