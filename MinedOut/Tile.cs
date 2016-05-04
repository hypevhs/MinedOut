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
        protected readonly Color GroundColor = new Color(0xC0, 0xC0, 0xC0);
        protected readonly Color DugGroundColor = new Color(0x55, 0x55, 0x55);
        public int X { get; }
        public int Y { get; }
        public bool Flagged { get; set; }
        public bool Dug { get; set; }

        protected Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public abstract void Draw(DrawCommandCollection drawCmds);
    }

    internal class GroundTile : Tile
    {
        public GroundTile(int x, int y) : base(x, y)
        {
        }

        public override void Draw(DrawCommandCollection drawCmds)
        {
            var ch = Flagged ? '\xd5' : ' ';
            var drawCmd = new DrawCommand(X, Y, ch, Color.Red, Dug ? DugGroundColor : GroundColor);
            drawCmds.Add(drawCmd);
        }
    }

    internal class MineTile : Tile
    {
        public MineTile(int x, int y) : base(x, y)
        {
        }

        public override void Draw(DrawCommandCollection drawCmds)
        {
            var ch = Flagged ? '\xd5' : '*';
            var drawCmd = new DrawCommand(X, Y, ch, Color.Red, Dug ? DugGroundColor : GroundColor);
            drawCmds.Add(drawCmd);
        }
    }
}
