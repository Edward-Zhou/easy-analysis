using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.ServiceHost
{
    public class TaskModelTypeConfigration : EntityTypeConfiguration<TaskModel>
    {
        public TaskModelTypeConfigration()
        {
            // MAP TABLE
            ToTable("Tasks");

            // SET PRIMARY KEY
            HasKey(m => m.Id);
            Property(m => m.Id)
                .HasMaxLength(128)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }

    public class TaskDbContext : DbContext
    {
        public DbSet<TaskModel> Tasks { get; set; }

        public TaskDbContext() : base ("EasHubConnection") 
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TaskModelTypeConfigration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
