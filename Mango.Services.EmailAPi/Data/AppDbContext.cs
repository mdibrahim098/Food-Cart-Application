using Mango.Services.EmailAPi.Models;
using Microsoft.EntityFrameworkCore;
namespace Mango.Services.EmailAPi.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }
   

      }

 }
