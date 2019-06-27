using LL_1_Parser.Core;
using LL_1_Parser.Parsing;
using System;

namespace LL_1_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Grammar g = new Grammar(new Symbol('E'));

            g.AddNonTerminalSymbols("EVPFLG");
            g.AddTerminalSymbols("+-*a()e");

            g.productions.Add(Production.FromString("E->VP"));
            g.productions.Add(Production.FromString("P->+V"));
            g.productions.Add(Production.FromString("P->e"));
            g.productions.Add(Production.FromString("V->-V"));
            g.productions.Add(Production.FromString("V->F"));
            g.productions.Add(Production.FromString("F->aLG"));
            g.productions.Add(Production.FromString("F->(E)G"));
            g.productions.Add(Production.FromString("L->(E)"));
            g.productions.Add(Production.FromString("L->e"));
            g.productions.Add(Production.FromString("G->*G"));
            g.productions.Add(Production.FromString("G->e"));

            ParsingTable parsingTable = new ParsingTable(g);

            Console.WriteLine(g.ToString());
            Console.WriteLine();
            Console.WriteLine(parsingTable.ProductionsInfo());
            Console.WriteLine();
            Console.WriteLine(parsingTable.ToString());
            Console.WriteLine();

            Parser.Parse(parsingTable, g, "(-a+a(a*))+a");

            Console.ReadKey();
            
        }
    }
}
