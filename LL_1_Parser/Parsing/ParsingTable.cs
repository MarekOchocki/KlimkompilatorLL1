using LL_1_Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL_1_Parser.Parsing
{
    class ParsingTable
    {
        public List<ProductionParsingInfo> pInfo;

        private Dictionary<char, List<Symbol>> prefixes_;
        private Dictionary<char, List<Symbol>> trivialFollows_;
        private Grammar grammar;

        public ParsingTable(Grammar grammar)
        {
            this.grammar = grammar;
            CheckGrammar(grammar);

            pInfo = new List<ProductionParsingInfo>();
            for(int i = 0; i < grammar.productions.Count; i++)
            {
                pInfo.Add(new ProductionParsingInfo(grammar.productions[i]));
            }

            prefixes_ = new Dictionary<char, List<Symbol>>();
            trivialFollows_ = new Dictionary<char, List<Symbol>>();

            FindPrefixesForNonTerminals(grammar);
            FindTrivialFollowsForNonTerminals(grammar);

            FindPrefixes(grammar);
            CheckPrefixes();

            FindFollows(grammar);
        }

        private void CheckGrammar(Grammar grammar)
        {
            for(int i = 0; i < grammar.productions.Count; i++)
            {
                if (grammar.productions[i].left.rep == grammar.productions[i].right[0].rep)
                    throw new ArgumentException("leftside recursion in production: " + grammar.productions[i].ToString());

                for(int j = i+1; j < grammar.productions.Count; j++)
                {
                    if(grammar.productions[i].left.rep == grammar.productions[j].left.rep &&
                        grammar.productions[i].right[0].rep == grammar.productions[j].right[0].rep)
                    {
                        throw new ArgumentException("leftside factorization in productions: " + grammar.productions[i].ToString() + " and " + grammar.productions[j].ToString());
                    }
                }
            }
        }

        private void FindPrefixesForNonTerminals(Grammar grammar)
        {
            foreach(var symb in grammar.nonTerminalSymbols)
            {
                var foundPrefixes = new List<Symbol>();

                var checkedProductions = new List<bool>();
                for (int j = 0; j < grammar.productions.Count; j++)
                    checkedProductions.Add(false);

                var productionsToCheck = new List<int>();
                for(int i = 0; i < grammar.productions.Count; i++)
                {
                    if(grammar.productions[i].left.rep == symb.rep)
                    {
                        productionsToCheck.Add(i);
                    }
                }

                while (productionsToCheck.Count > 0)
                {
                    Production checking = grammar.productions[productionsToCheck[0]];

                    if (grammar.IsTerminal(checking.right[0]))
                    {
                        foundPrefixes.Add(checking.right[0]);
                    }
                    else
                    {
                        for (int j = 0; j < grammar.productions.Count; j++)
                        {
                            if (!checkedProductions[j] &&
                                checking.right[0].rep == pInfo[j].production.left.rep)
                            {
                                productionsToCheck.Add(j);
                            }
                        }
                    }

                    checkedProductions[productionsToCheck[0]] = true;
                    productionsToCheck.RemoveAt(0);
                }

                prefixes_.Add(symb.rep, foundPrefixes);
            }
        }

        private void FindTrivialFollowsForNonTerminals(Grammar grammar)
        {
            foreach (var symb in grammar.nonTerminalSymbols)
            {
                var foundFollows = new List<Symbol>();

                for(int i = 0; i < grammar.productions.Count; i++)
                {
                    Production checking = grammar.productions[i];

                    if (!checking.HasSymbolOnRightSide(symb))
                        continue;

                    int symbolPosition = checking.FindSymbolOnRightSide(symb);

                    for(int rPos = symbolPosition+1; rPos < checking.right.Count; rPos++)
                    {
                        if (grammar.IsTerminal(checking.right[rPos]))
                        {
                            foundFollows.Add(checking.right[rPos]);
                            break;
                        }

                        foundFollows.AddRange(prefixes_[checking.right[rPos].rep]);
                        if (prefixes_[checking.right[rPos].rep].All(s => s.rep != 'e'))
                            break;
                        
                    }
                    
                }

                removeDuplicates(ref foundFollows);

                trivialFollows_.Add(symb.rep, foundFollows);
            }
        }

        private void removeDuplicates(ref List<Symbol> symbols)
        {
            for(int i = 0; i < symbols.Count; i++)
            {
                for(int j = i + 1; j < symbols.Count; j++)
                {
                    if (symbols[i].rep == symbols[j].rep)
                    {
                        symbols.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }

        private void FindPrefixes(Grammar grammar)
        {
            for (int i = 0; i < pInfo.Count; i++)
            {
                if (grammar.IsTerminal(pInfo[i].production.right[0]))
                {
                    pInfo[i].prefixes.Add(pInfo[i].production.right[0]);
                }
                else
                {
                    pInfo[i].prefixes.AddRange(prefixes_[pInfo[i].production.right[0].rep]);
                }
            }
        }

        private void FindFollows(Grammar grammar)
        {
            for(int i = 0; i < pInfo.Count; i++)
            {
                if (pInfo[i].prefixes.All(s => s.rep != 'e'))
                    continue;

                var symbolsToCheck = new List<Symbol>();
                var CheckedSymbols = new List<Symbol>();
                symbolsToCheck.Add(pInfo[i].production.left);
                CheckedSymbols.Add(pInfo[i].production.left);

                while (symbolsToCheck.Count > 0)
                {
                    for(int j = 0; j < pInfo.Count; j++)
                    {
                        if(pInfo[j].production.LastRight().rep == symbolsToCheck[0].rep &&
                           !CheckedSymbols.Any(s => s.rep == pInfo[j].production.left.rep))
                        {
                            symbolsToCheck.Add(pInfo[j].production.left);
                            CheckedSymbols.Add(pInfo[j].production.left);
                        }
                    }
                    symbolsToCheck.RemoveAt(0);
                }

                foreach(var s in CheckedSymbols)
                {
                    pInfo[i].follows.AddRange(trivialFollows_[s.rep]);
                }
                removeDuplicates(ref pInfo[i].follows);
            }
        }

        private void CheckPrefixes()
        {
            for(int i = 0; i < pInfo.Count; i++)
            {
                for(int j = i+1; j < pInfo.Count; j++)
                {
                    if(pInfo[i].production.left == pInfo[j].production.left)
                    {
                        if (pInfo[i].prefixes.Any(pref1 => pInfo[j].prefixes.Any(pref2 => pref1.rep == pref2.rep)))
                            throw new ArgumentException("Prefixes duplication in productions: " + pInfo[i].production.ToString() + " and " + pInfo[j].production.ToString());
                    }
                }
            }
        }

        public string ProductionsInfo()
        {
            var builder = new StringBuilder();

            int columnLen1 = 13;
            int columnLen2 = 10;
            int columnLen3 = 9;

            int lineNr = 1;
            foreach (var i in pInfo)
            {
                if ((lineNr.ToString() + ". " + i.production.ToString()).Length > columnLen1)
                    columnLen1 = i.production.ToString().Length;
                if (i.prefixes.Count > columnLen2)
                    columnLen2 = i.prefixes.Count;
                if (i.follows.Count > columnLen3)
                    columnLen3 = i.follows.Count;
                lineNr++;
            }

            builder.Append("Productions: ".PadRight(columnLen1 + 4));
            builder.Append("Prefixes: ".PadRight(columnLen2 + 4));
            builder.Append("Follows: ".PadRight(columnLen3 + 4));
            builder.AppendLine();

            lineNr = 1;
            foreach (var i in pInfo)
            {
                builder.Append((lineNr.ToString() + ". " + i.production.ToString()).PadRight(columnLen1+4));
                builder.Append(symbolListToString(i.prefixes).PadRight(columnLen2 + 4));
                builder.Append(symbolListToString(i.follows).PadRight(columnLen3 + 4));
                builder.AppendLine();
                lineNr++;
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            int cellWidth = 1;

            for(int row = 0; row < grammar.nonTerminalSymbols.Count; row++)
            {
                for(int col = 0; col < grammar.terminalSymbols.Count; col++)
                {
                    var cellStr = getTableCell(grammar.nonTerminalSymbols[row], grammar.terminalSymbols[col]);
                    if (cellStr.Length > cellWidth)
                        cellWidth = cellStr.Length;
                }
            }

            cellWidth += 1;

            string rowLine = "";
            for(int i = 0; i < cellWidth * (grammar.terminalSymbols.Count + 1) + 1; i++)
            {
                rowLine += "=";
            }

            for (int row = 0; row < grammar.nonTerminalSymbols.Count + 1; row++)
            {
                builder.AppendLine(rowLine);
                for (int col = 0; col < grammar.terminalSymbols.Count + 1; col++)
                {
                    if(row == 0)
                    {
                        if (col == 0)
                            builder.Append("|".PadRight(cellWidth));
                        else
                            builder.Append(("|" + grammar.terminalSymbols[col - 1].rep).PadRight(cellWidth));
                        continue;
                    }
                    if(col == 0)
                    {
                        builder.Append(("|" + grammar.nonTerminalSymbols[row - 1].rep).PadRight(cellWidth));
                        continue;
                    }

                    var cellStr = "|" + getTableCell(grammar.nonTerminalSymbols[row - 1], grammar.terminalSymbols[col - 1]);
                    builder.Append(cellStr.PadRight(cellWidth));
                }
                builder.AppendLine("|");
            }
            builder.AppendLine(rowLine);
            return builder.ToString();
        }

        private string getTableCell(Symbol nonTerminal, Symbol terminal)
        {
            for(int i = 0; i < pInfo.Count; i++)
            {
                if(pInfo[i].production.left.rep == nonTerminal.rep &&
                   (pInfo[i].prefixes.Any(s => s.rep == terminal.rep) ||
                    pInfo[i].follows.Any(s => s.rep == terminal.rep)))
                {
                    return symbolListToString(pInfo[i].production.right) + ", " + (i+1).ToString();
                }
            }
            return "";
        }

        public Production GetProductionFor(Symbol nonTerminal, Symbol terminal, ref int prodNumber)
        {
            for (int i = 0; i < pInfo.Count; i++)
            {
                if (pInfo[i].production.left.rep == nonTerminal.rep &&
                   (pInfo[i].prefixes.Any(s => s.rep == terminal.rep) ||
                    pInfo[i].follows.Any(s => s.rep == terminal.rep)))
                {
                    prodNumber = i + 1;
                    return pInfo[i].production;
                }
            }

            if (terminal.rep != 'e')
                return GetProductionFor(nonTerminal, new Symbol('e'), ref prodNumber);

            return null;
        }

        private string symbolListToString(List<Symbol> symbols)
        {
            var builder = new StringBuilder();

            foreach (var s in symbols)
                builder.Append(s.rep);
            return builder.ToString();
        }
    }
}
