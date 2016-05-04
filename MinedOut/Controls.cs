using System.Collections;
using System.Collections.Generic;
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
        public bool MoveUp { get; }
        public bool MoveDn { get; }
        public bool MoveLf { get; }
        public bool MoveRt { get; }
        public bool FlagUp { get; }
        public bool FlagDn { get; }
        public bool FlagLf { get; }
        public bool FlagRt { get; }

        public AiControls(AiPlayer plr)
        {
            this.plr = plr;
        }

        public void Update()
        {
            //read something from plr and set the bools
        }
    }
}