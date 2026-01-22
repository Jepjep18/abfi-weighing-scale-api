//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using abfi_weighing_scale_api.Models.Entities;

//namespace abfi_weighing_scale_api.Data.Configurations
//{
//    public class UserConfiguration : IEntityTypeConfiguration<User>
//    {
//        public void Configure(EntityTypeBuilder<User> builder)
//        {
//            builder.Property(u => u.Email)
//                   .IsRequired()
//                   .HasMaxLength(150);

//            builder.Property(u => u.FullName)
//                   .IsRequired()
//                   .HasMaxLength(150);

//            builder.Property(u => u.PasswordHash)
//                   .IsRequired();
//        }
//    }
//}
