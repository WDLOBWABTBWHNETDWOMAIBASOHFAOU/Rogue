using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    static class MultiArray
    {
        /// <summary>
        /// Extension method to convert an entire two-dimensional array into a different type.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="mArray"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static TOutput[,] ConvertAll<TInput, TOutput>(this TInput[,] mArray, Converter<TInput, TOutput> converter)
        {
            TOutput[,] result = new TOutput[mArray.GetLength(0), mArray.GetLength(1)];
            for (int y = 0; y < mArray.GetLength(1); y++)
            {
                for (int x = 0; x < mArray.GetLength(0); x++)
                {
                    result[x, y] = converter.Invoke(mArray[x, y]);
                }
            }
            return result;
        }
    }
}
