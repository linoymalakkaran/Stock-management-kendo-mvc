using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;

namespace StockManagement.Models
{
    public class Repository
    {

        public Category AddCategory(Category category)
        {
            using (var databaseContext = new DatabaseContext())
            {
                databaseContext.Categories.Add(category);
                databaseContext.SaveChanges();

                return category;
            }
        }

        public List<Category> GetAllCategories()
        {
            using (var databaseContext = new DatabaseContext())
            {
                return databaseContext.Categories.ToList();
            }
        }

        public Category DeleteCategory(int id)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var category = databaseContext.Categories.First(x => x.Id == id);
                databaseContext.Categories.Remove(category);
                databaseContext.SaveChanges();

                return category;
            }
        }

        public void DeleteAllCategory()
        {
            using (var databaseContext = new DatabaseContext())
            {
                var categories = databaseContext.Categories.ToList();
                categories.ForEach(c => databaseContext.Categories.Remove(c));
                
                databaseContext.SaveChanges();
            }
        }



        public Category UpdateCategory(int id, string categoryName)
        {
            using (var dataContext = new DatabaseContext())
            {
                if (dataContext.Categories.Any(x => x.Name == categoryName))
                {
                    throw new Exception("Category name already exists");
                }

                var category = dataContext.Categories.First(x => x.Id == id);
                category.Name = categoryName;
                dataContext.SaveChanges();

                return category;
            }
        }
    }
}