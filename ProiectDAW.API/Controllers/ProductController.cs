using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectDAW.API.Data;
using ProiectDAW.CommunicationObjects.Models.DTOs;
using ProiectDAW.CommunicationObjects.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProiectDAW.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;

        public ProductController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var products = await _databaseContext.Products.ToListAsync();

            if (products.Count == 0)
                return NotFound("No products found");
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _databaseContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound("Product not found");
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDTO product)
        {
            var categories = await _databaseContext.Categories.ToListAsync();
            if (categories.Count == 0)
                return BadRequest("Add a category first");

            var check = await _databaseContext.Products.FirstOrDefaultAsync(x => x.Title == product.Title);
            if (check != null)
                return BadRequest("Product already exists");

            try
            {
                var category = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.Id == product.Category.Id);
                await _databaseContext.Products.AddAsync(new Product
                {
                    Title = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    Image = product.Image,
                    Category = category
                });

                await _databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }

            return Ok("Product added successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDTO sentProduct)
        {
            var product = await _databaseContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return BadRequest("Product not found");

            try
            {
                var category = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.Id == sentProduct.Category.Id);
                if (category == null)
                    return BadRequest("Category not found");

                product.Title = sentProduct.Title;
                product.Description = sentProduct.Description;
                product.Price = sentProduct.Price;
                product.Image = sentProduct.Image;
                product.Category = category;

                await _databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }

            return Ok("Product modified successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound("Category not found");

            try
            {
                _databaseContext.Categories.Remove(category);

                await _databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }

            return Ok("Category removed successfully");
        }
    }
}
