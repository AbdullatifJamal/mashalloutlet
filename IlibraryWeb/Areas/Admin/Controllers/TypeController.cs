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
    public class TypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Ilibrary.Models.Type> objTypeList = _context.Type.ToList();
            return View(objTypeList);
        }

        //-------------------------------Create-----------------------------------
        public IActionResult Create()
        {
            TypeVM typeVM = new()
            {
                
                 SectionList= _context.Section.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),

                Type = new Ilibrary.Models.Type()
            };
            return View(typeVM);
        }

        [HttpPost]
        public IActionResult Create(TypeVM typeVM)   
        {
            Ilibrary.Models.Type obj = typeVM.Type;
            
            if (ModelState.IsValid)
            {
                _context.Type.Add(obj);
                _context.SaveChanges();
                TempData["success"] = "Type created successfully";
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
            TypeVM typeVM = new() { 
            Type = _context.Type.FirstOrDefault(u => u.Id == id),
            SectionList = _context.Section.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            };
            if (typeVM.Type == null)
            {
                return NotFound();
            }
            return View(typeVM);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Edit(TypeVM model)
        {
            if (ModelState.IsValid)
            {
                // Map TypeVM data to the Type entity
                var typeEntity = new Ilibrary.Models.Type
                {
                    Id = model.Type.Id,
                    Name = model.Type.Name,
                    SectionId = model.Type.SectionId
                };

                // Update the database
                _context.Type.Update(typeEntity);
                _context.SaveChanges();

                TempData["success"] = "Type updated successfully";
                return RedirectToAction("Index");
            }

            // Repopulate SectionList if ModelState is invalid
            model.SectionList = _context.Section.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).ToList();

            return View(model);
        }


        //------------------------------Delete---------------------------------------------
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Ilibrary.Models.Type? typeFromDb = _context.Type.FirstOrDefault(u => u.Id == id);
            if (typeFromDb == null)
            {
                return NotFound();
            }
            return View(typeFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Ilibrary.Models.Type? obj = _context.Type.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _context.Type.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Type deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
