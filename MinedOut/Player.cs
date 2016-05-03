using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace MinedOut
{
    internal class Player : IGameDrawable
    {
        public Minefield Minefield { get; }
        public int X { get; }
        public int Y { get; }
        private bool AtExitPos => X == Minefield.SizeX - 1 && Y == Minefield.SizeY - 1;
        private readonly Color groundColor = new Color(0xC0, 0xC0, 0xC0);

        public Player(Minefield minefield)
        {
            Minefield = minefield;
            X = 0;
            Y = 0;
        }

        public void Update()
        {
            if (AtExitPos)
            {
                //TODO
            }
        }

        public void Draw(DrawCommandCollection drawCmds)
        {
            var drawCmd = new DrawCommand(X, Y, (char)0x02, Color.Red, groundColor);
            drawCmds.Add(drawCmd);
        }
    }
}
