using CineProject.API.Models;
using CineProject.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CineProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _repository.GetAllAsync();
            if(products.Count() > 0)
            {
                return Ok(products);
            }
            else
            {
                return NoContent();
            }        
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NoContent();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Product product)
        {
            await _repository.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            await _repository.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
