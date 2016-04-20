using SFML.Graphics;

namespace MinedOut
{
    internal class DrawCommand
    {
        public int X { get; }
        public int Y { get; }
        public char WrittenChar { get; }
        public Color ForegroundColor { get; }
        public Color BackgroundColor { get; }

        public DrawCommand(int x, int y, char writtenChar, Color foregroundColor, Color backgroundColor)
        {
            X = x;
            Y = y;
            WrittenChar = writtenChar;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
    }
}
