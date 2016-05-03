using System;
using SFML.Graphics;

namespace MinedOut
{
    internal class GameScene : Drawable
    {
        private const int TerminalW = 80;
        private const int TerminalH = 25;
        private readonly TextBuffer buffer;
        private readonly Minefield minefield;

        public GameScene()
        {
            buffer = new TextBuffer(TerminalW, TerminalH, new Texture("content/fontdos.png"), 9, 16);

            minefield = new Minefield();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            DrawCommandCollection drawCmds = new DrawCommandCollection();

            drawCmds.PushCamera(new Camera(1, 1));
            minefield.Draw(drawCmds);
            drawCmds.PopCamera();

            DrawBorder(drawCmds);
            ProcessDrawCommands(target, states, drawCmds);
        }

        private void DrawBorder(DrawCommandCollection drawCmds)
        {
            const char bChar = ' ';
            var bColor = new Color(0xFC, 0xFC, 0x54);
            const int width = TerminalW - 20;
            const int height = TerminalH;

            for (var x = 0; x < width; x++)
            {
                var top = new DrawCommand(x, 0, bChar, Color.Black, bColor);
                var btm = new DrawCommand(x, height - 1, bChar, Color.Black, bColor);
                drawCmds.Add(top);
                drawCmds.Add(btm);
            }

            for (var y = 0; y < height; y++)
            {
                var left = new DrawCommand(0, y, bChar, Color.Black, bColor);
                var right = new DrawCommand(width - 1, y, bChar, Color.Black, bColor);
                drawCmds.Add(left);
                drawCmds.Add(right);
            }
        }

        private void ProcessDrawCommands(RenderTarget target, RenderStates states, DrawCommandCollection drawCmds)
        {
            buffer.Clear(' ', Color.White, new Color(0x00,0x00,0xA8));

            foreach (var drawCommand in drawCmds)
            {
                var x = drawCommand.X;
                var y = drawCommand.Y;
                var b = drawCommand.BackgroundColor;
                var f = drawCommand.ForegroundColor;
                var c = drawCommand.WrittenChar;

                buffer.Set(x, y, c, f, b);
            }

            buffer.Draw(target, states);
        }
    }
}