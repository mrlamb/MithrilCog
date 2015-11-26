using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public interface IDrawElement
    {
        void Draw();

        int Z { get; }
    }

    public class DrawElementComparer : IComparer<IDrawElement>
    {
        public int Compare(IDrawElement x, IDrawElement y)
        {
            return x.Z.CompareTo(y.Z);
        }
    }
}
