using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{

    // Representação da base de dados que herda da classe DbContext
    public class DataContext : DbContext
    { 
        public DataContext(DbContextOptions<DataContext>options)
        : base(options) 
        {
            
        }

        // Representação das tabelas na base de dados
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }

        
    
    
    
    }

}