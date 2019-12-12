using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Identity.Data;
using WebAPI_Identity.Models;

namespace WebAPI_Identity.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            this._context = context;
        }
            
        // GET: api/Products
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            var products = _context.products;

            return products;
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(int id)
        {
            var prod = _context.products.Find(id);
            if (prod != null)
                return new JsonResult(prod);
            else
                return NotFound();
        }

        // POST: api/Products
        [HttpPost]
        public Product Post([FromBody] Product value)
        {
            _context.Add(value);
            _context.SaveChanges();

            return value;
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Product value)
        {
            var prod = _context.products.AsNoTracking().Where(p => p.Id == id);

            if (prod != null)
            {
                _context.Update(value);
                _context.SaveChanges();
                return new JsonResult(prod);    // I return the old product values (before the update)
            } else
            {
                return NotFound();
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var prod = _context.products.Find(id);
            if(prod!=null) { 
                _context.Remove(prod);
                _context.SaveChanges();
                return new JsonResult(prod);    // I return the deleted product
            } else
            {
                return NotFound();
            }
        }
    }
}
