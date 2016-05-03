using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace MinedOut
{
    internal class DrawCommandCollection : IEnumerable<DrawCommand>
    {
        private readonly List<DrawCommand> drawCommands;
        private readonly Stack<Camera> cameraAngles;

        public DrawCommandCollection()
        {
            cameraAngles = new Stack<Camera>();
            drawCommands = new List<DrawCommand>();
        }

        public void Add(DrawCommand item)
        {
            //translate by all camera offsets
            var offset = GetCameraOffset();
            var translatedItem = item.TranslatedClone(offset.X, offset.Y);
            drawCommands.Add(translatedItem);
        }

        public int Count => drawCommands.Count;

        public DrawCommand this[int index] => drawCommands[index];

        public void PushCamera(Camera cam)
        {
            cameraAngles.Push(cam);
        }

        public Camera PopCamera()
        {
            return cameraAngles.Pop();
        }

        private Vector2i GetCameraOffset()
        {
            var finalDx = cameraAngles.Sum(c => c.Dx);
            var finalDy = cameraAngles.Sum(c => c.Dy);
            return new Vector2i(finalDx, finalDy);
        }

        public IEnumerator<DrawCommand> GetEnumerator()
        {
            return drawCommands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(List<DrawCommand> moreCmds)
        {
            foreach (var cmd in moreCmds)
            {
                Add(cmd);
            }
        }
    }

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

        /// <summary>
        /// Creates many <see cref="DrawCommand"/>s to facilitate the drawing of a string
        /// </summary>
        public static List<DrawCommand> FromString(int x, int y, string str, Color foregroundColor, Color backgroundColor)
        {
            var list = new List<DrawCommand>(str.Length);
            for (var i = 0; i < str.Length; i++)
            {
                var newX = x + i;
                list.Add(new DrawCommand(newX, y, str[i], foregroundColor, backgroundColor));
            }
            return list;
        }

        public DrawCommand TranslatedClone(int dx, int dy)
        {
            return new DrawCommand(X + dx, Y + dy, WrittenChar, ForegroundColor, BackgroundColor);
        }
    }

    internal interface IGameDrawable
    {
        void Draw(DrawCommandCollection drawCmds);
    }

    /// <summary>
    ///     An offset of drawing position. Suppose you have an object with a child object with a
    ///     child object. If you wanted to move the top object around, you'd normally have to notify its children and its
    ///     children's children to offset where they normally draw. Instead, I have a stack of cameras that the object can push
    ///     onto before asking its children to draw themselves. When the children draw themselves, the position they ask for is
    ///     translated through all the cameras in the stack before being actually drawn.
    /// </summary>
    internal class Camera
    {
        public int Dx { get; }
        public int Dy { get; }

        public Camera(int dx, int dy)
        {
            Dx = dx;
            Dy = dy;
        }
    }
}