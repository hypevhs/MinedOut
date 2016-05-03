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

        public void Draw(DrawCommandCollection drawCmds)
        {
            var drawCmd = new DrawCommand(X, Y, '*', Color.Red, Color.Cyan);
            drawCmds.Add(drawCmd);
        }
    }

    internal class MineTile : Tile
    {
        public MineTile(int x, int y) : base(x, y)
        {
        }
    }
}
