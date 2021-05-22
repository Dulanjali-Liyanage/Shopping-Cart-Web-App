using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCartDemo.Data;
using ShoppingCartDemo.Models;
using Microsoft.AspNetCore.Http;
using ShoppingCartDemo.Helpers;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingCart.Controllers
{
    
    public class MyCartItemsController : Controller
    {
        private readonly ShoppingCartDemoContext _context;

        public MyCartItemsController(ShoppingCartDemoContext context)
        {
            _context = context;
        }

        [Route("index")]
        // GET: MyCartItems
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
            

            if (cart == null) {
                ViewBag.cart = cart;
                ViewBag.total = 0;
            }
            else
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Price);
            }

            
            return View();
        }

        [Authorize(Roles = "CartUser")]
        [Route("addtocartview")]
        //displaying the details of the item
        // GET: Items/AddtoCart/5
        public async Task<IActionResult> AddtoCart(int? id)
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
            return View(item);
        }


        [Route("tocart")]
        public IActionResult ToCart([Bind("Id,Title,Category,Price,ImageName")] Item myCartItem)
        {
            Debug.WriteLine(myCartItem);
            
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(myCartItem);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
                /*int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Product = productModel.find(id), Quantity = 1 });
                }*/
                cart.Add(myCartItem);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", cart);
            }
            return RedirectToAction("Index","Items");
        }





        // GET: MyCartItems/Delete/5

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

            return PartialView("MyCartItemDeletePartialView", item);
        }

        // POST: MyCartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
            Debug.WriteLine(id);
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", cart);
            return RedirectToAction("index");
        }


        
        [Route("Logout")]
        public IActionResult Logout()
        {
            SessionHelper.SetObjectAsJson(HttpContext.Session, "User", null);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Items", null);
            return RedirectToAction("Index","Home");
        }

        private int isExist(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "Items");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }


    }
}
