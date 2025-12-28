using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Pronia_example.Contexts;
using Pronia_example.Helpers;
using Pronia_example.Migrations;
using Pronia_example.Models;
using Pronia_example.ViewModels.ProductViewModels;
using System.Reflection;


namespace Pronia_example.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController(AppDbContext _context, IWebHostEnvironment _enviroment) : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<ProductGetVM> products = await _context.Products.Include(x => x.Category).Select(product => new ProductGetVM()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = product.Category.Name,
                MainImagePath = product.MainImagePath,
                HoverImagePath = product.HoverImagePath,
                Rating = product.Rating
            }).ToListAsync();

            //List<ProductGetVM> vms = new();
            //foreach (var product in products)
            //{
            //    ProductGetVM vm = new()
            //    {
            //        Id=product.Id,
            //        Name = product.Name,
            //        Description = product.Description,
            //        Price = product.Price,
            //        CategoryName = product.Category.Name,
            //        MainImagePath = product.MainImagePath,
            //        HoverImagePath = product.HoverImagePath,
            //        Rating = product.Rating
            //    };
            //    vms.Add(vm);
            //}
            return View(products);
        }




        public async Task<IActionResult> Create()
        {
            await SendItemsWithViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {


            if (!ModelState.IsValid)
            {
                await SendItemsWithViewBag();
                return View(vm);
            }
            var isExistCategory = await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId);
            if (!isExistCategory)
            {
                await SendItemsWithViewBag();
                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil");
                return View(vm);
            }
            foreach (var tagId in vm.TagIds)
            {
                var isExistTag = await _context.Tags.AnyAsync(x => x.Id == tagId);
                if (!isExistTag)
                {
                    await SendItemsWithViewBag();
                    ModelState.AddModelError("TagIds", "Bele bir tag movcud deyil");
                    return View(vm);
                }
            }

            if (!vm.MainImage.CheckType())
            {
                await SendItemsWithViewBag();
                ModelState.AddModelError("MainImage", "Yalniz sekil formatinda data daxil edin");
                return View(vm);
            }

            if (!vm.MainImage.CheckSize(2))
            {
                ModelState.AddModelError("MainImage", "Sekil olcusu maksimum 2MB ola biler");
            }


            if (!vm.HoverImage.CheckType())
            {
                ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edin");
                return View(vm);
            }

            if (!vm.HoverImage.CheckSize(2))
            {
                ModelState.AddModelError("HoverImage", "Sekil olcusu maksimum 2MB ola biler");
            }

            string uniqueMainImageName = Guid.NewGuid().ToString() + vm.MainImage.FileName;
            string mainImagePath = @$"{_enviroment.WebRootPath}/assets/images/website-images/{uniqueMainImageName}";

            using FileStream mainStream = new FileStream(mainImagePath, FileMode.Create);
            await vm.MainImage.CopyToAsync(mainStream);





            string uniqueHoverImageName = Guid.NewGuid().ToString() + vm.HoverImage.FileName;
            string hoverImagePath = @$"{_enviroment.WebRootPath}/assets/images/website-images/{uniqueHoverImageName}";

            using FileStream hoverStream = new FileStream(hoverImagePath, FileMode.Create);
            await vm.HoverImage.CopyToAsync(hoverStream);


            Product product = new()
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                MainImagePath = uniqueMainImageName,
                HoverImagePath = uniqueHoverImageName,
                Rating = vm.Rating,
                ProductTags = []
            };

            foreach (var tagId in vm.TagIds)
            {
                ProductTag productTag = new()
                {
                    TagId = tagId,
                    Product = product
                };

                product.ProductTags.Add(productTag);
            }



            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();


            string folderPath = Path.Combine(_enviroment.WebRootPath, "assets", "images", "website-images");
            string mainImagePath = Path.Combine(folderPath, product.MainImagePath);
            string hoverImagePath = Path.Combine(folderPath, product.HoverImagePath);
            if (System.IO.File.Exists(mainImagePath))
            {
                System.IO.File.Delete(mainImagePath);
            }

            if (System.IO.File.Exists(hoverImagePath))
            {
                System.IO.File.Delete(hoverImagePath);
            }


            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.Include(x => x.ProductTags).FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
                return NotFound();


            await SendItemsWithViewBag();

            ProductUpdateVM vm = new ProductUpdateVM()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                MainImagePath = product.MainImagePath,
                HoverImagePath = product.HoverImagePath,
                Rating = product.Rating,
                TagIds = product.ProductTags.Select(x => x.TagId).ToList()
            };
            return View(vm);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                await SendItemsWithViewBag();
                return View(vm);
            }
            foreach (var tagId in vm.TagIds)
            {
                var isExistTag = await _context.Tags.AnyAsync(x => x.Id == tagId);
                if (!isExistTag)
                {
                    await SendItemsWithViewBag();
                    ModelState.AddModelError("TagIds", "Bele bir tag movcud deyil");
                    return View(vm);
                }
            }


            if (!vm.MainImage?.CheckType() ?? false)
            {
                ModelState.AddModelError("MainImage", "Yalniz sekil formatinda data daxil edin");
                return View(vm);
            }

            if (!vm.MainImage?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("MainImage", "Sekil olcusu maksimum 2MB ola biler");
            }


            if (!vm.HoverImage?.CheckType() ?? false)
            {
                ModelState.AddModelError("HoverImage", "Yalniz sekil formatinda data daxil edin");
                return View(vm);
            }

            if (!vm.HoverImage?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("HoverImage", "Sekil olcusu maksimum 2MB ola biler");
			}

            var existProduct = await _context.Products.Include(x=> x.ProductTags).FirstOrDefaultAsync(x=>x.Id==vm.Id);
            if (existProduct is null)
                return BadRequest();

            var isExistCategory = await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId);
            if (!isExistCategory)
            {
                await SendItemsWithViewBag();
                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil");
                return View(vm);
            }
            existProduct.Name = vm.Name;
            existProduct.Description = vm.Description;
            existProduct.Price = vm.Price;
            //existProduct.ImagePath= product.ImagePath;
            existProduct.CategoryId = vm.CategoryId;
            existProduct.ProductTags = [];
            foreach (var tagId in vm.TagIds)
            {
                ProductTag productTag = new()
                {
                    TagId = tagId,
                    ProductId = existProduct.Id
                };
                existProduct.ProductTags.Add(productTag);
            }
                string folderPath = Path.Combine(_enviroment.WebRootPath, "assets", "images", "website-images");

                if (vm.MainImage is not null)
                {
                    string newMainImagePath = await vm.MainImage.SaveFileAsync(folderPath);
                    string existMainImagePath = Path.Combine(folderPath, existProduct.MainImagePath);
                    ExtensionMethods.DeleteFile(existMainImagePath);
                    existProduct.MainImagePath = newMainImagePath;
                }

                if (vm.HoverImage is not null)
                {
                    string newHoverImagePath = await vm.HoverImage.SaveFileAsync(folderPath);
                    string existHoverImagePath = Path.Combine(folderPath, existProduct.HoverImagePath);
                    ExtensionMethods.DeleteFile(existHoverImagePath);
                    existProduct.HoverImagePath = newHoverImagePath;
                }

                _context.Products.Update(existProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            

        }

            public async Task<IActionResult> Detail(int id)
            {
                var product = await _context.Products.Include(x => x.Category).Select(product => new ProductGetVM()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryName = product.Category.Name,
                    MainImagePath = product.MainImagePath,
                    HoverImagePath = product.HoverImagePath,
                    Rating = product.Rating,
                    TagNames = product.ProductTags.Select(x => x.Tag.Name).ToList()
                }).FirstOrDefaultAsync(x => x.Id == id);

                if (product is null)
                    return NotFound();
                return View(product);
            }

            private async Task SendItemsWithViewBag()
            {
                var categories = await _context.Categories.ToListAsync();
                ViewBag.Categories = categories;


                var tags = await _context.Tags.ToListAsync();
                ViewBag.Tags = tags;
            }
    

    }
}
