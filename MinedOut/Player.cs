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
        private readonly GameScene scene;
        public int X { get; set; }
        public int Y { get; set; }

        private bool AtExitPos => X == Minefield.SizeX - 1 && Y == Minefield.SizeY - 1;
        private bool BombAtPos => scene.Minefield.GetTile(X, Y) is MineTile;
        private readonly Color groundColor = new Color(0xC0, 0xC0, 0xC0);

        public Player(GameScene scene)
        {
            this.scene = scene;
        }

        public void Update()
        {
            UpdatePosition();

            if (BombAtPos)
            {
                scene.Lose();
            }

            if (AtExitPos)
            {
                scene.Win();
            }
        }

        private void UpdatePosition()
        {
            //get next pos
            var nextPosX = X + (Controls.MoveRt ? 1 : 0) + (Controls.MoveLf ? -1 : 0);
            var nextPosY = Y + (Controls.MoveDn ? 1 : 0) + (Controls.MoveUp ? -1 : 0);

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

        public void Draw(DrawCommandCollection drawCmds)
        {
            var drawCmd = new DrawCommand(X, Y, (char)0x02, Color.Red, groundColor);
            drawCmds.Add(drawCmd);
        }
    }
}
