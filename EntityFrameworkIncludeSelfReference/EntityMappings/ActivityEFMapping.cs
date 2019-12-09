using EntityFrameworkIncludeSelfReference.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EntityFrameworkIncludeSelfReference.EntityMappings
{
    public class ActivityEFMapping : EntityTypeConfiguration<Activity>
    {
        public ActivityEFMapping()
        {
            ToTable(nameof(Activity), "public");

            HasKey(c => c.Id);

            Property(p => p.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName(nameof(Activity.Id));

            HasMany(s => s.ChildActivities);

            HasMany(s => s.Documents);
        }
    }
}