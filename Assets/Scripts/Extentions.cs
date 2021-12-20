using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Collections.Extensions;

namespace DefaultNamespace
{
    public static class Extentions
    {
        public static string ToMatrixString<T>(this IEnumerable<T[]> matrix, string delimiter = "\t")
        {
            var s = new StringBuilder();
            s.Append("\n");

            foreach (var t in matrix)
            {
                foreach (var t1 in t)
                {
                    s.Append(t1).Append(delimiter);
                }

                s.AppendLine();
            }

            return s.ToString();
        }

        public static string ToListString<T>(this IEnumerable<T> list)
        {
            var s = new StringBuilder();
            
            var enumerable = list as T[] ?? list.ToArray();
            var n = enumerable.Length;
            for (var i = 0; i < n; i++)
            {
                if (i!=n-1)
                {
                    s.Append(enumerable[i] + ", ");
                }
                else
                {
                    s.Append(enumerable[i]);
                }
            }

            return s.ToString();
        }
    }
    
}