using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ilibrary.DataAccess.Data;
using Ilibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Ilibrary.Utility;
using Ilibrary.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IlibraryWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Brand> objBrandList = _context.Brand.ToList();
            return View(objBrandList);
        }

        //-------------------------------Create-----------------------------------
        public IActionResult Create()
        {


            Brand brand = new Brand();
            
            return View(brand);
        }

        [HttpPost]
        public IActionResult Create(Brand brand)   
        {
            
            
            if (ModelState.IsValid)
            {
                _context.Brand.Add(brand);
                _context.SaveChanges();
                TempData["success"] = "Brand created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        //------------------------------Edit---------------------------------------------
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            
            Brand brand = _context.Brand.FirstOrDefault(u => u.Id == id);

            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand model)
        {
            if (ModelState.IsValid)
            {
                // Map BrandVM data to the Brand entity
                var BrandEntity = new Brand
                {
                    Id = model.Id,
                    Name = model.Name,
                };

                // Update the database
                _context.Brand.Update(BrandEntity);
                _context.SaveChanges();

                TempData["success"] = "Brand updated successfully";
                return RedirectToAction("Index");
            }

           

            return View(model);
        }


        //------------------------------Delete---------------------------------------------
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Ilibrary.Models.Brand? BrandFromDb = _context.Brand.FirstOrDefault(u => u.Id == id);
            if (BrandFromDb == null)
            {
                return NotFound();
            }
            return View(BrandFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Ilibrary.Models.Brand? obj = _context.Brand.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _context.Brand.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Brand deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
