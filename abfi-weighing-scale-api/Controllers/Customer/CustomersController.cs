using abfi_weighing_scale_api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace abfi_weighing_scale_api.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.CustomerName))
                {
                    return BadRequest("Customer name is required.");
                }

                var customer = new Models.Entities.Customer
                {
                    CustomerName = dto.CustomerName,
                    CustomerType = dto.CustomerType
                };

                _context.Customer.Add(customer);
                await _context.SaveChangesAsync();

                var result = new CustomerDto
                {
                    Id = customer.Id,
                    CustomerName = customer.CustomerName,
                    CustomerType = customer.CustomerType
                };

                return CreatedAtAction(
                    nameof(GetCustomer),
                    new { id = customer.Id },
                    result
                );
            }
            catch (Exception ex)
            {
                // Log exception here
                return StatusCode(500, "An error occurred while creating the customer.");
            }
        }


        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            try
            {
                var customers = await _context.Customer
                    .Select(c => new CustomerDto
                    {
                        Id = c.Id,
                        CustomerName = c.CustomerName,
                        CustomerType = c.CustomerType,
                    })
                    .ToListAsync();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Log the exception here (using your preferred logging method)
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            try
            {
                var customer = await _context.Customer
                    .Where(c => c.Id == id)
                    .Select(c => new CustomerDto
                    {
                        Id = c.Id,
                        CustomerName = c.CustomerName,
                        CustomerType = c.CustomerType,
                    })
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found.");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Customers/search?name={name}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Search term cannot be empty.");
                }

                var customers = await _context.Customer
                    .Where(c => c.CustomerName.Contains(name))
                    .Select(c => new CustomerDto
                    {
                        Id = c.Id,
                        CustomerName = c.CustomerName,
                        CustomerType = c.CustomerType,
                    })
                    .ToListAsync();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Customers/type/{type}
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomersByType(string type)
        {
            try
            {
                var customers = await _context.Customer
                    .Where(c => c.CustomerType == type)
                    .Select(c => new CustomerDto
                    {
                        Id = c.Id,
                        CustomerName = c.CustomerName,
                        CustomerType = c.CustomerType,
                    })
                    .ToListAsync();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
