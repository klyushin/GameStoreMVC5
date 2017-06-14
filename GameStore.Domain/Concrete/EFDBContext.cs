using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStore.Domain.Entities;

namespace GameStore.Domain.Concrete
{
    public class EFDBContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
    }
}
