using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using ProductManagement.Models;
using System.Threading.Tasks;
using System.Linq;

[Authorize(Roles = "Admin,Manager")]
public class ProductController : Controller
{
    private readonly ProductContext _context;

    public ProductController(ProductContext context)
    {
        _context = context;
    }

    // GET: /Product
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }

    // GET: Create (Admin only)
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Product \"{product.Name}\" has been successfully created!";
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Edit (Admin, Manager)
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Product \"{product.Name}\" has been successfully updated!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                else throw;
            }
        }
        return View(product);
    }

    // GET: Delete (Admin only)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        if (product == null) return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Product \"{product.Name}\" has been successfully deleted!";
        }
        return RedirectToAction(nameof(Index));
    }
}
