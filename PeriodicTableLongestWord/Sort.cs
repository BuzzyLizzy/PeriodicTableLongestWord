using System;
using System.Collections.Generic;
using System.Linq;

namespace PeriodicTableLongestWord
{
	public class Sort
	{

        Random rand;

		public Sort ()
		{
            rand = new Random();
		}

		public void QuickSort(List<string> strArr, int start, int end)
		{
            if (start < end)
            {
                int pivotIndex = QuickSortPartition(strArr, start, end);
                QuickSort(strArr, start, pivotIndex - 1);
                QuickSort(strArr, pivotIndex + 1, end);
            }

        }

        private int QuickSortPartition(List<string> strArr, int start, int end) {
			// Select random pivot
			int pivotIndex = end;
			int placeHolder = start;
            int returnPivotIndex = 0;
			string tmp;

            Console.WriteLine("QuickSortPartition: start={0} end={1}", start, end);

            for (int j = start; j < pivotIndex; j++) {
                if (strArr[j].Length <= strArr[pivotIndex].Length)
                {
                    // Swap string at j with string at placeHolder
                    tmp = strArr[placeHolder];
                    strArr[placeHolder] = strArr[j];
                    strArr[j] = tmp;
                    placeHolder++;
                }
            }

            // If all elements are smaller than the pivot point then we would have reached placeHolder up to 
            // index just before pivot. Check if pivot length larger than the placeHolder length, if it is
            // then we do not want to swap.
            if (placeHolder == pivotIndex-1)
            {
                if (strArr[placeHolder].Length > strArr[pivotIndex].Length)
                {
                    // pivot length smaller, we need to swap
                    tmp = strArr[placeHolder];
                    strArr[placeHolder] = strArr[pivotIndex];
                    strArr[pivotIndex] = tmp;
                    returnPivotIndex = placeHolder;
                }
                else
                {
                    returnPivotIndex = pivotIndex;
                }
            }
            else
            {
                // Simply swap
                tmp = strArr[placeHolder];
                strArr[placeHolder] = strArr[pivotIndex];
                strArr[pivotIndex] = tmp;
                returnPivotIndex = placeHolder;
            }

            Console.WriteLine("returnPivotIndex: {0}", returnPivotIndex);

			return returnPivotIndex;

		}

	}
}

