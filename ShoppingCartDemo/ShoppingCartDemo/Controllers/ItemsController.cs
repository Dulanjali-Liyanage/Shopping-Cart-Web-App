using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCartDemo.Data;
using ShoppingCartDemo.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using ShoppingCartDemo.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingCartDemo.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ShoppingCartDemoContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly UserManager<ApplicationUser> _UserManager;

        public ItemsController(ShoppingCartDemoContext context, IWebHostEnvironment hostEnvironment, SignInManager<ApplicationUser> SignInManager, UserManager<ApplicationUser> UserManager)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
            _SignInManager = SignInManager;
            _UserManager = UserManager;
        }

        [Route("product-list")]
        // GET: Items
        public async Task<IActionResult> Index(string itemCategory, string searchString)
        {
            // Use LINQ to get list of categories in the Item.
            IQueryable<string> categoryQuery = from m in _context.Item
                                               orderby m.Category
                                               select m.Category;

            var items = from m in _context.Item
                        select m;

            //first select the items title matched with the passed search string value
            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Title.Contains(searchString));
            }

            //then from the above filtered list filter out the items matching to the given category
            if (!string.IsNullOrEmpty(itemCategory))
            {
                items = items.Where(x => x.Category == itemCategory);
            }

            var itemCategoryVM = new ItemCategoryViewModel
            {
                Category = new SelectList(await categoryQuery.Distinct().ToListAsync()),
                Items = await items.ToListAsync()
            };
            ViewBag.category = new List<String>(await categoryQuery.Distinct().ToListAsync());
           
            //check whether the logged user's role is admin
            if (_SignInManager.IsSignedIn(User)) 
            {
                if (User.IsInRole("Admin")) 
                {
                    return View("AdminProductList", itemCategoryVM);
                }
                return View(itemCategoryVM);
            }

            return View(itemCategoryVM);
        }

        

        [Route("product-list/Details")]
        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Title,Category,Price,Image")] ItemViewModel itemView)
        {
            Debug.WriteLine(itemView.Image);
            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemView.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    itemView.Image.CopyTo(fileStream);
                }

                Item item = new Item
                {
                    Id = itemView.Id,
                    Title = itemView.Title,
                    Category = itemView.Category,
                    Price = itemView.Price,
                    ImageName = uniqueFileName
                };

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            

            ItemViewModel itemView = new ItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Category = item.Category,
                Price = item.Price,
            };


            return View(itemView);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Category,Price,Image")] ItemViewModel itemView)
        {
            if (id != itemView.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemView.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    itemView.Image.CopyTo(fileStream);
                }

                Item item = new Item
                {
                    Id = itemView.Id,
                    Title = itemView.Title,
                    Category = itemView.Category,
                    Price = itemView.Price,
                    ImageName = uniqueFileName
                };

                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(itemView);
        }

        [Authorize(Roles = "Admin")]
        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return PartialView("DeletePartialView",item);
        }

        [Authorize(Roles = "Admin")]
        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.Id == id);
        }
    }
}
