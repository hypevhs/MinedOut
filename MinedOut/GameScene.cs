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
        private readonly Color borderColor = new Color(0x00, 0x00, 0x7E);
        private readonly Color menuColor = new Color(0x00, 0x00, 0xA8);
        private readonly Color menuBackColorA = new Color(0xA8, 0xA8, 0xA8);
        private readonly Color menuBackColorB = new Color(0x00, 0xA8, 0xA8);
        public Minefield Minefield { get; }
        private readonly TextBuffer buffer;
        private readonly Player player;
        private bool IsWin { get; set; }
        private bool IsLose { get; set; }

        public GameScene()
        {
            buffer = new TextBuffer(TerminalW, TerminalH, new Texture("content/fontdos.png"), 9, 16);

            Minefield = new Minefield();
            player = new Player(this);
        }

        public void Update()
        {
            Controls.Update();
            if (!IsWin && !IsLose)
            {
                player.Update();
            }
        }

        public void Win()
        {
            IsWin = true;
        }

        public void Lose()
        {
            IsLose = true;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var drawCmds = new DrawCommandCollection();

            drawCmds.PushCamera(new Camera(1, 1));
            DrawGround(drawCmds);
            drawCmds.PushCamera(new Camera(1, 1));
            DrawGate(drawCmds);
            drawCmds.PushCamera(new Camera(1, 1));
            Minefield.Draw(drawCmds);
            player.Draw(drawCmds);
            drawCmds.PopCamera();
            drawCmds.PopCamera();
            drawCmds.PopCamera();

            drawCmds.PushCamera(new Camera(BorderWidth, 0));
            DrawGui(drawCmds);
            drawCmds.PopCamera();

            DrawBorder(drawCmds);
            ProcessDrawCommands(target, states, drawCmds);
        }

        private void DrawGui(DrawCommandCollection drawCmds)
        {
            const int width = 20;
            
            drawCmds.AddRange(DrawCommand.FromString(2, 1, "     - - -      ", menuBackColorA, menuColor));
            drawCmds.AddRange(DrawCommand.FromString(2, 2, "   MINED OUT!   ", Color.Black, menuBackColorA));
            drawCmds.AddRange(DrawCommand.FromString(2, 3, "     - - -      ", menuBackColorA, menuColor));

            drawCmds.AddRange(DrawCommand.FromString(4, 15, "  CONTROLS  ", Color.Black, menuBackColorA));

            drawCmds.AddRange(DrawCommand.FromString(7, 17, " \x18\x19\x1A\x1B", Color.Black, menuBackColorB));
            drawCmds.AddRange(DrawCommand.FromString(13, 17, "Move", Color.White, menuColor));

            drawCmds.AddRange(DrawCommand.FromString(1, 18, " Shift \x18\x19\x1A\x1B", Color.Black, menuBackColorA));
            drawCmds.AddRange(DrawCommand.FromString(13, 18, "Flag", Color.White, menuColor));
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