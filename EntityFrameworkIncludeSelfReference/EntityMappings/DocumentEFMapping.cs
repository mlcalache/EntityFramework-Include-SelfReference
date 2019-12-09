using EntityFrameworkIncludeSelfReference.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EntityFrameworkIncludeSelfReference.EntityMappings
{
    public class DocumentEFMapping : EntityTypeConfiguration<Document>
    {
        public DocumentEFMapping()
        {
            ToTable(nameof(Document), "public");

            HasKey(c => c.Id);

            Property(p => p.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName(nameof(Document.Id));
        }
    }
}