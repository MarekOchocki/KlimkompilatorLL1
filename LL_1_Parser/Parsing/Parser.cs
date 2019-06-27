using LL_1_Parser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LL_1_Parser.Parsing
{
    class Parser
    {
        public static void Parse(ParsingTable table, Grammar grammar, string word, StreamWriter log)
        {
            var symbols = new List<Symbol>();
            symbols.Add(grammar.startingSymbol);

            while(true)
            {
                log.Write("[" + word + ", " + symbolListToString(symbols) + "$, ");
                Console.Write("[" + word + ", " + symbolListToString(symbols) + "$, ");


                if (symbols.Count == 0)
                {
                    if(word.Length == 0)
                    {
                        log.WriteLine("acc]");
                        Console.WriteLine("acc]");
                    }
                    else
                    {
                        log.WriteLine("error]");
                        Console.WriteLine("error]");
                    }
                    return;
                }

                if(grammar.IsTerminal(symbols[0]))
                {
                    if(word.Length == 0)
                    {
                        log.WriteLine("error]");
                        Console.WriteLine("error]");
                        return;
                    }

                    if(word[0] == symbols[0].rep)
                    {
                        log.WriteLine("pop " + word[0] + "]");
                        Console.WriteLine("pop " + word[0] + "]");
                        symbols.RemoveAt(0);
                        word = word.Remove(0, 1);
                    }
                }else
                {
                    Production production = null;
                    int prodNumber = 0;

                    if (word.Length == 0)
                        production = table.GetProductionFor(symbols[0], new Symbol('e'), ref prodNumber);
                    else
                        production = table.GetProductionFor(symbols[0], new Symbol(word[0]), ref prodNumber);

                    if(production == null)
                    {
                        log.WriteLine("error]");
                        Console.WriteLine("error]");
                        return;
                    }

                    log.WriteLine(prodNumber.ToString() + "]");
                    Console.WriteLine(prodNumber.ToString() + "]");

                    symbols.RemoveAt(0);
                    symbols.InsertRange(0, production.right);
                    symbols.RemoveAll(s => s.rep == 'e');
                }
            }
            
        }

        private static string symbolListToString(List<Symbol> symbols)
        {
            var builder = new StringBuilder();

            foreach (var s in symbols)
                builder.Append(s.rep);
            return builder.ToString();
        }
    }
}
