﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpacePlanning
{
    public class BasicUtility
    {
        //returns a random object
        public static Random RandomMaker()
        {
            return new Random();
        }
        
        //sorts input list of double and returns the indices 
        public static List<int> SortIndex(List<double> A)
        {
            var sorted = A
                        .Select((x, i) => new KeyValuePair<double, int>(x, i))
                        .OrderBy(x => x.Key)
                        .ToList();
            //get the keys, in a list
            List<double> B = sorted.Select(x => x.Key).ToList();
            //get the values in a list
            List<int> idx = sorted.Select(x => x.Value).ToList();
            //return the indices list
            return idx;
        }
        
        //random double numbers between two decimals
        internal static double RandomBetweenNumbers(Random rn, double max, double min)
        {
            double num = rn.NextDouble() * (max - min) + min;
            return num;
        }
        
        //quicksort algorithm
        internal static List<int> Quicksort(double[] a, int[] index, int left, int right)
        {
            if (right <= left) return null;
            int i = Partition(ref a, ref index, left, right);
            Quicksort(a, index, left, i - 1);
            Quicksort(a, index, i + 1, right);
            List<int> sortedIndex = new List<int>();
            for(int j = 0; j < index.Length; j++)
            {
                sortedIndex.Add(index[j]);
            }

            return sortedIndex;
        }

        //toggle input value between 0 and 1
        internal static int RandomToggleInputInt()
        {
            double num = new Random().NextDouble();
            if (num >0.5) return 1;
            else return 0;
        }

        //toggle input value between 0 and 1
        internal static int ToggleInputInt(int value = 0)
        {
            if(value == 0)
            {
                return 1;
            }else
            {
                return 0;
            }
        }

        // partition a[left] to a[right], assumes left < right for Quicksort
        internal static int Partition(ref double[] a, ref int[] index,
        int left, int right)
        {
            int i = left - 1;
            int j = right;
            while (true)
            {
                while (IsLess(a[++i], a[right]));
                while (IsLess(a[right], a[--j]))    
                    if (j == left) break;           
                if (i >= j) break;                
                Exchange(a, index, i, j);              
            }
            Exchange(a, index, i, right);             
            return i;
        }
        
        //return lesser of the two values
        internal static bool IsLess(double x, double y)
        {
            return (x < y);
        }

        // exchange two indices in an array
        private static void Exchange(double[] a, int[] index, int i, int j)
        {
            double swap = a[i];
            a[i] = a[j];
            a[j] = swap;
            int b = index[i];
            index[i] = index[j];
            index[j] = b;
        }


        //binary search algorithm
        internal static int  BinarySearch(List<int> inputArray, int key)
        {
            int min = 0;
            int max = inputArray.Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (key == inputArray[mid]) return ++mid - 1;
                else if (key < inputArray[mid]) max = mid - 1;
                else min = mid + 1;
            }
            return -1;
        }


        //binary search algo with double
        internal static int BinarySearchDouble(List<double> inputArray, double key)
        {
            int min = 0;
            int max = inputArray.Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (key == inputArray[mid]) return ++mid - 1;
                else if (key < inputArray[mid]) max = mid - 1;
                else min = mid + 1;
            }
            return -1;
        }

        //cleans duplicate indices from a list
        internal static List<double> DuplicateIndexes(List<double> exprList)
        {
            var dups = exprList.GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

            List<double> distinct = exprList.Distinct().ToList();
            for (int i = 0; i < distinct.Count; i++)
            {
                double dis = distinct[i];
                for (int j = 0; j < exprList.Count; j++)
                {
                    if (dis == exprList[j])
                    {
                        break;
                    }
                }
            }
            return dups;

        }
    }


    
}
