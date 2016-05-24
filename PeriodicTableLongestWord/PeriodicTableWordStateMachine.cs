using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace PeriodicTableLongestWord
{
    public class PeriodicTableWordStateMachine
    {

        public delegate States StateFunctionDelegate(char c);

        private States currentState;
        private PeriodicSymbolObject[] HashTablePeriodicSymbols;
        private StringBuilder word;

        public enum States
        {
            Start,
            OnlyOneCharSymbol,
            OneOrTwoCharSymbol,
            OnlyTwoCharSymbol,
            ValidSecondChar,
            InvalidSecondChar,
            FirstCharAfterTwoCharInvalid,
            Invalid,
            max = Invalid
        }

        StateFunctionDelegate[] stateFuncs;

        public PeriodicTableWordStateMachine()
        {
            word = new StringBuilder();
            stateFuncs = new StateFunctionDelegate[Convert.ToInt32(States.max)+1];
            currentState = States.Start;
            stateFuncs[(int)States.Start] = StateStart;
            stateFuncs[(int)States.OnlyOneCharSymbol] = StateOnlyOneCharSymbol;
            stateFuncs[(int)States.OneOrTwoCharSymbol] = StateOneOrTwoCharSymbol;
            stateFuncs[(int)States.OnlyTwoCharSymbol] = StateOnlyTwoCharSymbol;
            stateFuncs[(int)States.ValidSecondChar] = StateValidSecondChar;
            stateFuncs[(int)States.InvalidSecondChar] = StateInvalidSecondChar;
            stateFuncs[(int)States.FirstCharAfterTwoCharInvalid] = StateFirstCharAfteTwoCharInvalid;
            stateFuncs[(int)States.Invalid] = StateInvalid;

            // Build Hash tables for period symbols
            // 1. Hash table with characters that represent only a one char symbol.
            // 2. Hash table with characters that can either be a one char symbol or the start of a 2 char symbol.
            //    This hash table's elements also contains a list of all possible second characters.
            // 3. Hash table with characters that can only be the start of a 2 character symbol.
            //    This hash table's elements also contains a list of all possible second characters.
            HashTablePeriodicSymbols = new PeriodicSymbolObject[26];
            string line;
            int key = 0;

            for (int i = 0; i < 26; i++)
            {
                HashTablePeriodicSymbols[i] = null;
            }

            using (StreamReader sr = new StreamReader("D:/Work/Tests/C#/PeriodicTableLongestWord/pt-data.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {

                    line = line.ToUpper();
                    key = HashKey(line[0]);

                    if (HashTablePeriodicSymbols[key] == null)
                    {
                        HashTablePeriodicSymbols[key] = new PeriodicSymbolObject(line);
                    }
                    else
                    {
                        HashTablePeriodicSymbols[key].AddSymbol(line);
                    }
                }
            }
        }

        public int HashKey(char c)
        {
            return Convert.ToInt32(Char.ToUpper(c)) - Convert.ToInt32('A');
        }

        public States Add(char c)
        {
            word.Append(Char.ToUpper(c));
            return ExecuteCurrentState(Char.ToUpper(c));
        }

        private States ExecuteCurrentState(char c)
        {
            return stateFuncs[(int)currentState](c);
        }

        public void Reset()
        {
            currentState = States.Start;
            word.Clear();
        }

        public States StateStart(char c)
        {
            currentState = FirstCharacterCheck(c);

            return currentState;
        }

        public States StateOnlyOneCharSymbol(char c)
        {
            currentState = FirstCharacterCheck(c);

            return currentState;
        }

        public States StateOneOrTwoCharSymbol(char c)
        {
            States nextState = States.Start;
            char previousChar = word[word.Length - 2];

            // Check for the second char validity, use hash key for previous char.
            int key = HashKey(previousChar);

            if (HashTablePeriodicSymbols[key] == null)
            {
                nextState = States.Invalid;
            }
            else
            {
                if (HashTablePeriodicSymbols[key].NumberTwoCharSymbols <= 0)
                {
                    // This is only a 1 char Symbol, theoretically we should not be able to ever reach this
                    // point in this state since this is a symbol that has ate least one two char symbol 
                    // associated with this character in the hash table.
                    nextState = States.OnlyOneCharSymbol;
                }
                else
                {
                    // Check if this char is a valid second char
                    bool isValidSecondChar = CheckValidSecondChar(previousChar, c);

                    // If not valid second char this may be valid one char symbol or the start of another 2 char symbol
                    if (!isValidSecondChar)
                    {
                        nextState = FirstCharacterCheck(c);
                    }
                    else
                    { 
                        nextState = States.ValidSecondChar; 
                    }
                }
            }

            currentState = nextState;

            return nextState;
        }

        public States StateOnlyTwoCharSymbol(char c)
        {
            States nextState = States.Start;
            char previousChar = word[word.Length - 2];

            // Check for the second char validity, use hash key for previous char.
            int key = HashKey(previousChar);

            nextState = !CheckValidSecondChar(previousChar, c) ? States.Invalid : States.Start;

            currentState = nextState;

            return nextState;
        }

        public States StateValidSecondChar(char c)
        {
            States nextState;

            // This is a third char after a 2 char symbol which was also possibly a one char symbol
            // that we shall receive at this point.
            // If this char is an invalid char then it is possible that 2 chars ago is a valid one 
            // char symbol and these last 2 chars must be considered for a possible valid 2 char symbol.

            // Check if this char is a valid possible one char symbol or start of a new 2 char symbol.
            nextState = FirstCharacterCheck(c);

            if (nextState == States.Invalid)
            {
                currentState = States.FirstCharAfterTwoCharInvalid;
                // We execute the next state immediately as it is not driven by the receipt of a character, the character was already received.
                nextState = StateFirstCharAfteTwoCharInvalid(c);
            }

            currentState = nextState;

            return nextState;
        }

        public States StateInvalidSecondChar(char c)
        {
            States nextState;
            char previousChar = word[word.Length - 2];

            // Is previous character a valid one char symbol?
            nextState = FirstCharacterCheck(previousChar);

            if (nextState != States.Invalid)
            {
                if (nextState == States.OneOrTwoCharSymbol || nextState == States.OnlyOneCharSymbol)
                {
                    // Is the current character a one char symbol or start of 2 char symbol?
                    nextState = FirstCharacterCheck(c);
                }
                else
                {
                    nextState = States.Invalid;
                }
            }

            currentState = nextState;

            return nextState;
        }

        public States StateFirstCharAfteTwoCharInvalid(char c)
        {
            States nextState;
            int key = 0;
            char previousChar = word[word.Length - 2];
            char previousChar2 = word[word.Length - 3];

            // This is a third char after a 2 char symbol which was also possibly a one char symbol
            // that we shall receive at this point.
            // If this char is an invalid char then it is possible that 2 chars ago is a valid one 
            // char symbol and these last 2 chars must be considered for a possible valid 2 char symbol.

            // Is the current char minus 2 chars back a valid one char symbol?
            key = HashKey(previousChar2);

            nextState = FirstCharacterCheck(previousChar2);
            if (nextState == States.OneOrTwoCharSymbol || nextState == States.OnlyOneCharSymbol)
            {
                // Is previous character plus this character a valid 2 char symbol?
                nextState = FirstCharacterCheck(previousChar);
                if (nextState == States.OneOrTwoCharSymbol || nextState == States.OnlyTwoCharSymbol)
                {
                    nextState = CheckValidSecondChar(previousChar, c) ? States.Start : States.Invalid;
                }
                else
                {
                    nextState = States.Invalid;
                }
            }
            else
            {
                nextState = States.Invalid;
            }

            currentState = nextState;

            return nextState;
        }

        public States StateInvalid(char c)
        {
            return currentState;
        }

        private bool CheckValidSecondChar(char c1, char c2)
        {
            int key = HashKey(c1);
            // Check if this char is a valid second char
            bool isValidSecondChar = false;
            for (int i = 0; i < HashTablePeriodicSymbols[key].SecondSymbolList.Count; i++)
            {
                if (HashTablePeriodicSymbols[key].SecondSymbolList[i] == c2)
                {
                    isValidSecondChar = true;
                }
            }
            return isValidSecondChar;
        }

        private States FirstCharacterCheck(char c)
        {
            States nextState = States.Start;

            int key = HashKey(c);

            if (HashTablePeriodicSymbols[key] == null)
            {
                // No such symbol exists, Invalid
                nextState = States.Invalid;
            }
            else
            {
                // There is one or two symbols listed for this char
                if (HashTablePeriodicSymbols[key].NumberOneCharSymbols > 0 && HashTablePeriodicSymbols[key].NumberTwoCharSymbols > 0)
                {
                    // There are possible one char and two char symbols that start with this character
                    nextState = States.OneOrTwoCharSymbol;
                }
                else if (HashTablePeriodicSymbols[key].NumberOneCharSymbols > 0)
                {
                    // There is only a one char symbol
                    nextState = States.OnlyOneCharSymbol;
                }
                else if (HashTablePeriodicSymbols[key].NumberTwoCharSymbols > 0)
                {
                    // THere is only two char symbols that start with this char, no one char symbols.
                    nextState = States.OnlyTwoCharSymbol;
                }
                else
                {
                    nextState = States.Invalid;
                }
            }

            return nextState;

        }


    }

    class PeriodicSymbolObject
    {
        public PeriodicSymbolObject(string symbol)
        {
            NumberOneCharSymbols = 0;
            NumberTwoCharSymbols = 0;
            SecondSymbolList = new List<char>();
            AddSymbol(symbol);
        }
        public void AddSymbol(string symbol)
        {
            C = symbol[0];
            if (symbol.Length == 1)
            {
                NumberOneCharSymbols++;
            }
            else
            {
                NumberTwoCharSymbols++;
                SecondSymbolList.Add(symbol[1]);
            }
        }
        public int NumberOneCharSymbols { get; set; }
        public int NumberTwoCharSymbols { get; set; }
        public char C { get; set; }
        public List<char> SecondSymbolList { get; set; }

    }
}
