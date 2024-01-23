using Microsoft.EntityFrameworkCore;
using ModelDataLibrary.Models;
namespace DatabaseContextLibrary.DatabaseContext
{
    public class DatabaseContextInfo : DbContext
    {
        public DatabaseContextInfo(DbContextOptions<DatabaseContextInfo> options) : base(options)
        {
        }

        public DbSet<UserToken> UserTokens { get; set; }
    }
}
