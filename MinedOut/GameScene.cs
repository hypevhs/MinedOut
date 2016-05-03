using System;
using SFML.Graphics;

namespace MinedOut
{
    internal class GameScene : Drawable
    {
        private const int TerminalW = 80;
        private const int TerminalH = 25;
        private const int BorderWidth = TerminalW - 20;
        private const int BorderHeight = TerminalH;
        private const int GroundWidth = BorderWidth - 2;
        private const int GroundHeight = BorderHeight - 2;
        private const int GateWidth = GroundWidth - 2;
        private const int GateHeight = GroundHeight - 2;
        private readonly Color groundColor = new Color(0xC0, 0xC0, 0xC0);
        private readonly Color gateColor = new Color(0xA8, 0x54, 0x00);
        private readonly Color borderColor = new Color(0xFC, 0xFC, 0x54);
        private readonly Color menuColor = new Color(0x00, 0x00, 0xA8);
        private readonly TextBuffer buffer;
        private readonly Minefield minefield;
        private readonly Player player;

        public GameScene()
        {
            buffer = new TextBuffer(TerminalW, TerminalH, new Texture("content/fontdos.png"), 9, 16);

            minefield = new Minefield();
            player = new Player(minefield);
        }

        public void Update()
        {
            player.Update();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var drawCmds = new DrawCommandCollection();

            drawCmds.PushCamera(new Camera(1, 1));
            DrawGround(drawCmds);
            drawCmds.PushCamera(new Camera(1, 1));
            DrawGate(drawCmds);
            drawCmds.PushCamera(new Camera(1, 1));
            minefield.Draw(drawCmds);
            player.Draw(drawCmds);
            drawCmds.PopCamera();
            drawCmds.PopCamera();
            drawCmds.PopCamera();

            DrawBorder(drawCmds);
            ProcessDrawCommands(target, states, drawCmds);
        }

        private void DrawGround(DrawCommandCollection drawCmds)
        {
            for (var y = 0; y < GroundHeight; y++)
            {
                for (var x = 0; x < GroundWidth; x++)
                {
                    var gnd = new DrawCommand(x, y, ' ', Color.White, groundColor);
                    drawCmds.Add(gnd);
                }
            }
        }

        private void DrawGate(DrawCommandCollection drawCmds)
        {
            const char bChar = (char)0xC5;
            var bForeColor = gateColor;
            var bBackColor = groundColor;

            for (var y = 0; y < GateHeight; y++)
            {
                for (var x = 0; x < GateWidth; x++)
                {
                    if (x + y >= GateHeight+GateWidth-3 || x + y < 2)
                    {
                        continue;
                    }
                    var gate = new DrawCommand(x, y, bChar, bForeColor, bBackColor);
                    drawCmds.Add(gate);
                }
            }
        }

        private void DrawBorder(DrawCommandCollection drawCmds)
        {
            const char bChar = ' ';
            var bColor = borderColor;

            for (var x = 0; x < BorderWidth; x++)
            {
                var top = new DrawCommand(x, 0, bChar, Color.Black, bColor);
                var btm = new DrawCommand(x, BorderHeight - 1, bChar, Color.Black, bColor);
                drawCmds.Add(top);
                drawCmds.Add(btm);
            }

            for (var y = 0; y < BorderHeight; y++)
            {
                var left = new DrawCommand(0, y, bChar, Color.Black, bColor);
                var right = new DrawCommand(BorderWidth - 1, y, bChar, Color.Black, bColor);
                drawCmds.Add(left);
                drawCmds.Add(right);
            }
        }

        private void ProcessDrawCommands(RenderTarget target, RenderStates states, DrawCommandCollection drawCmds)
        {
            var clearColor = menuColor;
            buffer.Clear(' ', Color.White, clearColor);

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