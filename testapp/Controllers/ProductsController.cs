using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using testapp.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace testapp.Views
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ProductsController()
        {
        }

        public ProductsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _userManager = userManager;
        }
        

        // GET: Products
        public async Task<ActionResult> Index()
        {
            var products = await db.Products.ToListAsync();
            var permission = new Permission();
            if (User.Identity.IsAuthenticated) {
                var groups = new[] { "Administrator", "Archivarius" };
                permission = Permission.GetCurrentUserPermissions(User.Identity.Name, groups);
            }
            return View(Tuple.Create(products, permission));
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.Where(p=> p.Id == id).Include(p => p.User).FirstOrDefaultAsync();
            product.UserFullName = product.User.GetFullName(product.User);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Count,Created,Modify")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.UserId = ApplicationUser.GetCurrentUser(User.Identity.Name).Id;
                product.Created = DateTime.Now;
                product.Modify = DateTime.Now;
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }
        
        [Authorize]
        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.Where(p => p.Id == id).Include(p => p.User).FirstOrDefaultAsync();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Count,Created,Modify")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.UserId = ApplicationUser.GetCurrentUser(User.Identity.Name).Id;
                product.Modify = DateTime.Now;
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult ExportToExcel()
        {
            var dbproducts = db.Products.ToList();
            var products = new System.Data.DataTable("testexcell");
            products.Columns.Add("№", typeof(int));
            products.Columns.Add("Наименование товара", typeof(string));
            products.Columns.Add("Количество", typeof(int));
            products.Columns.Add("Описание", typeof(string));
            products.Columns.Add("Дата изменений", typeof(DateTime));

            var i = 0;
            foreach (var item in dbproducts)
            {
                i++;
                products.Rows.Add(i, item.Name, item.Count, item.Description, item.Modify);
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=TestExcelFile.xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
