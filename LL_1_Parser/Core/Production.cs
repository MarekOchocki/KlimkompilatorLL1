using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL_1_Parser.Core
{
    class Production
    {
        public Symbol left;
        public List<Symbol> right;

        public Production(Symbol left_, List<Symbol> right_)
        {
            left = left_;
            right = right_;
        }

        public static Production FromString(string src)
        {
            var parts = src.Split(new string[] { "->" }, StringSplitOptions.None);

            var left = new Symbol(parts[0][0]);

            var right = new List<Symbol>();
            foreach (char s in parts[1])
                right.Add(new Symbol(s));

            return new Production(left, right);
        }

        public Symbol FirstRight()
        {
            return right[0];
        }

        public Symbol LastRight()
        {
            return right[right.Count - 1];
        }

        public bool HasSymbolOnRightSide(Symbol s)
        {
            foreach(var r in right)
            {
                if (r.rep == s.rep)
                    return true;
            }
            return false;
        }

        public int FindSymbolOnRightSide(Symbol s)
        {
            for(int i = 0; i < right.Count; i++)
            {
                if (right[i].rep == s.rep)
                    return i;
            }
            return -1;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(left.rep);
            builder.Append("->");
            foreach(var s in right)
            {
                builder.Append(s.rep);
            }

            return builder.ToString();
        }
    }
}
