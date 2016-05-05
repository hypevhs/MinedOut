using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace MinedOut
{
    internal abstract class Tile
    {
        protected Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
        public bool Flagged { get; set; }
        public bool Dug { get; set; }

        public override string ToString()
        {
            return $"Tile: {X}, {Y}";
        }

        public int DistanceTo(Tile other)
        {
            return DistanceTo(other.X, other.Y);
        }

        public int DistanceTo(int x, int y)
        {
            //taxicab distance
            return
                Math.Abs(X - x) +
                Math.Abs(Y - y);
        }

        public Vector2i ToVector2i()
        {
            return new Vector2i(X, Y);
        }
    }

    internal abstract class DrawableTile : Tile, IGameDrawable
    {
        protected readonly Color GroundColor = new Color(0xC0, 0xC0, 0xC0);
        protected readonly Color DugGroundColor = new Color(0x55, 0x55, 0x55);
        protected Color BackgroundColor
        {
            get
            {
                var col = Dug ? DugGroundColor : GroundColor;
                if ((X + Y) % 2 == 0)
                {
                    col = new Color(
                        (byte)(col.R * .95f),
                        (byte)(col.G * .95f),
                        (byte)(col.B * .95f)
                    );
                }
                return col;
            }
        }

        protected DrawableTile(int x, int y) : base(x, y)
        {
        }

        public abstract void Draw(DrawCommandCollection drawCmds);
    }

    internal class GroundTile : DrawableTile
    {
        public GroundTile(int x, int y) : base(x, y)
        {
        }

        public override void Draw(DrawCommandCollection drawCmds)
        {
            var ch = Flagged ? '\xd5' : ' ';
            var drawCmd = new DrawCommand(X, Y, ch, Color.Red, BackgroundColor);
            drawCmds.Add(drawCmd);
        }
    }

    internal class MineTile : DrawableTile
    {
        public bool DrawMines { get; set; }
        private GameScene Scene { get; }

        public MineTile(int x, int y, GameScene scene) : base(x, y)
        {
            Scene = scene;
        }

        public override void Draw(DrawCommandCollection drawCmds)
        {
            var animate = Scene.FrameCounter / 20 % 2 == 0;

            char ch = ' ';
            if (Flagged)
            {
                ch = '\xd5';
                if (DrawMines && animate)
                    ch = '*';
            }
            else if (DrawMines)
            {
                ch = '*';
            }
            
            var drawCmd = new DrawCommand(X, Y, ch, Color.Red, BackgroundColor);
            drawCmds.Add(drawCmd);
        }
    }
}
