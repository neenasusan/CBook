using CBook.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;




namespace CBook.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetData()
        {
          using(CBookEntitiesBooks db = new CBookEntitiesBooks())
          {
            List<Book> bookList = db.Books.ToList<Book>();
            return Json(new { data = bookList }, JsonRequestBehavior.AllowGet);
          }
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id=0)
        {
          if(id== 0)
          return View(new Book());
          else
          {
           using (CBookEntitiesBooks db = new CBookEntitiesBooks())
            {
            return View(db.Books.Where(x => x.BookID == id).FirstOrDefault< Book>());
            }
          }
        }
        [HttpPost]
        public ActionResult AddOrEdit(Book bk)
        {
      using (CBookEntitiesBooks db = new CBookEntitiesBooks())
         {
        if (bk.BookID == 0)
        {
          var isExist = IsBNameExist(bk.BName);
          if (isExist)
         {
           ModelState.AddModelError("BExist", "Book already Exist");
           return View(bk);
         }
          else
          {
            db.Books.Add(bk);
            db.SaveChanges();
            return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
          }
          
        }
        else
        {
          db.Entry(bk).State = EntityState.Modified;
          db.SaveChanges();
          return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }
        }
   
        }
    [HttpPost]
    public ActionResult Delete(int id)
    {
      using (CBookEntitiesBooks db = new CBookEntitiesBooks())
      {
        Book bk = db.Books.Where(x => x.BookID == id).FirstOrDefault<Book>();
        db.Books.Remove(bk);
        db.SaveChanges();
        return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
      }
    }

    [NonAction]
    public bool IsBNameExist(string bookname)
    {
      using (CBookEntitiesBooks db = new CBookEntitiesBooks())
      {
        var v = db.Books.Where(a => a.BName == bookname).FirstOrDefault();
        return v != null;
      }

    }




  }
}
