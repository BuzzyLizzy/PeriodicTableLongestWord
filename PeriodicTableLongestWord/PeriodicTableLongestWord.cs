using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeriodicTableLongestWord
{
	public class PeriodicTableGetLongestWord
	{
		public List<string> DictionaryWords { set; get; }

		public PeriodicTableGetLongestWord ()
		{
		}

		public void ReadWords(string fileName) {

			// Read file into array
			using (StreamReader sr = new StreamReader(fileName)) {
				DictionaryWords = new List<string>();
                string line;

                int i = 0;
				while ((line = sr.ReadLine()) != null) {
					DictionaryWords.Add (line);
                    i++;
				}
			}
		}
	}
}

