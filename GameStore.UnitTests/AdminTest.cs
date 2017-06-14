using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameStore.UnitTests
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void Index_Contains_All_Games()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(g => g.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1"},
                new Game {GameId = 2, Name = "Игра2"},
                new Game {GameId = 3, Name = "Игра3"},
                new Game {GameId = 4, Name = "Игра4"},
                new Game {GameId = 5, Name = "Игра5"}
            });

            AdminController adminController = new AdminController(mock.Object);

            //act
            List<Game> result = ((IEnumerable<Game>) adminController.Index().ViewData.Model).ToList();

            //assert
            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual("Игра1", result[0].Name);
            Assert.AreEqual("Игра2", result[1].Name);
            Assert.AreEqual("Игра3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Game()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(g => g.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1"},
                new Game {GameId = 2, Name = "Игра2"},
                new Game {GameId = 3, Name = "Игра3"},
                new Game {GameId = 4, Name = "Игра4"},
                new Game {GameId = 5, Name = "Игра5"}
            });

            AdminController adminController = new AdminController(mock.Object);

            //act
            Game g1 = adminController.Edit(1).ViewData.Model as Game;
            Game g2 = adminController.Edit(2).ViewData.Model as Game;
            Game g3 = adminController.Edit(3).ViewData.Model as Game;

            //assert
            Assert.AreEqual(g1.GameId, 1);
            Assert.AreEqual(g2.GameId, 2);
            Assert.AreEqual(g3.GameId, 3);
        }

        [TestMethod]
        public void Cannont_Edit_NotExisting_Game()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(g => g.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1"},
            });

            AdminController adminController = new AdminController(mock.Object);

            //act
            Game g1 = adminController.Edit(6).ViewData.Model as Game;

            //assert
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            Game game = new Game {Name = "test"};
            AdminController adminController = new AdminController(mock.Object);
            //act
            ActionResult result = adminController.Edit(game);

            //assert
            mock.Verify(m => m.SaveGame(game));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            Game game = new Game {Name = "tet"};
            AdminController adminController = new AdminController(mock.Object);

            adminController.ModelState.AddModelError("error", "error");

            //act
            ActionResult result = adminController.Edit(game);

            //assert
            mock.Verify(m => m.SaveGame(It.IsAny<Game>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Games()
        {
            //arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1"},
                new Game {GameId = 2, Name = "Игра2"},
                new Game {GameId = 3, Name = "Игра3"},
                new Game {GameId = 4, Name = "Игра4"},
                new Game {GameId = 5, Name = "Игра5"}
            });

            AdminController adminController = new AdminController(mock.Object);
            
            //act
            adminController.Delete(4);

            //assert
            mock.Verify(m => m.DeleteGame(4));
        }
    }
}