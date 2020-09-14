using System;
using System.Data.Entity;
using System.Web.Security;

namespace StockManagement.Models
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {

            //var defaultCategory = new Category()
            //{
            //    Name = categoryName,
            //    Email = "",
            //    Type = ""
            //};

            //context.Categories.Add(defaultCategory);
            //context.SaveChanges();
            base.Seed(context);
        }
    }
}