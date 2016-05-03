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
        private readonly Color groundColor = Color.Transparent;
        private readonly Color playerColor = Color.White;

        public Player(GameScene scene)
        {
            this.scene = scene;
        }

        public void Update()
        {
            UpdatePosition();
            MakeDigMark();
            PlaceFlags();

            if (BombAtPos)
            {
                scene.Lose();
            }

            if (AtExitPos)
            {
                scene.Win();
            }
        }

        private void MakeDigMark()
        {
            var tile = scene.Minefield.GetTile(X, Y);
            tile.Dug = true;
        }

        private void PlaceFlags()
        {
            if (Controls.FlagUp)
                ToggleFlagAt(X, Y - 1);
            if (Controls.FlagDn)
                ToggleFlagAt(X, Y + 1);
            if (Controls.FlagLf)
                ToggleFlagAt(X - 1, Y);
            if (Controls.FlagRt)
                ToggleFlagAt(X + 1, Y);
        }

        private void ToggleFlagAt(int x, int y)
        {
            if (!scene.Minefield.IsInRange(x, y))
                return;
            var tile = scene.Minefield.GetTile(x, y);
            tile.Flagged = !tile.Flagged;
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
            //TODO: it's a dumb mistake to walk on a flag
            return scene.Minefield.IsInRange(nextPosX, nextPosY);
        }

        public void Draw(DrawCommandCollection drawCmds)
        {
            var drawCmd = new DrawCommand(X, Y, '\x02', playerColor, groundColor);
            drawCmds.Add(drawCmd);
        }
    }
}
