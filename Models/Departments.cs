using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Departments
    {
        [Key]
        public int ID { get; set; }

        public string Department { get; set; }

      
    }
}
