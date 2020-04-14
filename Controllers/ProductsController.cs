using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        public ActionResult MoreInfo(int id)
        {
            Product P = new Product();

            using (db)
            {
				P = db.Products.Include(x => x.Category).SingleOrDefault(p => p.id == id);
            }
            return View(P);
        }
        public ActionResult Edit(int? id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            SelectListItem item1 = new SelectListItem()
            {
                Text = "Mobiles",
                Value = "1",
                Selected = true
            };
            SelectListItem item2 = new SelectListItem()
            {
                Text = "Laptops",
                Value = "3",
                Selected = true
            };
            SelectListItem item3 = new SelectListItem()
            {
                Text = "Mobiles",
                Value = "2",
                Selected = false
            };
            SelectListItem item4 = new SelectListItem()
            {
                Text = "Electrical Devices",
                Value = "15",
                Selected = false
            };
            items.Add(item1);
            items.Add(item2);
            items.Add(item3);
            items.Add(item4);
            ViewBag.category = items;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            Session["imgPath"] = product.image;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, [Bind(Include = "image,name,price,description,category")]Product product)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {

                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssff") + filename;
                    string extension = Path.GetExtension(file.FileName);

                    string path = Path.Combine(Server.MapPath("~/photos/"), _filename);
                    product.image = "~/photos/" + _filename;
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (file.ContentLength <= 100000)
                        {
                            string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                            db.Entry(product).State = EntityState.Modified;

                            if (db.SaveChanges() > 0)
                            {
                                file.SaveAs(path);
                                if (System.IO.File.Exists(oldImgPath))
                                {
                                    System.IO.File.Delete(oldImgPath);
                                }
                                TempData["msg"] = "Data Updated";
                                db.SaveChanges();
                                return RedirectToAction("MoreInfo");

                            }
                        }


                        else
                        {
                            ViewBag.msg = "File size must be equal or less than 1mb ";
                        }

                    }
                    else
                    {
                        ViewBag.msg = "Invalid file type";
                    }
                }

                else
                {
                    product.image = Session["imgPath"].ToString();
                    db.Entry(product).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["msg"] = "Data Updated";
                        db.SaveChanges();
                        return RedirectToAction("MoreInfo");
                    }
                }
            }
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
            var item = db.Products.Where(x => x.id == id).First();
            var item2 = db.Categories.Find(item.category_id);
            item2.number_of_products--;
            db.Entry(item2).State = EntityState.Modified;
            db.Products.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Layout");
        }
    }

}

        
        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
      //  [HttpPost]

      //  [ValidateAntiForgeryToken]
      //  public ActionResult Edit([Bind(Include = "image,name,price,description,category")] Product product)
       // {
        //    if (ModelState.IsValid)
          //  {
             //   db.Entry(product).State = EntityState.Modified;
             //   db.SaveChanges();
             //   return RedirectToAction("MoreInfo");
         //   }
         //   return View(product);
       // }
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


