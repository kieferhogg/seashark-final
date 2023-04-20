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
  public class OrderController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<UpdatesHub> _hubContext;

    public OrderController(ApplicationDbContext context, IHubContext<UpdatesHub> hubContext)
    {
      _context = context;
      _hubContext = hubContext;
    }

    // GET: api/Order
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
      var orders = await _context.Orders.Include(o => o.Product).ToListAsync();
      return new JsonResult(orders, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }

    // GET: api/Order/5
    [HttpGet("{id}")]
    public ActionResult<Order> GetOrder(int id)
    {
      var order = _context.Orders.Find(id);

      if (order == null)
      {
        return NotFound();
      }

      return order;
    }

    // PUT: api/Order/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order order)
    {
      if (id != order.Id)
      {
        return BadRequest();
      }

      _context.Entry((object)order).State = EntityState.Modified;
      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException ex)
      {
        if (!_context.Orders.Any(o => o.Id == id))
        {
          return NotFound();
        }
        else
        {
          Console.WriteLine("Exception while updating order:");
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.StackTrace);

          // Return a generic error message
          return StatusCode(500, "An error occurred while updating the order");
        }
      }
      await _hubContext.Clients.All.SendAsync("OrderChanged", "updated", order);
      return NoContent();
    }

    // POST: api/Order
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
      Console.WriteLine("Received order data: " + JsonSerializer.Serialize(order));
      Console.WriteLine(JsonSerializer.Serialize(order.Id));
      Console.WriteLine(JsonSerializer.Serialize(order.ProductId));
      Console.WriteLine(JsonSerializer.Serialize(order.Quantity));
      _context.Orders.Add(order);
      await _context.SaveChangesAsync();

      Console.WriteLine("Saved order data: " + JsonSerializer.Serialize(order));

      await _hubContext.Clients.All.SendAsync("OrderChanged", "created", order);

      return CreatedAtAction("GetOrder", new { id = order.Id }, order);
    }

    // DELETE: api/Order/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
      var order = _context.Orders.Find(id);
      if (order == null)
      {
        return NotFound();
      }
      _context.Orders.Remove(order);
      await _context.SaveChangesAsync();

      await _hubContext.Clients.All.SendAsync("OrderChanged", "deleted", order);

      return NoContent();
    }
  }
}
