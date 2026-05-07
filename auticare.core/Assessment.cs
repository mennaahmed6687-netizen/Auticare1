using System.ComponentModel.DataAnnotations;

namespace auticare.core
{
    public class Assessment
    {
        [Key]
       public int assessment_id { get; set; }
        

        [MaxLength(50)]
        public required string name { get; set; }
        [Required]
        public int score { get; set; }
        [MaxLength(100)]
        public string? notes { get; set; }
        [Required]
        [MaxLength(100)]
        public string ?category { get; set; }
        [Required]
        public DateTime date { get; set; } = DateTime.Now;
        public int ChildId {  get; set; } //foreginkey
        public virtual Childern ?Child { get; set; }


    }
}
