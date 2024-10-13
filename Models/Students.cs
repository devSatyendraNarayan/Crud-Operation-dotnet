using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Students
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="Required*")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Required*")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage ="Required*")]
       
        public int DepID { get; set; }

        [Required(ErrorMessage ="Required*")]
        public string Mobile {  get; set; }

        public string Description { get; set; }

        public string Department { get; set; }

    }
}
