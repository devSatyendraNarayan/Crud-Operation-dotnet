using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class StudentContexts:DbContext

    {
        public StudentContexts(DbContextOptions<StudentContexts> options):base(options) 
        { 
        
        }

        public DbSet<Students> tbl_student {  get; set; }
        public DbSet<Departments> tbl_departments { get; set; }
    }
}
