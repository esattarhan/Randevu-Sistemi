using Microsoft.AspNetCore.Mvc;
using WebApplication4.Models;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication4.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, ProductName = "Terlik", Quantity = 10, Category = "Giyim" },
            new Product { Id = 2, ProductName = "Ayakkabı", Quantity = 5, Category = "Giyim" },
            new Product { Id = 3, ProductName = "Çizme", Quantity = 3, Category = "Giyim" }
        };

        private static int NextId => _products.Any() ? _products.Max(p => p.Id) + 1 : 1;

        public IActionResult GetProducts()
        {
            return View(_products);
        }

        public IActionResult CreateProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            if (!ModelState.IsValid) return View(product);
            product.Id = NextId;
            _products.Add(product);
            return RedirectToAction("GetProducts");
        }

        public IActionResult Edit(int id)
        {
            var p = _products.FirstOrDefault(x => x.Id == id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            var existing = _products.FirstOrDefault(x => x.Id == product.Id);
            if (existing == null) return NotFound();
            existing.ProductName = product.ProductName;
            existing.Quantity = product.Quantity;
            existing.Category = product.Category;
            return RedirectToAction("GetProducts");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var existing = _products.FirstOrDefault(x => x.Id == id);
            if (existing != null) _products.Remove(existing);
            return RedirectToAction("GetProducts");
        }
    }
}
