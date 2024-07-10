using Microsoft.EntityFrameworkCore;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Context
{
    public class ShahrChapContext:DbContext
    {
        public ShahrChapContext(DbContextOptions<ShahrChapContext> options) : base(options)
        {
            
        }

        #region User
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        #endregion
    }
}
