using System.IO;
using SFML.Graphics;
using SFML.System;

namespace MinedOut
{
    internal struct TextBufferEntry
    {
        public char WrittenChar { get; }
        public Color Fore { get; }
        public Color Back { get; }

        public TextBufferEntry(char writtenChar, Color fore, Color back)
        {
            WrittenChar = writtenChar;
            Fore = fore;
            Back = back;
        }
    }

    internal class TextBuffer : Drawable
    {
        private static Shader textBufferShader;

        public int BufferWidth { get; }
        public int BufferHeight { get; }
        public int CharWidth { get; private set; }
        public int CharHeight { get; private set; }
        public Sprite Sprite { get; }

        private bool dirty;
        private readonly RenderTexture rt;
        private readonly Vertex[] screenQuad;
        private RenderStates textStates;
        private readonly Texture foreData;
        private readonly Texture backData;
        private Texture asciiFont;
        private readonly byte[] foreDataRaw;
        private readonly byte[] backDataRaw;

        public TextBuffer(uint w, uint h, Texture fnt, int charW, int charH)
        {
            BufferWidth = (int)w;
            BufferHeight = (int)h;
            asciiFont = fnt;
            dirty = true;
            CharWidth = charW;
            CharHeight = charH;

            foreDataRaw = new byte[w * h * 4];
            foreData = new Texture(new Image(w, h, foreDataRaw)) {Smooth = false};
            backDataRaw = new byte[w * h * 4];
            backData = new Texture(new Image(w, h, backDataRaw)) {Smooth = false};

            rt = new RenderTexture(w * (uint)CharWidth, h * (uint)CharHeight);
            rt.Texture.Smooth = true;
            Sprite = new Sprite(rt.Texture);
            if (textBufferShader == null)
            {
                var vertText = File.ReadAllText("content/textbuffer.vert");
                var fragText = File.ReadAllText("content/textbuffer.frag");
                textBufferShader = Shader.FromString(vertText, fragText);
            }
            textStates = new RenderStates(textBufferShader);

            screenQuad = new[] {
                new Vertex(new Vector2f(0, 0), Color.White, new Vector2f(0, 0)),
                new Vertex(new Vector2f(rt.Size.X, 0), Color.White, new Vector2f(1, 0)),
                new Vertex(new Vector2f(rt.Size.X, rt.Size.Y), Color.White, new Vector2f(1, 1)),
                new Vertex(new Vector2f(0, rt.Size.Y), Color.White, new Vector2f(0, 1)),
            };

            Clear();
        }

        public void Set(int x, int y, char c, Color fg, Color bg)
        {
            Set(y * BufferWidth + x, c, fg, bg);
        }

        public void Set(int x, int y, Color fg, Color bg)
        {
            Set(y * BufferWidth + x, fg, bg);
        }

        public void Set(int idx, Color fg, Color bg)
        {
            idx *= 4;
            foreDataRaw[idx] = fg.R;
            foreDataRaw[idx + 1] = fg.G;
            foreDataRaw[idx + 2] = fg.B;
            backDataRaw[idx] = bg.R;
            backDataRaw[idx + 1] = bg.G;
            backDataRaw[idx + 2] = bg.B;
            backDataRaw[idx + 3] = bg.A;
            dirty = true;
        }

        public void Set(int idx, char c, Color fg, Color bg)
        {
            Set(idx, fg, bg);
            foreDataRaw[idx * 4 + 3] = (byte)c;
            dirty = true;
        }

        public TextBufferEntry Get(int x, int y)
        {
            return Get(y * BufferWidth + x);
        }

        public TextBufferEntry Get(int idx)
        {
            idx *= 4;
            return new TextBufferEntry((char)foreDataRaw[idx + 3],
                new Color(foreDataRaw[idx], foreDataRaw[idx + 1], foreDataRaw[idx + 2]),
                new Color(backDataRaw[idx], backDataRaw[idx + 1], backDataRaw[idx + 2], backDataRaw[idx + 3]));
        }

        public TextBufferEntry this[int idx]
        {
            get
            {
                return Get(idx);
            }
            set
            {
                Set(idx, value.WrittenChar, value.Fore, value.Back);
            }
        }

        public TextBufferEntry this[int x, int y]
        {
            get
            {
                return Get(x, y);
            }
            set
            {
                Set(x, y, value.WrittenChar, value.Fore, value.Back);
            }
        }

        public void Clear(char c = (char)0)
        {
            Clear(c, Color.White, Color.Black);
        }

        public void Clear(char c, Color fg, Color bg)
        {
            for (int i = 0; i < BufferWidth * BufferHeight; i++)
                Set(i, c, fg, bg);
        }

        public void Print(int x, int y, string str)
        {
            Print(y * BufferWidth + x, str);
        }

        public void Print(int x, int y, string str, Color fg, Color bg)
        {
            Print(y * BufferWidth + x, str, fg, bg);
        }

        public void Print(int I, string str)
        {
            Print(I, str, new Color(192, 192, 192), Color.Black);
        }

        public void Print(int I, string str, Color fg, Color bg)
        {
            for (var i = 0; i < str.Length; i++)
                Set(I + i, str[i], fg, bg);
        }

        private void Update()
        {
            if (!dirty)
                return;
            dirty = false;
            foreData.Update(foreDataRaw);
            backData.Update(backDataRaw);

            textStates.Shader.SetParameter("font", asciiFont);
            textStates.Shader.SetParameter("foredata", foreData);
            textStates.Shader.SetParameter("backdata", backData);
            textStates.Shader.SetParameter("buffersize", BufferWidth, BufferHeight);
            var fontSizeX = asciiFont.Size.X / CharWidth;
            var fontSizeY = asciiFont.Size.Y / CharHeight;
            textStates.Shader.SetParameter("fontsizes", CharWidth, CharHeight, fontSizeX, fontSizeY);

            rt.Clear(Color.Transparent);
            rt.Draw(screenQuad, PrimitiveType.Quads, textStates);
            rt.Display();
        }

        public void Draw(RenderTarget r, RenderStates s)
        {
            Update();
            Sprite.Draw(r, s);
        }
    }
}