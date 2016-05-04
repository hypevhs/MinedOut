using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace MinedOut
{
    internal class PlayerBase : IGameDrawable
    {
        private GameScene scene;
        public int X { get; set; }
        public int Y { get; set; }

        private bool AtExitPos => X == Minefield.SizeX - 1 && Y == Minefield.SizeY - 1;
        private bool BombAtPos => scene.Minefield.GetTile(X, Y) is MineTile;
        private readonly Color playerColor = Color.White;
        protected IControls controls;

        protected PlayerBase(GameScene scene)
        {
            this.scene = scene;
            MakeDigMark();
        }

        public void Update()
        {
            controls.Update();
            UpdatePosition();
            MakeDigMark();
            PlaceFlags();

            if (BombAtPos)
            {
                scene.IsLose = true;
            }

            if (AtExitPos)
            {
                scene.IsWin = true;
            }
        }

        private void MakeDigMark()
        {
            var tile = scene.Minefield.GetTile(X, Y);
            tile.Dug = true;
        }

        private void PlaceFlags()
        {
            if (controls.FlagUp)
                ToggleFlagAt(X, Y - 1);
            if (controls.FlagDn)
                ToggleFlagAt(X, Y + 1);
            if (controls.FlagLf)
                ToggleFlagAt(X - 1, Y);
            if (controls.FlagRt)
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
            var nextPosX = X;
            var nextPosY = Y;
            if (controls.MoveLf || controls.MoveRt) //prioritize LR in the event of a diagonal input
                nextPosX += (controls.MoveRt ? 1 : 0) + (controls.MoveLf ? -1 : 0);
            else
                nextPosY += (controls.MoveDn ? 1 : 0) + (controls.MoveUp ? -1 : 0);

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
            var drawCmd = new DrawCommand(X, Y, '\x02', playerColor, Color.Transparent);
            drawCmds.Add(drawCmd);
        }
    }

    internal class HumanPlayer : PlayerBase
    {
        public HumanPlayer(GameScene scene) : base(scene)
        {
            controls = new HumanControls();
        }
    }

    internal class AiPlayer : PlayerBase
    {
        public AiPlayer(GameScene scene) : base(scene)
        {
            controls = new AiControls(this, scene.Minefield);
        }
    }
}
