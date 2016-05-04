using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window;

namespace MinedOut
{
    internal interface IControls
    {
        bool MoveUp { get; }
        bool MoveDn { get; }
        bool MoveLf { get; }
        bool MoveRt { get; }
        bool FlagUp { get; }
        bool FlagDn { get; }
        bool FlagLf { get; }
        bool FlagRt { get; }

        void Update();
    }

    internal class HumanControls : IControls
    {
        public bool MoveUp => currMoveUp && !lastMoveUp && !currFlag;
        public bool MoveDn => currMoveDn && !lastMoveDn && !currFlag;
        public bool MoveLf => currMoveLf && !lastMoveLf && !currFlag;
        public bool MoveRt => currMoveRt && !lastMoveRt && !currFlag;
        public bool FlagUp => currMoveUp && !lastMoveUp && currFlag;
        public bool FlagDn => currMoveDn && !lastMoveDn && currFlag;
        public bool FlagLf => currMoveLf && !lastMoveLf && currFlag;
        public bool FlagRt => currMoveRt && !lastMoveRt && currFlag;

        private bool lastMoveUp;
        private bool lastMoveDn;
        private bool lastMoveLf;
        private bool lastMoveRt;
        private bool currMoveUp;
        private bool currMoveDn;
        private bool currMoveLf;
        private bool currMoveRt;
        private bool currFlag;

        public void Update()
        {
            lastMoveUp = currMoveUp;
            lastMoveDn = currMoveDn;
            lastMoveLf = currMoveLf;
            lastMoveRt = currMoveRt;

            currMoveUp = Keyboard.IsKeyPressed(Keyboard.Key.Up);
            currMoveDn = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            currMoveLf = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            currMoveRt = Keyboard.IsKeyPressed(Keyboard.Key.Right);
            currFlag = Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift);
        }
    }

    internal class AiControls : IControls
    {
        private readonly AiPlayer plr;
        private readonly Minefield field;
        private readonly int?[,] mineCounterMatrix;
        private HashSet<Tile> explorePls;
        public bool MoveUp { get; private set; }
        public bool MoveDn { get; private set; }
        public bool MoveLf { get; private set; }
        public bool MoveRt { get; private set; }
        public bool FlagUp { get; private set; }
        public bool FlagDn { get; private set; }
        public bool FlagLf { get; private set; }
        public bool FlagRt { get; private set; }

        public AiControls(AiPlayer plr, Minefield field)
        {
            this.plr = plr;
            this.field = field;
            mineCounterMatrix = new int?[Minefield.SizeX, Minefield.SizeY];
            explorePls = new HashSet<Tile>();
        }

        private bool lastAdvance;
        private bool currAdvance;
        public void Update()
        {
            //read something from plr and set the bools
            //ai phase 1: travel where mine counter is 0

            lastAdvance = currAdvance;
            currAdvance = Keyboard.IsKeyPressed(Keyboard.Key.A);

            ResetControls();
            if (currAdvance && !lastAdvance)
            {
                UpdatePriorities();
                UpdateControls();
                //exploreThese.Count should be 2 and then something else idk
            }
        }

        private void ResetControls()
        {
            MoveUp = false;
            MoveDn = false;
            MoveLf = false;
            MoveRt = false;
            FlagUp = false;
            FlagDn = false;
            FlagLf = false;
            FlagRt = false;
        }

        private void UpdateControls()
        {
            //get explore request, sorted by distance ascending
            var exploreThese = explorePls.ToList().OrderBy(DistanceFromPlayer).ToList();
            var canMoveToInOneStep = exploreThese.Where(MagnitudeFromPlayerIsOne);
            var exploreHere = canMoveToInOneStep.First();
            explorePls.Remove(exploreHere);
            exploreThese.Remove(exploreHere);

            var moveMeX = exploreHere.X - plr.X;
            var moveMeY = exploreHere.Y - plr.Y;

            if (moveMeX == 1) MoveRt = true;
            if (moveMeX ==-1) MoveLf = true;
            if (moveMeY == 1) MoveDn = true;
            if (moveMeY ==-1) MoveUp = true;
        }

        private bool MagnitudeFromPlayerIsOne(Tile tile)
        {
            return DistanceFromPlayer(tile) == 1;
        }

        private int DistanceFromPlayer(Tile tile)
        {
            //taxicab distance
            return
                Math.Abs(tile.X - plr.X) +
                Math.Abs(tile.Y - plr.Y);
        }

        private void UpdatePriorities()
        {
            //save current count to MCM
            mineCounterMatrix[plr.X, plr.Y] = GetAdjMineCount(plr.X, plr.Y);

            var mcmWhereX = plr.X;
            var mcmWhereY = plr.Y;
            int? mcmHere = mineCounterMatrix[mcmWhereX, mcmWhereY].Value;

            //if the counter is 0, explore all unexplored neighbors
            if (mcmHere == 0)
            {
                var adjUnexplored = GetAdjUnexplored(mcmWhereX, mcmWhereY);
                PleaseExplore(adjUnexplored);
            }
        }

        private void PleaseExplore(IEnumerable<Tile> where)
        {
            explorePls.UnionWith(where);
        }

        private IEnumerable<Tile> GetAdjUnexplored(int x, int y)
        {
            var found = field.GetAdjacent(x, y).Where(t => t.Dug == false);
            return found;
        }

        private int GetAdjMineCount(int x, int y)
        {
            return field.GetAdjacentMineCount(x, y);
        }
    }
}