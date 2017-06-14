using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Domain.Entities
{
    public class Cart
    {
        private List<CartLine> _lineCollection = new List<CartLine>();

        public void AddItem(Game game, int quantity)
        {
            CartLine line = _lineCollection.Where(g => g.Game.GameId == game.GameId).FirstOrDefault();

            if (line == null)
            {
                _lineCollection.Add(
                    new CartLine
                    {
                        Game = game,
                        Quantity = quantity
                    });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Game game)
        {
            _lineCollection.RemoveAll(g => g.Game.GameId == game.GameId);
        }

        public void Clear()
        {
            _lineCollection.Clear();
        }

        public decimal ComputeTotalValue()
        {
            return _lineCollection.Sum(e => e.Game.Price*e.Quantity);
        }

        public IEnumerable<CartLine> Lines
        {
            get { return _lineCollection; }
        }
    }

    public class CartLine
    {
        public Game Game { get; set; }
        public int Quantity { get; set; }
    }
}
