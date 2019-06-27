using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL_1_Parser.Core
{
    class Grammar
    {
        public Symbol startingSymbol;
        public List<Symbol> terminalSymbols;
        public List<Symbol> nonTerminalSymbols;
        public List<Production> productions;

        public Grammar(Symbol starting)
        {
            startingSymbol = starting;
            terminalSymbols = new List<Symbol>();
            nonTerminalSymbols = new List<Symbol>();
            productions = new List<Production>();
        }

        public void AddTerminalSymbols(string symbols)
        {
            foreach (char c in symbols)
                terminalSymbols.Add(new Symbol(c));
        }

        public void AddNonTerminalSymbols(string symbols)
        {
            foreach (char c in symbols)
                nonTerminalSymbols.Add(new Symbol(c));
        }

        public bool IsTerminal(Symbol symbol)
        {
            return terminalSymbols.Any(s => s.rep == symbol.rep);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Starting: " + startingSymbol.rep);
            builder.Append("Terminals: ");

            foreach (var s in terminalSymbols)
                builder.Append(s.rep);
            builder.AppendLine();

            builder.Append("NonTerminals: ");
            foreach (var s in nonTerminalSymbols)
                builder.Append(s.rep);
            builder.AppendLine();

            builder.AppendLine("Productions: ");
            for(int i = 0; i < productions.Count; i++)
                builder.AppendLine((i+1).ToString() + ". " + productions[i].ToString());
            
            return builder.ToString();
        }
    }
}
