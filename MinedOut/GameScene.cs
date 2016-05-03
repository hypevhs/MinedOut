using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace MinedOut
{
    internal class GameScene : Drawable
    {
        private readonly TextBuffer buffer;
        private readonly Minefield minefield;

        public GameScene()
        {
            buffer = new TextBuffer(80, 24);
            buffer.SetFontTexture(new Texture("content/font.png"));

            minefield = new Minefield();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            DrawCommandCollection drawCmds = new DrawCommandCollection();
            minefield.Draw(drawCmds);

            ProcessDrawCommands(target, states, drawCmds);
        }

        private void ProcessDrawCommands(RenderTarget target, RenderStates states, DrawCommandCollection drawCmds)
        {
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
