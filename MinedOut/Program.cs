using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace MinedOut
{
    internal static class Program
    {
        private static RenderWindow window;
        private static Minefield minefield;
        private static TextBuffer buffer;

        private static void Main(string[] args)
        {
            PreRun();
            LoadContentInitialize();

            while (window.IsOpen)
            {
                UpdateDraw();
            }
        }

        private static void PreRun()
        {
            if (!Shader.IsAvailable)
            {
                Console.WriteLine("No shader available. Please update your graphics drivers.");
                Environment.Exit(1);
            }
        }

        private static void LoadContentInitialize()
        {
            window = new RenderWindow(
                new VideoMode(800, 600), "Mined Out");
            window.SetFramerateLimit(60);
            window.Closed += (obj, e) => { window.Close(); };
            window.Size = new Vector2u(800, 600);

            minefield = new Minefield();
            
            buffer = new TextBuffer(80, 24);
            buffer.SetFontTexture(new Texture("content/font.png"));
        }

        private static void UpdateDraw()
        {
            window.DispatchEvents();
            window.Clear();
            
            var drawCmds = minefield.Draw();

            foreach (var drawCommand in drawCmds)
            {
                var x = drawCommand.X;
                var y = drawCommand.Y;
                var b = drawCommand.BackgroundColor;
                var f = drawCommand.ForegroundColor;
                var c = drawCommand.WrittenChar;

                buffer.Set(x, y, c, f, b);
            }

            buffer.Draw(window, RenderStates.Default);

            window.Display();
        }
    }
}
