using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;

namespace GameStore.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private IGameRepository _repository;

        public AdminController(IGameRepository repo)
        {
            _repository = repo;
        }

        // GET: Admin
        public ViewResult Index()
        {
            return View(_repository.Games);
        }

        public ViewResult Edit(int gameId)
        {
            Game game = _repository.Games.FirstOrDefault(g => g.GameId == gameId);

            return View(game);
        }

        [HttpPost]
        public ActionResult Edit(Game game)
        {
            if (ModelState.IsValid)
            {
                _repository.SaveGame(game);
                TempData["message"] = string.Format("Изменения в игре {0} были сохранены", game.Name);
                return RedirectToAction("Index");
            }
            else
            {
                return View(game);
            }
        }

        public ViewResult Create()
        {


            return View("Edit", new Game());
        }

        [HttpPost]
        public ActionResult Delete(int gameId)
        {
            Game deletedGame = _repository.DeleteGame(gameId);
            if (deletedGame != null)
            {
                TempData["message"] = string.Format("Игра \"{0}\" была удалена",
                    deletedGame.Name);
            }
            return RedirectToAction("Index");
        }
    }
}