using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PupilProjectPlanner
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TournamentSort<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TournamentSort<T> Sort(T[] objects, IComparer<T> comparer)
        {
            return new TournamentSort<T>(objects, objects.Length, comparer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="count"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TournamentSort<T> Sort(IEnumerable<T> objects, int count, IComparer<T> comparer)
        {
            return new TournamentSort<T>(objects, count, comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TournamentSort<T> Sort(ICollection<T> objects, IComparer<T> comparer)
        {
            return new TournamentSort<T>(objects, objects.Count, comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="count"></param>
        /// <param name="comparer"></param>
        public TournamentSort(IEnumerable<T> objects, int count, IComparer<T> comparer)
        {
            this.count = count;
            lineCount = (int)Math.Ceiling(Math.Log(count, 2)) + 1;
            this.objects = new T[int.MaxValue >> (63 - lineCount)];
            this.comparer = comparer;

            status = new bool[int.MaxValue >> (63 - lineCount)];
            int i = 0;

            foreach (var obj in objects)
            {
                this.objects[this.objects.Length - ++i] = obj;
                status[this.objects.Length - i] = true;
            }
        }

        private T[] objects;
        private bool[] status;
        private int count;
        private IComparer<T> comparer;

        private int lineCount;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetResult()
        {
            T value = Get(0, 0, out bool valid);

            while (valid)
            {
                yield return value;
                status[0] = false;
                value = Get(0, 0, out valid);
            }
        }

        private T Get(int line, int item, out bool valid)
        {
            int index = GetIndex(line,item);
            valid = false;

            if (status[index] == true)
            {
                valid = true;
            }
            else if (line + 1 < lineCount)
            {
                status[index] = true;

                T a = Get(line + 1, item * 2, out bool childValid);
                valid = childValid;

                T b = Get(line + 1, item * 2 + 1, out childValid);

                if (valid)
                {
                    if (childValid)
                    {
                        if (comparer.Compare(a, b) >= 0)
                        {
                            status[GetIndex(line + 1, item * 2)] = false;
                            return a;
                        }
                        else
                        {
                            status[GetIndex(line + 1, item * 2 + 1)] = false;
                            return b;
                        }
                    }
                    else
                    {
                        status[GetIndex(line + 1, item * 2)] = false;
                        return a;
                    }
                }
                else if (childValid)
                {
                    valid = true;
                    status[GetIndex(line + 1, item * 2 + 1)] = false;
                    return b;
                }

                status[index] = false;
            }

            return objects[index];
        }

        private int GetIndex(int line, int item)
        {
            return (int)Math.Pow(2, line) + item - 1;
        }
    }
}
