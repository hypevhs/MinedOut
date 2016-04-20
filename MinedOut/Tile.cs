using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace MinedOut
{
    internal abstract class Tile : IGameDrawable
    {
        private int X { get; }
        private int Y { get; }

        protected Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public List<DrawCommand> Draw()
        {
            return new List<DrawCommand>
            {
                new DrawCommand(X, Y, '*', Color.Red, Color.Cyan)
            };
        }
    }

    internal class MineTile : Tile
    {
        public MineTile(int x, int y) : base(x, y)
        {
        }
    }
}
