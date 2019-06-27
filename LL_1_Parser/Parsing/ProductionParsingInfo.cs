using LL_1_Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL_1_Parser.Parsing
{
    class ProductionParsingInfo
    {
        public Production production;
        public List<Symbol> prefixes;
        public List<Symbol> follows;

        public ProductionParsingInfo(Production p)
        {
            prefixes = new List<Symbol>();
            follows = new List<Symbol>();
            production = p;
        }
    }
}
