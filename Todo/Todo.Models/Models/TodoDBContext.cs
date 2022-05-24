using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Models
{
    #region Commands
    //add-migration(dotnet ef migrations) initialcreate
    //update-database
    //add-migration CreateTable_Users
    //add-migration DropColumn1AndColumn2_Users(remove column)
    //Remove-migration(to remove migration which is not updated)
    //update-database initialcreate (to revert migration)
    //script-migration
    #endregion
    public class TodoDBContext:DbContext
    {
        public TodoDBContext(DbContextOptions<TodoDBContext> dbContextOptions):base(dbContextOptions)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("");
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
