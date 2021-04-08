using DevCars.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DevCars.API.Persistence
{
    public class DevCarsDbContext : DbContext
    {
        public DevCarsDbContext(DbContextOptions<DevCarsDbContext> options) : base(options)
        {

        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ExtraOrderItem> ExtraOrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder model)
        {

            //Metodo 1 chamar as classes de persistencia dentro de configuration para que cada entidade fiquem dentro de suas proprias configurações
            //model.ApplyConfiguration(new CarDbConfiguration());
            //model.ApplyConfiguration(new CustomerDbConfiguration());
            //model.ApplyConfiguration(new OrderDbConfiguration());
            //model.ApplyConfiguration(new ExtraOrderItemDbConfiguration());

            //Metodo 2
            model.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
