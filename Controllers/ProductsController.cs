using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Emarket.Models;


namespace Emarket.Controllers
{
    public class ProductsController : Controller
    {
        private EmarketDBEntities db = new EmarketDBEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5

        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Product product = db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(product);
        //}

        // GET: Products/Create

        //public ActionResult Create()
        //{
        //    ViewBag.category_id = new SelectList(db.Categories, "id", "name");
        //    return View();
        //}

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,name,price,image,description,category_id")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Products.Add(product);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
        //    return View(product);
        //}


        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        public ActionResult Layout()
        {
            var record = db.Products.ToList();
            return View(record);
        }
        public ActionResult Search(string key)
        {
            //var categoryID = (from p in db.Categories
            //                  where p.name == key
            //                  select new Category { id = p.id });
            var categoryID = (from p in db.Categories
                              where p.name == key
                              select p.id).FirstOrDefault();
           // int parsed = int.Parse(categoryID);
            var listOfProducts = db.Products.Where(x => x.category_id== categoryID).ToList();
            /* */
            //  SqlCommand cmd = new SqlCommand(categoryID);  
            //  int parsed = Convert.ToInt32(categoryID);
            //  var listOfProducts = db.Products.Where(x => x.id == parsed).ToList();
            return View(listOfProducts);
        }
        //public ActionResult Search(string key)
        //{

        //    IEnumerable<Category> rec;
        //    foreach (var item in db.Categories)
        //    {
        //        if (item.name == key)
        //        {
        //             rec = from id in db.Categories
        //                      select id;
        //        }
        //    }
        //    IEnumerable<Product> se = from p in db.Products
        //             where p.category_id == rec
        //             select p;


        //    return View();
        // }
        //    //}
        //[ActionName("Layout")]
        //public ActionResult Search(string key)
        //{
        //    //IEnumerable<Product> selection = from p in db.Products
        //    //                                 where p.Category.name == key
        //    //                                 select p;
        //    //IEnumerable<Category>
        //    var selection = from p in db.Categories
        //                                      where p.name==key
        //                                      select new {p.id};
        //    var bsmallah = from q in db.Products
        //                                    where selection.Equals(q.id)
        //                                    select q;
        //    return View(bsmallah);
        //}


    }
}
