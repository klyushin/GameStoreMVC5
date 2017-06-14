using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.WebUI.Models;

namespace GameStore.WebUI.Controllers
{
    public class GameController : Controller
    {
        private IGameRepository _repository;
        public int PageSize = 4;

        public GameController(IGameRepository repo)
        {
            _repository = repo;
            
        }

        public ViewResult List(string category, int page = 1)
        {
            GamesListViewModel model = new GamesListViewModel
            {
                Games = _repository.Games
                .Where(game => category == null || game.Category == category)
                .OrderBy(game => game.GameId)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? _repository.Games.Count() :
                    _repository.Games.Count(g => g.Category == category)
                },
                CurrentCategory = category
            };



            return View(model);
        }
    }
}