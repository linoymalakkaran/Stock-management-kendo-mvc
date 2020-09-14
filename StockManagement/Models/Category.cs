using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
    public class Category
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Type { get; set; }
    }
}