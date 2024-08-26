using NguyeThanhTri.Context;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NguyeThanhTri.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WebsiteEcomEntities1 _context;

        public ProductsController()
        {
            _context = new WebsiteEcomEntities1(); // Khởi tạo context
        }

        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            ViewBag.ItemCount = products.Count; // Đếm số lượng sản phẩm
            return View(products);
        }



        public ActionResult Create()
        {
            // Tạo danh sách danh mục với thuộc tính Id và Name
            var categories = _context.Categories.Select(c => new { Id = c.CategoryId, Name = c.Name }).ToList();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");

            var brands = _context.Brands.AsNoTracking().Select(b => new { Id = b.BrandId, Name = b.BrandName }).ToList();
            ViewBag.BrandId = new SelectList(brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Xác định tên tệp và đường dẫn lưu trữ
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/images/pro"), fileName);

                    // Lưu tệp vào thư mục
                    imageFile.SaveAs(filePath);

                    // Cập nhật URL hình ảnh trong đối tượng sản phẩm
                    product.ImageUrl = fileName;
                }

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var categories = _context.Categories.Select(c => new { Id = c.CategoryId, Name = c.Name }).ToList();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", product.CategoryId);

            var brands = _context.Brands.AsNoTracking().Select(b => new { Id = b.BrandId, Name = b.BrandName }).ToList();
            ViewBag.BrandId = new SelectList(brands, "Id", "Name", product.BrandId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm đối tượng Product hiện tại trong cơ sở dữ liệu
                    var productInDb = _context.Products.Find(product.Id);
                    if (productInDb == null)
                    {
                        return HttpNotFound();
                    }

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Xác định tên tệp và đường dẫn lưu trữ
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/images/pro"), fileName);
                        // Kiểm tra và tạo thư mục nếu không tồn tại
                        var directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Lưu tệp vào thư mục
                        imageFile.SaveAs(filePath);

                        // Cập nhật URL hình ảnh trong đối tượng danh mục
                        productInDb.ImageUrl = fileName;
                    }

                    // Cập nhật các thuộc tính khác
                    productInDb.Name = product.Name;

                    // Đánh dấu đối tượng là đã thay đổi
                    _context.Entry(productInDb).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    ModelState.AddModelError("", "Lỗi xảy ra khi lưu tệp ảnh: " + ex.Message);
                }
            }

            return View(product);
        }
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Eager load Product và Brand để đảm bảo chúng không bị null
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .SingleOrDefault(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }



        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = _context.Products.Find(id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
