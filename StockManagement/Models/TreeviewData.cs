using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockManagement.Models
{
    public class Rootobject
    {
        public List<Class1> Property1 { get; set; }
    }

    public class Class1
    {
        public string text { get; set; }
        public bool expanded { get; set; }
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string text { get; set; }
    }


}