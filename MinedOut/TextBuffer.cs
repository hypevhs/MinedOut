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

        public TextBuffer(uint W, uint H)
        {
            BufferWidth = (int)W;
            BufferHeight = (int)H;
            dirty = true;
            CharWidth = 8;
            CharHeight = 12;

            foreDataRaw = new byte[W * H * 4];
            foreData = new Texture(new Image(W, H, foreDataRaw));
            foreData.Smooth = false;
            backDataRaw = new byte[W * H * 4];
            backData = new Texture(new Image(W, H, backDataRaw));
            backData.Smooth = false;

            rt = new RenderTexture(W * (uint)CharWidth, H * (uint)CharHeight);
            rt.Texture.Smooth = true;
            Sprite = new Sprite(rt.Texture);
            if (textBufferShader == null)
            {
                var vertText = File.ReadAllText("content/textbuffer.vert");
                var fragText = File.ReadAllText("content/textbuffer.frag");
                textBufferShader = Shader.FromString(vertText, fragText);
            }
            textStates = new RenderStates(textBufferShader);

            screenQuad = new Vertex[] {
                new Vertex(new Vector2f(0, 0), Color.White, new Vector2f(0, 0)),
                new Vertex(new Vector2f(rt.Size.X, 0), Color.White, new Vector2f(1, 0)),
                new Vertex(new Vector2f(rt.Size.X, rt.Size.Y), Color.White, new Vector2f(1, 1)),
                new Vertex(new Vector2f(0, rt.Size.Y), Color.White, new Vector2f(0, 1)),
            };

            Clear();
        }

        public void SetFontTexture(Texture Fnt, int CharW = 8, int CharH = 12)
        {
            this.CharWidth = CharW;
            this.CharHeight = CharH;
            asciiFont = Fnt;
            dirty = true;
        }

        public void Set(int X, int Y, char C, Color Fg, Color Bg)
        {
            Set(Y * BufferWidth + X, C, Fg, Bg);
        }

        public void Set(int X, int Y, Color Fg, Color Bg)
        {
            Set(Y * BufferWidth + X, Fg, Bg);
        }

        public void Set(int Idx, Color Fg, Color Bg)
        {
            Idx *= 4;
            foreDataRaw[Idx] = Fg.R;
            foreDataRaw[Idx + 1] = Fg.G;
            foreDataRaw[Idx + 2] = Fg.B;
            backDataRaw[Idx] = Bg.R;
            backDataRaw[Idx + 1] = Bg.G;
            backDataRaw[Idx + 2] = Bg.B;
            backDataRaw[Idx + 3] = Bg.A;
            dirty = true;
        }

        public void Set(int Idx, char C, Color Fg, Color Bg)
        {
            Set(Idx, Fg, Bg);
            foreDataRaw[Idx * 4 + 3] = (byte)C;
            dirty = true;
        }

        public TextBufferEntry Get(int X, int Y)
        {
            return Get(Y * BufferWidth + X);
        }

        public TextBufferEntry Get(int Idx)
        {
            Idx *= 4;
            return new TextBufferEntry((char)foreDataRaw[Idx + 3],
                new Color(foreDataRaw[Idx], foreDataRaw[Idx + 1], foreDataRaw[Idx + 2]),
                new Color(backDataRaw[Idx], backDataRaw[Idx + 1], backDataRaw[Idx + 2], backDataRaw[Idx + 3]));
        }

        public TextBufferEntry this[int Idx]
        {
            get
            {
                return Get(Idx);
            }
            set
            {
                Set(Idx, value.WrittenChar, value.Fore, value.Back);
            }
        }

        public TextBufferEntry this[int X, int Y]
        {
            get
            {
                return Get(X, Y);
            }
            set
            {
                Set(X, Y, value.WrittenChar, value.Fore, value.Back);
            }
        }

        public void Clear(char C = (char)0)
        {
            Clear(C, Color.White, Color.Black);
        }

        public void Clear(char C, Color Fg, Color Bg)
        {
            for (int i = 0; i < BufferWidth * BufferHeight; i++)
                Set(i, C, Fg, Bg);
        }

        public void Print(int X, int Y, string Str)
        {
            Print(Y * BufferWidth + X, Str);
        }

        public void Print(int X, int Y, string Str, Color Fg, Color Bg)
        {
            Print(Y * BufferWidth + X, Str, Fg, Bg);
        }

        public void Print(int I, string Str)
        {
            Print(I, Str, new Color(192, 192, 192), Color.Black);
        }

        public void Print(int I, string Str, Color Fg, Color Bg)
        {
            for (int i = 0; i < Str.Length; i++)
                Set(I + i, Str[i], Fg, Bg);
        }

        void Update()
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
            textStates.Shader.SetParameter("fontsizes", CharWidth, CharHeight, asciiFont.Size.X / CharWidth, asciiFont.Size.Y / CharHeight);

            rt.Clear(Color.Transparent);
            rt.Draw(screenQuad, PrimitiveType.Quads, textStates);
            rt.Display();
        }

        public void Draw(RenderTarget R, RenderStates S)
        {
            Update();
            Sprite.Draw(R, S);
        }
    }
}