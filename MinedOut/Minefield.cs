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
        private readonly GameScene scene;
        public const int SizeX = 54;
        public const int SizeY = 19;
        public const int NumMines = 125;

        private readonly Tile[,] tiles;

        public Minefield(GameScene scene)
        {
            this.scene = scene;
            tiles = new Tile[SizeX, SizeY];

            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    tiles[x, y] = new GroundTile(x, y, scene);
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
            //get random coord
            int x;
            int y;

            while (true)
            {
                x = RandomProvider.Random.Next(SizeX);
                y = RandomProvider.Random.Next(SizeY);

                //places not to put a mine: start, end, somewhere where a mine already exists
                var badEnter = x < 3 && y < 3;
                var badExit = x > SizeX - 4 && y > SizeY - 4;
                var badAlready = tiles[x, y] is MineTile;
                if (!badEnter && !badExit && !badAlready)
                    break;
            }

            //place mine
            tiles[x, y] = new MineTile(x, y, scene);
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

            DrawExit(drawCmds);
        }

        private void DrawExit(DrawCommandCollection drawCmds)
        {
            var animation = @"-\|/";
            var drawnChar = animation[scene.FrameCounter/6 % animation.Length];
            drawCmds.Add(new DrawCommand(SizeX - 1, SizeY - 1, drawnChar, Color.Yellow, Color.Transparent));
        }

        public bool IsInRange(int x, int y)
        {
            return x >= 0 && x < SizeX
                && y >= 0 && y < SizeY;
        }

        public int GetAdjacentMines(int x, int y)
        {
            var count = 0;
            for (var dy = -1; dy <= 1; dy++)
            {
                for (var dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    var thisX = x + dx;
                    var thisY = y + dy;
                    if (!IsInRange(thisX, thisY))
                        continue;
                    if (tiles[thisX, thisY] is MineTile)
                        count++;
                }
            }
            return count;
        }

        public void SetDrawMines(bool draw)
        {
            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    tiles[x, y].DrawMines = true;
                }
            }
        }
    }
}
