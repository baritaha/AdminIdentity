using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Tasks.Data
{
    public class TasksDBContext:IdentityDbContext
    {
        public TasksDBContext(DbContextOptions<TasksDBContext>options):base(options)
        {

        }
    }
}
