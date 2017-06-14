using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameStore.UnitTests
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //arrange
            Game game1 = new Game {GameId = 1, Name = "Игра1"};
            Game game2 = new Game {GameId = 2, Name = "Игра2"};
            Cart cart = new Cart();

            //act
            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            List<CartLine> result = cart.Lines.ToList();

            //assert
            Assert.AreEqual(result[0].Game, game1);
            Assert.AreEqual(result[1].Game, game2);
            Assert.IsTrue(result.Count == 2);

        }

        [TestMethod]
        public void Can_Add_Quantities_For_Existing_Lines()
        {
            //arrange
            Game game1 = new Game {GameId = 1, Name = "Игра1"};
            Game game2 = new Game {GameId = 2, Name = "Игра2"};
            Cart cart = new Cart();

            //act
            cart.AddItem(game1, 2);
            cart.AddItem(game2, 3);
            cart.AddItem(game1, 1);
            cart.AddItem(game2, 2);
            List<CartLine> result = cart.Lines.OrderBy(g => g.Game.GameId).ToList();

            //assert
            Assert.AreEqual(result[0].Quantity, 3);
            Assert.AreEqual(result[1].Quantity, 5);
            Assert.IsTrue(result.Count == 2);


        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //arrange
            Game game1 = new Game {GameId = 1, Name = "Игра1"};
            Game game2 = new Game {GameId = 2, Name = "Игра2"};
            Game game3 = new Game {GameId = 3, Name = "Игра3"};
            Cart cart = new Cart();

            //act
            cart.AddItem(game1, 1);
            cart.AddItem(game2, 2);
            cart.AddItem(game2, 3);

            cart.RemoveLine(game1);
            List<CartLine> result = cart.Lines.ToList();
            //assert
            Assert.AreEqual(result.Count(g => g.Game == game1), 0);
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //arrange
            Game game1 = new Game {GameId = 1, Name = "Игра1", Price = 100};
            Game game2 = new Game {GameId = 2, Name = "Игра2", Price = 50};
            Cart cart = new Cart();

            //act
            cart.AddItem(game1, 1);
            cart.AddItem(game1, 2);
            cart.AddItem(game2, 3);
            cart.AddItem(game1, 2);

            decimal result = cart.ComputeTotalValue();

            //assert 500+150
            Assert.AreEqual(result, 650);

        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //arrange
            Game game1 = new Game {GameId = 1, Name = "Игра1", Price = 100};
            Game game2 = new Game {GameId = 2, Name = "Игра2", Price = 55};
            Cart cart = new Cart();

            //act
            cart.AddItem(game1, 2);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 3);

            cart.Clear();

            List<CartLine> result = cart.Lines.ToList();

            //assert
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(
                new List<Game>
                {
                    new Game {GameId = 1, Name = "Игра1", Category = "Кат1"}
                }.AsQueryable()
                );
            Cart cart = new Cart();
            CartController cartController = new CartController(mock.Object, null);

            //act
            cartController.AddToCart(cart, 1, null);

            //assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToList()[0].Game.GameId, 1);
        }

        [TestMethod]
        public void Adding_Game_To_Cart_Goes_To_Cart_Screen()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games)
                .Returns(new List<Game> {new Game {GameId = 1, Name = "Игра1", Category = "Кат1"}}.AsQueryable());
            CartController cartController = new CartController(mock.Object, null);
            Cart cart = new Cart();

            //act
            RedirectToRouteResult result = cartController.AddToCart(cart, 1, "myUrl");

            //assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");

        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //arrange
            Cart cart = new Cart();
            CartController cartController = new CartController(null, null);

            //act
            CartIndexViewModel cartIndexViewModel =
                (CartIndexViewModel) cartController.Index(cart, "myUrl").ViewData.Model;

            //assert
            Assert.AreEqual(cartIndexViewModel.Cart, cart);
            Assert.AreEqual(cartIndexViewModel.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController cartController = new CartController(null, mock.Object);
            //act
            ViewResult viewResult = cartController.Checkout(cart, shippingDetails);

            //assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);
            Assert.AreEqual("", viewResult.ViewName);
            Assert.AreEqual(false, viewResult.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //arrange
            Mock<IOrderProcessor> mock  = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Game(), 1);
            CartController cartController = new CartController(null, mock.Object);
            cartController.ModelState.AddModelError("error", "error");

            //act
            ViewResult result = cartController.Checkout(cart, new ShippingDetails());

            //assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);


        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart(); 
            cart.AddItem(new Game(), 1);
            CartController cartController = new CartController(null, mock.Object);

            //act
            ViewResult result = cartController.Checkout(cart, new ShippingDetails());

            //assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
}
}
