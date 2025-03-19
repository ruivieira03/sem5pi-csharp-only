using Hospital.Domain.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Logs
{
    public class ProfileUpdateLogEntityTypeConfiguration : IEntityTypeConfiguration<ProfileUpdateLog>
    {
        public void Configure(EntityTypeBuilder<ProfileUpdateLog> builder)
        {
            builder.ToTable("ProfileUpdateLogs");
            builder.HasKey(log => log.Id);
            builder.Property(log => log.UserId)
                .IsRequired(); // Should be required for tracking purposes
            builder.Property(log => log.ChangedFields)
                .IsRequired(); // Assuming this should be required
            builder.Property(log => log.Timestamp)
                .IsRequired(); // Assuming this should be required
        }
    }

}
