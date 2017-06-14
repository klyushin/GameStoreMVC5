using System.Linq;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Models;

namespace GameStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IGameRepository _repository;
        private IOrderProcessor _orderProcessor;

        public CartController(IGameRepository repo, IOrderProcessor orderProcessor)
        {
            _repository = repo;
            _orderProcessor = orderProcessor;
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Index(Cart cartParam, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cartParam,
                ReturnUrl = returnUrl

            });
        }
        public RedirectToRouteResult AddToCart(Cart cartParam, int gameId, string returnUrl)
        {
            Game game = _repository.Games.FirstOrDefault(g => g.GameId == gameId);
            Cart cart = cartParam;
            if (game != null)
            {
                cart.AddItem(game, 1);
            }


            return RedirectToAction("Index", new {returnUrl});
        }

        public RedirectToRouteResult RemoveFromCart(Cart cartParam, int gameId, string returnUrl)
        {
            Game game = _repository.Games.FirstOrDefault(g => g.GameId == gameId);
            Cart cart = cartParam;

            if (game != null)
            {
                cart.RemoveLine(game);
            }
            return RedirectToAction("Index", new { returnUrl });

        }


        public ActionResult Checkout()
        {

            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("","Извините ваша корзина пуста!");
            }
            if (ModelState.IsValid)
            {
                _orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }
            return View(shippingDetails);
        }
    }
}