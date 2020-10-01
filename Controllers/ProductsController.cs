using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
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
        public ProductsController()
        {
            ViewBag.ListCart = db.Carts.ToList();
        }
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Layout()
        {
            EmarketDBEntities emarketDB = new EmarketDBEntities();
            var categorylist = emarketDB.Categories.ToList();
            SelectList list = new SelectList(categorylist, "id", "name");
            ViewBag.CategoryList = list;
			var record = db.Products.ToList();
			return View(record);
        }
        public ActionResult Search(string key)
        {
            EmarketDBEntities emarketDB = new EmarketDBEntities();
            var categorylist = emarketDB.Categories.ToList();
            SelectList list = new SelectList(categorylist, "id", "name");
            ViewBag.CategoryList = list;
            //var categoryID = (from p in db.Categories
            //                  where p.name == key
            //                  select new Category { id = p.id });
            var categoryID = (from p in db.Categories
                              where p.name == key
                              select p.id).FirstOrDefault();
           // int parsed = int.Parse(categoryID);
            var listOfProducts = db.Products.Where(x => x.category_id== categoryID).ToList();
            //  SqlCommand cmd = new SqlCommand(categoryID);  
            //  int parsed = Convert.ToInt32(categoryID);
            //  var listOfProducts = db.Products.Where(x => x.id == parsed).ToList();
            return View(listOfProducts);
        }
        public ActionResult MoreInfo(int id)
        {
            Product P = new Product();

            using (db)
            {
				P = db.Products.Include(x => x.Category).SingleOrDefault(p => p.id == id);
            }
            return View(P);
        }

        // GET: Productscopy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
            return View(product);
        }

        // POST: Productscopy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,price,image,description,category_id")] Product product,HttpPostedFileBase ImageFile)
        {

  
                    if (ModelState.IsValid)
                    {
                        string path = "";
                        if (ImageFile.FileName.Length > 0)
                        {
                            path = "~/Photos/" + Path.GetFileName(ImageFile.FileName);
                            ImageFile.SaveAs(Server.MapPath(path));
                        }
                        product.image = path;

                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Layout");
                    }
                

            

            ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
            return View(product);
        }
       
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.category_id = new SelectList(db.Categories, "id", "name");
            return View();
        }
        [HttpPost]
        public ActionResult Add(Product product)
        {        
            if (product.ImageFile == null)
            {
                ViewBag.error = "This is Required";
            }
            else
            {
                string filename = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                string Extention = Path.GetExtension(product.ImageFile.FileName);
                filename += Extention;
                product.image = "~/photos/" + filename;
                filename = Path.Combine(Server.MapPath("~/photos/"), filename);
                if (Extention.ToLower() == ".jpg" || Extention.ToLower() == ".png" || Extention.ToLower() == ".jpeg")
                {
                    if (ModelState.IsValid)
                    {
                        product.ImageFile.SaveAs(filename);
                        db.Products.Add(product);
                        var item2 = db.Categories.Find(product.category_id);
                        item2.number_of_products++;
                        db.Entry(item2).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Layout");
                    }
                }
                else
                {
                    ViewBag.msg = "Invalid File Type";
                }
            }
            ModelState.Clear();
            ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
            return View();
        }
        public ActionResult Delete(int id)
        {
			var item = db.Products.Where(X => X.id == id).First();
            var item2 = db.Categories.Find(item.category_id);
            item2.number_of_products--;
            db.Entry(item2).State = EntityState.Modified;
			var x = db.Carts.ToList();
            db.SaveChanges();
            return RedirectToAction("Layout");
        }
		public ActionResult Removefromcart (int productid)
		{
			var item = db.Carts.Where(x => x.product_id == productid).First();
			db.Entry(item).State = EntityState.Modified;
			db.Carts.Remove(item);
			db.SaveChanges();
			return RedirectToAction("Layout");
		}


		public ActionResult addtocart(int product_id)
		{
			
				var car = new Cart();
				
				var product = db.Products.Find(product_id);
				
				car.added_at = DateTime.Now;
				car.Product = product;
				car.product_id = product_id;

				db.Carts.Add(car);
				db.SaveChanges();
				
			return Redirect("layout");
		}
	}

}

        

 //   ------   ---------------------------------------------------------------------------------------------------
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


