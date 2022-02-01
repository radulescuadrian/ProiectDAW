using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectDAW.API.Data;
using ProiectDAW.CommunicationObjects.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProiectDAW.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;

        public CategoryController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _databaseContext.Categories.ToListAsync();

            if(categories.Count == 0)
                return NotFound("No categories found");
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound("Category not found");
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string category)
        {
            var check = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.Name == category);
            
            if (check != null)
                return BadRequest("Category already exists");

            try
            {
                await _databaseContext.Categories.AddAsync(new Category
                {
                    Name = string.Concat(category[0].ToString().ToUpper(), category.AsSpan(1)),
                });

                await _databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }

            return Ok("Category added successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            var category = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound("Category not found");

            try
            {
                category.Name = value;

                await _databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }

            return Ok("Category modified successfully");
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
