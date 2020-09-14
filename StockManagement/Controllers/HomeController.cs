using StockManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace StockManagement.Controllers
{
    public class HomeController : Controller
    {

        #region private memebers

        private Repository _repository;

        public Repository Repository
        {
            get { return _repository ?? (_repository = new Repository()); }
            set { _repository = value; }
        }

        #endregion


        #region home page actions

        public ActionResult Index()
        {
            CategoryList model = new CategoryList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CategoryList model, HttpPostedFileBase fileBase, string command)
        {
            if (command == "Import")
            {
                DataSet ds = readExcel(fileBase);
                model.Categories = createModel(model, ds);
            }
            else
            {
                model.Categories = (List<Category>)Session["myExcelData"];
                addToCategory(model);
            }
            return View(model);
        }

        private List<Category> createModel(CategoryList model, DataSet ds)
        {
            List<Category> categories = new List<Category>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                categories.Add(new Category()
                {
                    Id = Convert.ToInt32(ds.Tables[0].Rows[i][0]),
                    Name = ds.Tables[0].Rows[i][1].ToString(),
                    Email = ds.Tables[0].Rows[i][2].ToString(),
                    Type = ds.Tables[0].Rows[i][3].ToString()
                });
            }
            Session.Add("myExcelData", categories);
            return categories;
        }

        #endregion


        #region treeviewData populating

        [HttpGet]
        public JsonResult getTreeViewData()
        {
            Rootobject treeData = null;
            if (Session["myExcelData"] != null)
            {
                List<Category> categories = null;
                bool isSavedToDB = Session["saveToDB"] != null && (bool)Session["saveToDB"];
                if (isSavedToDB)
                {
                    categories = Repository.GetAllCategories();
                }
                else
                {
                    categories = (List<Category>)Session["myExcelData"];
                }

                List<Category> swCategories = categories.Where(c => c.Type == "SW").ToList();
                List<Category> hwCategories = categories.Where(c => c.Type == "HW").ToList();

                treeData = new Rootobject()
                {
                    Property1 = new List<Class1>()
                    { 
                        new Class1()
                        {
                            text = "HardWarwe",
                            expanded = true,
                            items = new List<Item>()
                            {
                             new Item() { id = hwCategories[0].Id, text = hwCategories[0].Name },
                             new Item() { id = hwCategories[1].Id, text = hwCategories[1].Name }
                            }
                        },
                        new Class1()
                        {
                            text = "Software",
                            expanded = true,
                            items = new List<Item>()
                            {
                             new Item() { id = swCategories[0].Id, text = swCategories[0].Name },
                             new Item() { id = swCategories[1].Id, text = swCategories[1].Name }
                            }
                        }
                    }
                };
            }
            else
            {
                treeData = new Rootobject()
                {
                    Property1 = new List<Class1>()
                {
                    new Class1()
                    {
                        text = "HardWarwe"
                    },
                    new Class1()
                    {
                        text = "Software"
                    }
                }
                };
            }
            

           
            return this.Json(treeData.Property1, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region db operations

        public void addToCategory(CategoryList model)
        {
            Repository.DeleteAllCategory();
            foreach (var category in model.Categories)
            {
                Repository.AddCategory(category);
            }
            Session.Add("saveToDB", true);
        }

        [HttpPost]
        public JsonResult updateDB(string json)
        {
            if (json != null)
            {
                string name = string.Empty;
                int id = 0;
                Repository.UpdateCategory(id,name);
            }
            return this.Json("Ok");
        }

        #endregion

        #region helpers

        public DataSet readExcel(HttpPostedFileBase file)
        {
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {

                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                    excelConnection.Close();
                }
                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    // DataSet ds = new DataSet();
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }
            }
            return ds;
        }

        #endregion

    }
}