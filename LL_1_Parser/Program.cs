using LL_1_Parser.Core;
using LL_1_Parser.Parsing;
using System;
using System.IO;

namespace LL_1_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: parser.exe <inputFile>, <outputFile> <word>");
                Console.WriteLine("Input File format:");
                Console.WriteLine("Line 1: Starting Symbol");
                Console.WriteLine("Line 2: Nonterminal symbols (e.g. \"EVPFLG\"");
                Console.WriteLine("Line 3: terminal symbols (e.g. \"+-*a()e\"), with 'e' that means epsilon");
                Console.WriteLine("Line 4: number of productions");
                Console.WriteLine("Line 5+: production (e.g. \"E->VP\" or \"P->+V\"");
                return;
            }


            StreamReader sr = new StreamReader(args[0]);
            StreamWriter wr = new StreamWriter(args[1]);

            Grammar g = new Grammar(new Symbol(sr.ReadLine()[0]));

            g.AddNonTerminalSymbols(sr.ReadLine());
            g.AddTerminalSymbols(sr.ReadLine());

            int howManyProductions = int.Parse(sr.ReadLine());
            
            for(int i = 0; i < howManyProductions; i++)
            {
                g.productions.Add(Production.FromString(sr.ReadLine()));
            }

            ParsingTable parsingTable = new ParsingTable(g);

            Console.WriteLine(g.ToString());
            Console.WriteLine();
            Console.WriteLine(parsingTable.ProductionsInfo());
            Console.WriteLine();
            Console.WriteLine(parsingTable.ToString());
            Console.WriteLine();

            wr.WriteLine(g.ToString());
            wr.WriteLine();
            wr.WriteLine(parsingTable.ProductionsInfo());
            wr.WriteLine();
            wr.WriteLine(parsingTable.ToString());
            wr.WriteLine();

            Parser.Parse(parsingTable, g, args[2], wr);

            sr.Close();
            wr.Close();

            Console.ReadKey();
            
        }
    }
}
