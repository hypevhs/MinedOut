using SFML.Window;

namespace MinedOut
{
    internal static class Controls
    {
        public static bool MoveUp => currMoveUp && !lastMoveUp && !currFlag;
        public static bool MoveDn => currMoveDn && !lastMoveDn && !currFlag;
        public static bool MoveLf => currMoveLf && !lastMoveLf && !currFlag;
        public static bool MoveRt => currMoveRt && !lastMoveRt && !currFlag;
        public static bool FlagUp => currMoveUp && !lastMoveUp && currFlag;
        public static bool FlagDn => currMoveDn && !lastMoveDn && currFlag;
        public static bool FlagLf => currMoveLf && !lastMoveLf && currFlag;
        public static bool FlagRt => currMoveRt && !lastMoveRt && currFlag;

        private static bool lastMoveUp;
        private static bool lastMoveDn;
        private static bool lastMoveLf;
        private static bool lastMoveRt;
        private static bool currMoveUp;
        private static bool currMoveDn;
        private static bool currMoveLf;
        private static bool currMoveRt;
        private static bool currFlag;

        public static void Update()
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
}