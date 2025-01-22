

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ilibrary.DataAccess.Data;
using Ilibrary.Models;
using Ilibrary.DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ilibrary.Models.ViewModels;
using Ilibrary.Utility;
using Microsoft.AspNetCore.Authorization;
using SendGrid.Helpers.Mail;
using System.Linq;

namespace IlibraryWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment , ApplicationDbContext context)
        { 
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _context = context;


        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
              
            return View(objProductList);
        }

      
        public IActionResult Upsert(int? id)
        {
            
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }), 
                BrandList = _context.Brand.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                TypeList = _context.Type.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                
                Product = new Product()
            };
            if(id ==  null || id== 0){
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM); 
            }
            
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? typeFromDb = _context.Products.FirstOrDefault(u => u.Id == id);
            if (typeFromDb == null)
            {
                return NotFound();
            }
            return View(typeFromDb);
        }




        [HttpPost]
        public IActionResult Upsert(ProductVM productVM)
        {
            // Set upload date
            productVM.Product.UploadDate = DateTime.Now;

            // Validate ModelState
            if (ModelState.IsValid)
            {
                // Parse sizes from the input
                List<string> sizeList = productVM.Product.Size?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList() ?? new List<string>();

                // Handle Main Image
                if (productVM.MainImage != null)
                {
                    // If a new main image is provided, convert it to byte array
                    productVM.Product.MainImage = ConvertToByteArray(productVM.MainImage);
                }
                else if (productVM.Product.Id != 0)
                {
                    // Preserve existing main image during update
                    var existingProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == productVM.Product.Id);
                    if (existingProduct != null)
                    {
                        productVM.Product.MainImage = existingProduct.MainImage;
                    }
                }

                // Handle Secondary Images
                if (productVM.SecondaryImages != null && productVM.SecondaryImages.Any())
                {
                    // Convert uploaded secondary images to byte arrays
                    List<byte[]> secondaryImages = new List<byte[]>();
                    foreach (var image in productVM.SecondaryImages)
                    {
                        secondaryImages.Add(ConvertToByteArray(image));
                    }
                    productVM.Product.SecondaryImages = secondaryImages.ToArray();
                }
                else if (productVM.Product.Id != 0)
                {
                    // Preserve existing secondary images during update
                    var existingProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == productVM.Product.Id);
                    if (existingProduct != null)
                    {
                        productVM.Product.SecondaryImages = existingProduct.SecondaryImages;
                    }
                }

                // Add or Update Product
                if (productVM.Product.Id == 0)
                {
                    // Insert new product
                    _context.Products.Add(productVM.Product);
                    _context.SaveChanges();

                    int productId = productVM.Product.Id; // Retrieve generated ProductId

                    // Add sizes for the new product
                    foreach (string item in sizeList)
                    {
                        Sizes size = new Sizes()
                        {
                            ProductId = productId,
                            size = item,
                        };
                        _context.Sizes.Add(size);
                    }
                    _context.SaveChanges();
                    TempData["success"] = "Product created successfully!";
                }
                else
                {
                    // Update existing product
                    var existingProduct = _context.Products.Find(productVM.Product.Id);
                    if (existingProduct != null)
                    {
                        // Update the fields, preserving images if not replaced
                        existingProduct.Name = productVM.Product.Name;
                        existingProduct.Description = productVM.Product.Description;
                        existingProduct.Price = productVM.Product.Price;
                        existingProduct.Category = productVM.Product.Category;
                        existingProduct.BrandId = productVM.Product.BrandId;
                        existingProduct.TypeId = productVM.Product.TypeId;
                        existingProduct.MainImage = productVM.Product.MainImage ?? existingProduct.MainImage;
                        existingProduct.SecondaryImages = productVM.Product.SecondaryImages ?? existingProduct.SecondaryImages;

                        _context.Products.Update(existingProduct);
                        _context.SaveChanges();

                        // Remove old sizes associated with the product
                        var existingSizes = _context.Sizes.Where(s => s.ProductId == productVM.Product.Id).ToList();
                        _context.Sizes.RemoveRange(existingSizes);

                        // Add updated sizes
                        foreach (string item in sizeList)
                        {
                            Sizes size = new Sizes()
                            {
                                ProductId = productVM.Product.Id,
                                size = item,
                            };
                            _context.Sizes.Add(size);
                        }
                        _context.SaveChanges();
                        TempData["success"] = "Product updated successfully!";
                    }
                }

                return RedirectToAction("Index");
            }

            // Rebind Dropdown Lists on Validation Failure
            RebindSelectLists(productVM);

            // Return the same view with validation errors
            return View(productVM);
        }


        // Utility Method: Convert File to Byte Array
        private byte[] ConvertToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // Utility Method: Rebind Select Lists
        private void RebindSelectLists(ProductVM productVM)
        {
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            productVM.BrandList = _context.Brand.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            productVM.TypeList = _context.Type.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }



        [HttpPost]
        public IActionResult DeleteMainImage(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            product.MainImage = null; // Remove the image data
            _context.SaveChanges();

            return Ok();
        }




        //#endregion
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _context.Products.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _context.Products.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Type deleted successfully";
            return RedirectToAction("Index");
        }

        #endregion

    }
} 
