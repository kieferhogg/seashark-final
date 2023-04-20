using System.Text.Json;
using System.Text.Json.Serialization;
using final_backend.Data;
using final_backend.Hubs;
using final_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace final_backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<UpdatesHub> _hubContext;

    public ProductController(ApplicationDbContext context, IHubContext<UpdatesHub> hubContext)
    {
      _context = context;
      _hubContext = hubContext;
    }

    // GET: api/Product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
      var products = await _context.Products.Include(p => p.Orders).ToListAsync();
      return new JsonResult(products, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
      var product = _context.Products.Find(id);

      if (product == null)
      {
        return NotFound();
      }

      return product;
    }

    // POST: api/Product
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      await _hubContext.Clients.All.SendAsync("ProductChanged", "created", product);

      return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // PUT: api/Product/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
      if (id != product.Id)
      {
        return BadRequest();
      }

      _context.Entry(product).State = EntityState.Modified;
      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException ex)
      {
        if (!_context.Products.Any(p => p.Id == id))
        {
          return NotFound();
        }
        else
        {
          // Add logging to the exception
          Console.WriteLine("Exception while updating product:");
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.StackTrace);

          // Return a generic error message
          return StatusCode(500, "An error occurred while updating the product");
        }
      }

      await _hubContext.Clients.All.SendAsync("ProductChanged", "updated", product);

      return NoContent();
    }

    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
      var product = _context.Products.Find(id);
      if (product == null)
      {
        return NotFound();
      }

      _context.Products.Remove(product);
      await _context.SaveChangesAsync();

      await _hubContext.Clients.All.SendAsync("ProductChanged", "deleted", product);

      return NoContent();
    }
  }
}
