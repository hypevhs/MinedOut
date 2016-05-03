using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace MinedOut
{
    internal class Player : IGameDrawable
    {
        private Minefield Minefield { get; }
        public int X { get; set; }
        public int Y { get; set; }

        private bool ControlsMoveUp { get; set; }
        private bool ControlsMoveDn { get; set; }
        private bool ControlsMoveLf { get; set; }
        private bool ControlsMoveRt { get; set; }

        private bool AtExitPos => X == Minefield.SizeX - 1 && Y == Minefield.SizeY - 1;
        private readonly Color groundColor = new Color(0xC0, 0xC0, 0xC0);

        public Player(Minefield minefield)
        {
            Minefield = minefield;
        }

        public void Update()
        {
            GetControls();
            UpdatePosition();

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
