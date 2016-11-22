using System;

namespace Bias
{
    class Bias
    {
        Random rnd;
        int max;
        int min = 0;
        double[] bias;

        /// <summary>
        /// Create a new Bias instance giving only a maximum value
        /// </summary>
        /// <param name="max">highest value in range</param>
        public Bias(double max)
        {
            rnd = new Random();
            this.max = (int)max;
            bias = new double[this.max];
            for (int i=0; i < max; i++)
            {
                bias[i] = 1.0 / max; 
            }
        }

        /// <summary>
        /// Create a new Bias instance giving both a minimum and a maximum value
        /// </summary>
        /// <param name="min">lowest value in range</param>
        /// <param name="max">highest value in range</param>
        /// <param name="step">step size</param>
        public Bias(int min, int max)
        {
            rnd = new Random();
            this.max = max;
            this.min = min;
            bias = new double[max-min];
            for (int i = 0; i < this.max - this.min; i++)
            {
                bias[i] = 1.0 / (max - min);
            }
        }
        /// <summary>
        /// Returns the next biased int
        /// </summary>
        public int Next()
        {
            /* ----------------------------------------------
             * ----Generate random Double between 0 and 1----
             * ----------------------------------------------
             */
            double val = rnd.NextDouble();
            double count = 0;


            for (int i = 0; i < max - min; i++)
            {
                /* -------------------------------------------------------------------------------------------
                 * ---increase count by bias------------------------------------------------------------------
                 * ---if the random number ends up being smaller than count, then the number will be picked---
                 * ---no comparison is needed to check if it's greater, because it would have been returned---
                 * -----in the previous iteration-------------------------------------------------------------
                 * -------------------------------------------------------------------------------------------
                 */
                count += bias[i];
                if (val < count)
                {
                    for (int x = 0; x < max - min; x++)
                    {
                        // if (x == i) bias[x] -= 0.1;
                        // else bias[x] += 0.1 / (max - min - 1);
                        if (x == i) bias[x] -= bias[i] / (max - min);
                        else bias[x] += (bias[i] / (max-min)) / (max - min - 1);
                    }
                    bias[i] += 1 - sumBias();
                    return min+i;
                }
            }
            throw new Exception("\"Random number " + Math.Round(val, 2) + " is higher than total bias number " + count + "\"");
        }

        public double[] GetBias
        {
            get { return bias; }
        }

        public double sumBias()
        {
            double ret = 0;
            foreach (double i in bias)
            {
                ret += i;
            }
            return ret;
        }
    }
}
