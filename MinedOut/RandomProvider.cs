using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinedOut
{
    internal static class RandomProvider
    {
        public static Random Random { get; }

        static RandomProvider()
        {
            Random = new Random();
        }
    }
}
