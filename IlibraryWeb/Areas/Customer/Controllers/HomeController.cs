using Ilibrary.DataAccess.Data;
using Ilibrary.DataAccess.Repository.IRepository;
using Ilibrary.Models;
using Ilibrary.Models.ViewModels;
using Ilibrary.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Type = Ilibrary.Models.Type;

namespace IlibraryWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;



        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _context = context;


        }

        public IActionResult Index()
        {

            ///////////////// old Index 
            //IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            //var query = from product in _context.Products
            //            join orderDetail in _context.OrderDetails
            //            on product.Id equals orderDetail.ProductId into productOrders
            //            orderby productOrders.Count() descending
            //            select product;

            // Execute the query and convert the result to a list of products
            //IEnumerable<Product> productsOrderedByOrderCount = query.ToList();

            /////////new index
            MainVM vm = new MainVM()
            {

                onBannerProducts = _context.Products.ToList().Where(c => c.OnBanner == true),
                LatestProducts = _context.Products.ToList().OrderBy(c => c.UploadDate),
                onSaleProducts = _context.Products.ToList().Where(c => c.Discount != 0)
            };



            return View(vm);
        }

        public IActionResult Filter(
    string? searchTerm,
    string? sort,
    string[]? categories,
    string? section,
    string[]? brands,
    string[]? types,
    bool? discount,
    bool? isAvailable,
    string[]? selectedAlphaSizes,
    string[]? selectedNumericSizes,
    int page = 0)
        {
            page = Math.Max(page, 0);

            // Query products
            var products = _context.Products
                                   .Include(p => p.Type)
                                       .ThenInclude(t => t.Section)
                                   .Include(p => p.brand)
                                   .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                products = products.Where(p => p.Name.ToLower().Contains(searchTerm));
            }

            // Apply category filter
            if (categories != null && categories.Any())
            {
                products = products.Where(p => categories.Contains(p.Category));
            }

            // Apply discount filter
            if (discount == true)
            {
                products = products.Where(p => p.Discount > 0);
            }

            // Apply availability filter
            if (isAvailable == true)
            {
                products = products.Where(p => p.IsAvailable == true);
            }

            // Apply section filter
            if (!string.IsNullOrEmpty(section))
            {
                section = section.ToLower();
                products = products.Where(p => p.Type.Section.Name.ToLower() == section);
            }

            // Apply brand filter
            if (brands != null && brands.Any())
            {
                products = products.Where(p => brands.Contains(p.brand.Name));
            }

            // Apply type filter
            if (types != null && types.Any())
            {
                products = products.Where(p => types.Contains(p.Type.Name));
            }

            // Apply size filters using the Sizes table
            if ((selectedAlphaSizes != null && selectedAlphaSizes.Any()) ||
    (selectedNumericSizes != null && selectedNumericSizes.Any()))
            {
                products = products.Where(p => _context.Sizes
                    .Any(s => s.ProductId == p.Id &&
                             ((selectedAlphaSizes != null && selectedAlphaSizes.Contains(s.size)) ||
                              (selectedNumericSizes != null && selectedNumericSizes.Contains(s.size)))));
            }


            // Apply sorting logic
            products = sort switch
            {
                "NameAsc" => products.OrderBy(p => p.Name),
                "NameDesc" => products.OrderByDescending(p => p.Name),
                "PriceAsc" => products.OrderBy(p => p.Price - (p.Discount * p.Price / 100)),
                "PriceDesc" => products.OrderByDescending(p => p.Price - (p.Discount * p.Price / 100)),
                _ => products.OrderBy(p => p.Name)
            };

            // Pagination
            const int pageSize = 12;
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var paginatedProducts = products.Skip(page * pageSize).Take(pageSize).ToList();

            // Fetch and process sizes
            var allSizes = _context.Sizes.Select(s => s.size).Distinct().ToList();
            var alphabetSizes = allSizes.Where(s => char.IsLetter(s[0])).OrderBy(s => s).ToList();
            var numericSizes = allSizes.Where(s => double.TryParse(s, out _)).OrderBy(s => double.Parse(s)).ToList();

            // Setup ViewBag
            ViewBag.CurrentSort = sort;
            ViewBag.AllCategories = _context.Products.Select(p => p.Category).Distinct().ToList();
            ViewBag.AllSections = _context.Section.Select(s => s.Name).ToList();
            ViewBag.AllBrands = _context.Brand.Select(b => b.Name).ToList();
            ViewBag.AllTypes = _context.Type.Select(t => t.Name).ToList();
            ViewBag.AlphabetSizes = alphabetSizes;
            ViewBag.NumericSizes = numericSizes;
            ViewBag.PageIndex = page;
            ViewBag.TotalPages = totalPages;

            // Build model
            var model = new FilterVM
            {
                BrandList = brands?.Select(b => new Brand { Name = b }).ToList() ?? new List<Brand>(),
                ProductList = paginatedProducts,
                SearchTerm = searchTerm,
                SelectedCategories = categories?.ToList(),
                SelectedBrands = brands?.ToList(),
                Section = section,
                Discount = discount,
                isAvailable = isAvailable,
                TypeList = types?.Select(t => new Type { Name = t }).ToList() ?? new List<Type>(),
                SelectedTypes = types?.ToList(),
                SelectedAlphaSizes = selectedAlphaSizes?.ToList(),
                SelectedNumericSizes = selectedNumericSizes?.ToList(),
                SortOption = sort,
                PageIndex = page,
                TotalPages = totalPages
            };

            return View(model);
        }














        //public IActionResult Filter(string? searchTerm, string? sort, string[]? categories, string? section)
        //{
        //    var products = _context.Products.AsQueryable();

        //    // Apply search filter (case-insensitive search)
        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        searchTerm = searchTerm.ToLower();
        //        products = products.Where(p => p.Name.ToLower().Contains(searchTerm));
        //    }

        //    // Apply category filter
        //    if (categories != null && categories.Any())
        //    {
        //        products = products.Where(p => categories.Contains(p.Category));
        //    }

        //    // Apply section filter (assuming 'section' corresponds to some property of the products)
        //    if (!string.IsNullOrEmpty(section))
        //    {
        //        products = products.Where(p => p.Section.ToLower() == section.ToLower());
        //    }

        //    // Apply sorting logic
        //    switch (sort)
        //    {
        //        case "NameAsc":
        //            products = products.OrderBy(p => p.Name);
        //            break;
        //        case "NameDesc":
        //            products = products.OrderByDescending(p => p.Name);
        //            break;
        //        case "PriceAsc":
        //            products = products.OrderBy(p => p.Price - (p.Discount * p.Price / 100));
        //            break;
        //        case "PriceDesc":
        //            products = products.OrderByDescending(p => p.Price - (p.Discount * p.Price / 100));
        //            break;
        //        default:
        //            products = products.OrderBy(p => p.Name);  // Default: Relevance
        //            break;
        //    }

        //    // Paginate results
        //    var pageSize = 12;
        //    var pageIndex = 0;
        //    var paginatedProducts = products.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        //    // View data setup
        //    ViewBag.CurrentSort = sort;
        //    ViewBag.AllCategories = _context.Categories.Select(c => c.Name).ToList();

        //    var model = new FilterVM
        //    {
        //        ProductList = paginatedProducts,
        //        SearchTerm = searchTerm,
        //        SelectedCategories = categories?.ToList(),
        //        Section = section
        //    };

        //    return View(model);
        //}










        public IActionResult Details(int productId)
        {

            ////IEnumerable<Comment> allComments = _context.Comments.Where(c => c.ProductId == productId);
            //ShoppingCart cart = new()

            //{
            //    //Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
            //    Product = _context.Products.FirstOrDefault(u => u.Id == productId),

            //    Count = 1,
            //    ProductId = productId
            //};
            ////Product produc = _unitOfWork.Product.Get(u =>u.Id== productId, includeProperties: "Category");
            ////return View(produc);
            //var com = new Comment { ProductId = productId };
            //DetailsVM detailsVM = new()
            //{
            //    //shoppingcarts = cart,
            //    //comments = com,
            //    //allcomments = allComments

            //};
            Product _product = _context.Products.FirstOrDefault(c => c.Id == productId);
            Ilibrary.Models.Type type = _context.Type.FirstOrDefault(c => c.Id == _product.TypeId);
            _product.Type = type;
            _product.brand = _context.Brand.FirstOrDefault(c => c.Id == _product.BrandId);
            DetailsVM vm = new DetailsVM()
            {
                product = _product,
                Products = _context.Products.ToList().Where(p => p.Id != _product.Id && _context.Sizes
                    .Any(s => s.ProductId == p.Id &&
                             ((_product.Size != null && _product.Size.Contains(s.size)))))
            };
            return View(vm);


        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(DetailsVM vm)

        {

            ////ShoppingCart shoppingCart = vm.shoppingcarts;
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var userName = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            //var indexOfAtSymbol = userName.IndexOf('@');
            //if (indexOfAtSymbol != -1)
            //{
            //    userName = userName.Substring(0, indexOfAtSymbol);

            //}
            //shoppingCart.ApplicationUserId = userId;

            //ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            //u.ProductId == shoppingCart.ProductId);

            //if (cartFromDb != null)
            //{
            //    //shopping cart exists
            //    cartFromDb.Count += shoppingCart.Count;
            //    _unitOfWork.ShoppingCart.Update(cartFromDb);
            //    _unitOfWork.Save();
            //}
            //else
            //{
            //    //add cart record
            //    _unitOfWork.ShoppingCart.Add(shoppingCart);
            //    _unitOfWork.Save();
            //    HttpContext.Session.SetInt32(SD.SessionCart,
            //    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            //}
            //TempData["success"] = "Cart updated successfully";
            //_unitOfWork.Save();



            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddComment(DetailsVM model)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;

        //    var userName = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
        //    var indexOfAtSymbol = userName.IndexOf('@');
        //    if (indexOfAtSymbol != -1)
        //    {
        //        userName = userName.Substring(0, indexOfAtSymbol);

        //    }

        //    Comment comm = model.comments;
        //    comm.ProductId = model.shoppingcarts.ProductId;
        //    comm.Rating = model.comments.Rating;
        //    comm.UserName = userName;
        //    int id;
        //    _context.Comments.Add(comm);
        //    _context.SaveChanges();
        //    //return RedirectToAction("Index", "Product", new { id = comm.ProductId });
        //    return RedirectToAction(nameof(Index));


        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteComment(int id)
        //{
        //    Comment comment =  _context.Comments.FirstOrDefault(i => i.Id == id);
        //    _context.Comments.Remove(comment);
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));


        //}
    }
}
