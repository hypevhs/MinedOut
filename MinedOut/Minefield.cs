using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace MinedOut
{
    internal class Minefield : IGameDrawable
    {
        private const int SizeX = 23;
        private const int SizeY = 16;

        private readonly Tile[,] tiles;

        public Minefield()
        {
            tiles = new Tile[SizeX, SizeY];

            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    tiles[x, y] = new MineTile(x, y);
                }
            }
        }

        public void Draw(DrawCommandCollection drawCmds)
        {
            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    var tile = tiles[x, y];
                    tile.Draw(drawCmds);
                }
            }
        }
    }
}
