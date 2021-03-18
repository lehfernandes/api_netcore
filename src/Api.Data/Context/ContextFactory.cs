using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Data.Context
{
    public class ContextFactory : IDesignTimeDbContextFactory<MyContext>
    {
        public MyContext CreateDbContext(string[] args)
        {
            var connetionString = "Server=localhost;Port=3306;Database=dbAPI;Uid=root;Pwd=123456";
            var optionBuilder = new DbContextOptionsBuilder<MyContext>();
            optionBuilder.UseMySql(connetionString, b => b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));            
            return new MyContext(optionBuilder.Options);
        }
    }
}
