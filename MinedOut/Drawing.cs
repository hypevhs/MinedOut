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
            item.X += offset.X;
            item.Y += offset.Y;
            drawCommands.Add(item);
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
    }

    internal class DrawCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char WrittenChar { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public DrawCommand(int x, int y, char writtenChar, Color foregroundColor, Color backgroundColor)
        {
            X = x;
            Y = y;
            WrittenChar = writtenChar;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
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
        public int Dx { get; private set; }
        public int Dy { get; private set; }

        public Camera(int dx, int dy)
        {
            Dx = dx;
            Dy = dy;
        }
    }
}