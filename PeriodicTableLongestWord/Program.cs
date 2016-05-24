using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Diagnostics;

namespace PeriodicTableLongestWord
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			PeriodicTableGetLongestWord pt = new PeriodicTableGetLongestWord ();
			Sort sort = new Sort ();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine ("Reading file . . . ");

			pt.ReadWords ("../../../words.txt");

			Console.WriteLine ("Done");

			Console.WriteLine ("Sorting words from longest to shortest . . . ");
            Console.Write("\n");

            pt.DictionaryWords.Sort(CompareLengths);

            stopwatch.Stop();
            long elapsedSort = stopwatch.ElapsedMilliseconds;

            stopwatch.Start();

            PeriodicTableWordStateMachine ptMachine = new PeriodicTableWordStateMachine();
            bool wordFound = false;
            PeriodicTableWordStateMachine.States currentState;

            for (int i = pt.DictionaryWords.Count-1; i >= 0 && !wordFound; i--)
            {

                currentState = PeriodicTableWordStateMachine.States.Start;
                for (int j = 0; j < pt.DictionaryWords[i].Length && currentState != PeriodicTableWordStateMachine.States.Invalid; j++)
                {
                    currentState = ptMachine.Add(pt.DictionaryWords[i][j]);
                }

                if (currentState != PeriodicTableWordStateMachine.States.Invalid)
                {
                    wordFound = true;
                    Console.WriteLine("Word found: {0}", pt.DictionaryWords[i]);
                }

                ptMachine.Reset();
            }

            stopwatch.Stop();
            long elapsedPeriodicWords = stopwatch.ElapsedMilliseconds;

            /*
            for (int i = 0; i < 200 ; i++)
            {
                Console.Write(pt.DictionaryWords[i] + " ");
            }
            */

            Console.Write("\n");

            Console.WriteLine("Time Elapsed:");
            Console.WriteLine("\tSorting: \t\t{0}", elapsedSort);
            Console.WriteLine("\tBuild Periodic Words: \t{0}", elapsedPeriodicWords);
            Console.WriteLine("\tTotal: \t\t\t{0}", elapsedSort + elapsedPeriodicWords);

            Console.Write("\n");
            Console.WriteLine("Hit any key .....");
            Console.ReadKey();

			int x = 0;
			x++;

		}

        static int CompareLengths(string s1, string s2)
        {
            /*
            int i = s1.Length.CompareTo(s2.Length);
            int retVal = i;

            if (i < 0) retVal = 1;
            if (i > 0) retVal = -1;
            else retVal = 0;
            */

            return s1.Length.CompareTo(s2.Length); 
        }
	}
}
