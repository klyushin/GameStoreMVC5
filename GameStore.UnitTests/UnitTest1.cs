using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.HtmlHelpers;
using GameStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(x => x.Games).Returns(
                new List<Game>
                {
                    new Game {GameId = 1, Name = "Игра1"},
                    new Game {GameId = 2, Name = "Игра2"},
                    new Game {GameId = 3, Name = "Игра3"},
                    new Game {GameId = 4, Name = "Игра4"},
                    new Game {GameId = 5, Name = "Игра5"}
                });
            GameController controller = new GameController(mock.Object);
            controller.PageSize = 3;

            //act
            GamesListViewModel result = (GamesListViewModel) controller.List(null, 2).Model;

            //assert
            List<Game> games = result.Games.ToList();
            Assert.IsTrue(games.Count == 2);
            Assert.AreEqual(games[0].Name, "Игра4");
            Assert.AreEqual(games[1].Name, "Игра5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //arrange
            HtmlHelper htmlHelper = null;

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.CurrentPage = 2;
            pagingInfo.ItemsPerPage = 10;
            pagingInfo.TotalItems = 28;

            //act
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //assert
            MvcHtmlString result = htmlHelper.PageLinks(pagingInfo, pageUrlDelegate);
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(x => x.Games).Returns(
                new List<Game>
                {
                    new Game {GameId = 1, Name = "Игра1"},
                    new Game {GameId = 2, Name = "Игра2"},
                    new Game {GameId = 3, Name = "Игра3"},
                    new Game {GameId = 4, Name = "Игра4"},
                    new Game {GameId = 5, Name = "Игра5"}
                });
            GameController gameController = new GameController(mock.Object);
            gameController.PageSize = 3;

            //act
            GamesListViewModel gamesListViewModel = (GamesListViewModel) gameController.List(null, 2).Model;


            //assert
            PagingInfo pagingInfo = gamesListViewModel.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Games()
        {
            // Организация (arrange)
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1", Category = "Cat1"},
                new Game {GameId = 2, Name = "Игра2", Category = "Cat2"},
                new Game {GameId = 3, Name = "Игра3", Category = "Cat1"},
                new Game {GameId = 4, Name = "Игра4", Category = "Cat2"},
                new Game {GameId = 5, Name = "Игра5", Category = "Cat3"}
            });
            GameController controller = new GameController(mock.Object);
            controller.PageSize = 3;

            // Action
            List<Game> result = ((GamesListViewModel) controller.List("Cat2", 1).Model)
                .Games.ToList();

            // Assert
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Игра2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "Игра4" && result[1].Category == "Cat2");
        }


        [TestMethod]
        public void Can_Create_Categories()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1", Category = "Симулятор"},
                new Game {GameId = 2, Name = "Игра2", Category = "Симулятор"},
                new Game {GameId = 3, Name = "Игра3", Category = "Шутер"},
                new Game {GameId = 4, Name = "Игра4", Category = "RPG"},
            });

            NavController navController = new NavController(mock.Object);

            //act
            List<string> results = ((IEnumerable<string>) navController.Menu().Model).ToList();

            //assert
            Assert.AreEqual(results.Count, 3);
            Assert.AreEqual(results[0], "RPG");
            Assert.AreEqual(results[1], "Симулятор");
            Assert.AreEqual(results[2], "Шутер");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(
                new List<Game>
                {
                    new Game {GameId = 1, Name = "Игра1", Category = "Симулятор"},
                    new Game {GameId = 2, Name = "Игра2", Category = "Симулятор"},
                    new Game {GameId = 3, Name = "Игра3", Category = "Шутер"},
                    new Game {GameId = 4, Name = "Игра4", Category = "RPG"},
                });

            NavController navController = new NavController(mock.Object);
            string categoryToSelect = "Шутер";

            //act
            string result = navController.Menu(categoryToSelect).ViewBag.SelectedCategory;
            //assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Game_Count()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(g => g.Games).Returns(
                new List<Game>
                {
                    new Game {GameId = 1, Name = "Игра1", Category = "Cat1"},
                    new Game {GameId = 2, Name = "Игра2", Category = "Cat2"},
                    new Game {GameId = 3, Name = "Игра3", Category = "Cat1"},
                    new Game {GameId = 4, Name = "Игра4", Category = "Cat2"},
                    new Game {GameId = 5, Name = "Игра5", Category = "Cat3"}
                }
                );

            GameController gameController = new GameController(mock.Object);
            gameController.PageSize = 3;

            //act
            int totalItemsResultCat1 = ((GamesListViewModel) gameController.List("Cat1", 1).Model).PagingInfo.TotalItems;
            int totalItemsResultCat2 = ((GamesListViewModel)gameController.List("Cat2", 1).Model).PagingInfo.TotalItems;
            int totalItemsResultCat3 = ((GamesListViewModel)gameController.List("Cat3", 1).Model).PagingInfo.TotalItems;
            int totalItemsResultCatNull = ((GamesListViewModel)gameController.List(null, 1).Model).PagingInfo.TotalItems;

            //assert
            Assert.AreEqual(totalItemsResultCat1, 2);
            Assert.AreEqual(totalItemsResultCat2, 2);
            Assert.AreEqual(totalItemsResultCat3, 1);
            Assert.AreEqual(totalItemsResultCatNull, 5);


        }
    }
}