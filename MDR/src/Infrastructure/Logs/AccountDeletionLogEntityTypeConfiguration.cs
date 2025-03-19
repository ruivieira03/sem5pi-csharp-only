using Hospital.Domain.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Logs
{
    public class AccountDeletionLogEntityTypeConfiguration : IEntityTypeConfiguration<AccountDeletionLog>
    {
        public void Configure(EntityTypeBuilder<AccountDeletionLog> builder)
        {
            builder.ToTable("AccountDeletionLogs");
            builder.HasKey(log => log.Id);
            builder.Property(log => log.UserId)
                .IsRequired(); // Should be required for tracking purposes
            builder.Property(log => log.Timestamp)
                .IsRequired(); // Should be required to know when the deletion occurred
        }
    }

}
