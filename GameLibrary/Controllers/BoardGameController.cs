using GameLibrary.Data;
using GameLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Controllers
{
    public class BoardGameController : Controller
    {
        private readonly GameContext _context;
        public BoardGameController(GameContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            return View(_context.BoardGames
                .Include(p=>p.Publishers).ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(BoardGameModel boardGame)
        {

            if (ModelState.IsValid)
            {
                boardGame.PublishersId = GetPublisher(boardGame.NewPublisher);
                _context.BoardGames.Add(boardGame);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(boardGame);
        }
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return Problem("id is null");
            }
            var game = _context.BoardGames.FirstOrDefault(
                g => g.Id == id);
            if (game == null)
            {
                return Problem("game is null'");
            }
            return View(game);
        }
        [HttpPost]
        public ActionResult Delete(int? id, bool notUsed)
        {
            if (_context.BoardGames == null)
            {
                return Problem("Entity set 'GameContext.BoardGames is null.'");
            }
            var game = _context.BoardGames.Find(id);
            _context.BoardGames.Remove(game);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(int id, BoardGameModel game)
        {
            if (ModelState.IsValid)
            {
                game.PublishersId = GetPublisher(game.NewPublisher);
                _context.Entry(game).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(game);
        }
        public ActionResult Details(int id) 
        {
            if (id == null) { return NotFound(); }
            var game = _context.BoardGames
                .Where(b => b.Id == id)
                .Include(b => b.Publishers)
                .FirstOrDefault();
            return View(game);
        }
        private int GetPublisher(string publisher)
        {
            PublisherModel? pub = null;
            pub = _context.Publishers
                .Where(p => p.Name.ToLower() == publisher.ToLower())
                .FirstOrDefault();
            if(pub == null) 
            { 
                pub = new PublisherModel {  Name = publisher };
                _context.Add(pub);
                _context.SaveChanges(true);
            }
            return pub.Id;
        }
    }
}
