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
        public const int SizeX = 54;
        public const int SizeY = 19;
        public const int NumMines = 200;

        private readonly Tile[,] tiles;

        public Minefield()
        {
            tiles = new Tile[SizeX, SizeY];

            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    tiles[x, y] = new GroundTile(x, y);
                }
            }

            //now place mines
            for (var i = 0; i < NumMines; i++)
            {
                PlaceMine();
            }
        }

        private void PlaceMine()
        {
            var r = new Random();

            //get random coord
            int x;
            int y;

            while (true)
            {
                x = r.Next(SizeX);
                y = r.Next(SizeY);

                //places not to put a mine: start, end, somewhere where a mine already exists
                var badEnter = x < 3 && y < 3;
                var badExit = x > SizeX - 4 && y > SizeY - 4;
                var badAlready = tiles[x, y] is MineTile;
                if (!badEnter && !badExit && !badAlready)
                    break;
            }

            //place mine
            tiles[x, y] = new MineTile(x, y);
        }

        public Tile GetTile(int x, int y)
        {
            //TODO range check
            return tiles[x, y];
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
