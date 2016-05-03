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

        private void UpdatePosition()
        {
            //get next pos
            var nextPosX = X + (ControlsMoveRt ? 1 : 0) + (ControlsMoveLf ? -1 : 0);
            var nextPosY = Y + (ControlsMoveDn ? 1 : 0) + (ControlsMoveUp ? -1 : 0);

            //move if no wall blocking our way
            if (CanWalk(nextPosX, nextPosY))
            {
                X = nextPosX;
                Y = nextPosY;
            }
        }

        private bool CanWalk(int nextPosX, int nextPosY)
        {
            return nextPosX >= 0 && nextPosX < Minefield.SizeX
                && nextPosY >= 0 && nextPosY < Minefield.SizeY;
        }

        private void GetControls()
        {
            ControlsMoveUp = Keyboard.IsKeyPressed(Keyboard.Key.Up);
            ControlsMoveDn = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            ControlsMoveLf = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            ControlsMoveRt = Keyboard.IsKeyPressed(Keyboard.Key.Right);
        }

        public void Draw(DrawCommandCollection drawCmds)
        {
            var drawCmd = new DrawCommand(X, Y, (char)0x02, Color.Red, groundColor);
            drawCmds.Add(drawCmd);
        }
    }
}
