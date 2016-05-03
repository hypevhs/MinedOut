using SFML.Window;

namespace MinedOut
{
    internal static class Controls
    {
        public static bool MoveUp => CurrMoveUp && !LastMoveUp && !CurrFlag;
        public static bool MoveDn => CurrMoveDn && !LastMoveDn && !CurrFlag;
        public static bool MoveLf => CurrMoveLf && !LastMoveLf && !CurrFlag;
        public static bool MoveRt => CurrMoveRt && !LastMoveRt && !CurrFlag;
        public static bool FlagUp => CurrMoveUp && !LastMoveUp && CurrFlag;
        public static bool FlagDn => CurrMoveDn && !LastMoveDn && CurrFlag;
        public static bool FlagLf => CurrMoveLf && !LastMoveLf && CurrFlag;
        public static bool FlagRt => CurrMoveRt && !LastMoveRt && CurrFlag;

        private static bool LastMoveUp { get; set; }
        private static bool LastMoveDn { get; set; }
        private static bool LastMoveLf { get; set; }
        private static bool LastMoveRt { get; set; }
        private static bool CurrMoveUp { get; set; }
        private static bool CurrMoveDn { get; set; }
        private static bool CurrMoveLf { get; set; }
        private static bool CurrMoveRt { get; set; }
        private static bool CurrFlag { get; set; }

        public static void Update()
        {
            LastMoveUp = CurrMoveUp;
            LastMoveDn = CurrMoveDn;
            LastMoveLf = CurrMoveLf;
            LastMoveRt = CurrMoveRt;

            CurrMoveUp = Keyboard.IsKeyPressed(Keyboard.Key.Up);
            CurrMoveDn = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            CurrMoveLf = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            CurrMoveRt = Keyboard.IsKeyPressed(Keyboard.Key.Right);
            CurrFlag = Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift);
        }
    }
}