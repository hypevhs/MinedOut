using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace MinedOut
{
    internal class AiControls : IControls
    {
        private readonly AiPlayer plr;
        private readonly Minefield field;
        private readonly HashSet<DrawableTile> explorePls;
        private readonly HashSet<DrawableTile> flagPls;
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
            explorePls = new HashSet<DrawableTile>();
            flagPls = new HashSet<DrawableTile>();
            pathfinder = new Pathfinder(plr, field);
        }

        private bool lastAdvance;
        private bool currAdvance;
        private bool oddFrame;
        private readonly Pathfinder pathfinder;

        public void Update()
        {
            //ai phase 1: travel where mine counter is 0

            //lastAdvance = currAdvance;
            currAdvance = Keyboard.IsKeyPressed(Keyboard.Key.A);

            ResetControls();
            if (currAdvance && !lastAdvance)
            {
                oddFrame = !oddFrame;
                if (oddFrame)
                    UpdatePriorities();
                else
                    UpdateControls();
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

        private void UpdatePriorities()
        {
            //if we're on an explore candidate, remove it from the todolist
            explorePls.RemoveWhere(t =>
                t.X == plr.X &&
                t.Y == plr.Y
            );

            //save current count to MCM
            var mcmWhereX = plr.X;
            var mcmWhereY = plr.Y;
            var mcmHere = field.GetAdjacentMineCount(mcmWhereX, mcmWhereY);

            var flaggedUndug = GetAdjacentTiles(mcmWhereX, mcmWhereY, flagged: true, dug: false).ToList();
            var unflaggedUndug = GetAdjacentTiles(mcmWhereX, mcmWhereY, flagged: false, dug: false).ToList();
            var undug = GetAdjacentTiles(mcmWhereX, mcmWhereY, flagged: null, dug: false).ToList();

            //if the counter is the same as the FlaggedUndug, explore the UnflaggedUndug
            if (mcmHere == flaggedUndug.Count)
            {
                explorePls.UnionWith(unflaggedUndug);
            }

            //if the counter is the same as the Undug, flag the UnflaggedUndug
            if (mcmHere == undug.Count)
            {
                flagPls.UnionWith(unflaggedUndug);
            }
        }

        private IEnumerable<DrawableTile> GetAdjacentTiles(int x, int y, bool? flagged, bool? dug)
        {
            var matches = field.GetAdjacent(x, y).Where(t =>
            {
                if (!flagged.HasValue)
                    return true;
                if (flagged.Value && t.Flagged)
                    return true;
                if (!flagged.Value && !t.Flagged)
                    return true;
                return false;
            }).Where(t =>
            {
                if (!dug.HasValue)
                    return true;
                if (dug.Value && t.Dug)
                    return true;
                if (!dug.Value && !t.Dug)
                    return true;
                return false;
            });
            return matches;
        }

        private void MoveTowardTarget(DrawableTile exploreTarget)
        {
            var nextStep = pathfinder.NextStepInMovingTowards(exploreTarget);

            //can reach our target in one step? remove it from todolist, then move to it
            var tileTmp = new GroundTile(nextStep.X, nextStep.Y);
            if (tileTmp.DistanceTo(plr.X, plr.Y) <= 1)
            {
                var diffX = nextStep.X - plr.X;
                var diffY = nextStep.Y - plr.Y;
                if (diffX == 1) MoveRt = true;
                if (diffX == -1) MoveLf = true;
                if (diffY == 1) MoveDn = true;
                if (diffY == -1) MoveUp = true;
            }
            else
            {
                throw new Exception("the distance is not 1, so it's a pathfinding error probably");
            }
        }
        
        private void UpdateControls()
        {
            //if possible, do <action> then end the frame
            if (TryFlagNearby())
                return;
            if (TryMoveTowardFlag())
                return;
            if (TryExplore())
                return;

            //if theres STILL nothing to do, start searching along the undug
            var alongTheUndug = field.GetAllTiles().Where(t =>
            {
                if (!t.Dug)
                    return false;
                var itsSurroundingUndug = field.GetAdjacent(t.X, t.Y).Where(t2 => !t2.Dug).ToList();
                if (itsSurroundingUndug.All(t2 => t2.Flagged))
                    return false;
                return itsSurroundingUndug.Any();
            });
            explorePls.UnionWith(alongTheUndug);
        }

        private bool TryMoveTowardFlag()
        {
            //for all places needed to be flagged, target adjacent dug tiles
            var standHereToFlag = new List<DrawableTile>();
            foreach (var flagReq in flagPls)
            {
                standHereToFlag.AddRange(field.GetCardinalAdjacent(flagReq.X, flagReq.Y).Where(t => t.Dug));
            }
            
            //and get the closest one
            var sortedFlag = standHereToFlag.OrderBy(GetDistanceScore).ToList();
            var flagTarget = sortedFlag.FirstOrDefault();
            if (flagTarget == null)
                return false;
            MoveTowardTarget(flagTarget);
            return true;
        }

        private int GetDistanceScore(DrawableTile tile)
        {
            return
                tile.DistanceTo(Minefield.SizeX - 1, Minefield.SizeY - 1)*2 +
                tile.DistanceTo(plr.X, plr.Y);
        }

        private bool TryExplore()
        {
            //get explore request, sorted by distance ascending
            var hasADuggedPathToIt = explorePls.Where(t =>
            {
                return field.GetCardinalAdjacent(t.X, t.Y).Any(t2 => t2.Dug);
            }).ToList();
            var sortedExplore = hasADuggedPathToIt.OrderBy(GetDistanceScore).ToList();
            var exploreTarget = sortedExplore.FirstOrDefault();
            if (exploreTarget == null)
                return false;
            MoveTowardTarget(exploreTarget);
            return true;
        }

        private bool TryFlagNearby()
        {
            var flagMe = field.GetCardinalAdjacent(plr.X, plr.Y).Intersect(flagPls);
            var flagThis = flagMe.FirstOrDefault();
            if (flagThis == null)
                return false;
            var diffX = flagThis.X - plr.X;
            var diffY = flagThis.Y - plr.Y;
            if (diffX == 1) FlagRt = true;
            if (diffX == -1) FlagLf = true;
            if (diffY == 1) FlagDn = true;
            if (diffY == -1) FlagUp = true;
            flagPls.Remove(flagThis);
            return true;
        }

        public void Draw(DrawCommandCollection drawCmds)
        {
            //draw the lists for debug purposes
            foreach (var expl in explorePls)
            {
                drawCmds.Add(new DrawCommand(expl.X, expl.Y, '#', Color.Yellow, Color.Transparent));
            }

            foreach (var flag in flagPls)
            {
                drawCmds.Add(new DrawCommand(flag.X, flag.Y, '#', Color.Red, Color.Transparent));
            }

            pathfinder.Draw(drawCmds);
        }
    }
}