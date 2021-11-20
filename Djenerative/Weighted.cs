using System;
using System.Collections.Generic;
using System.Linq;

namespace Djenerative;

internal class Weighted
{
    public class ChanceParam
    {
        public Action Func { get; }
        public double Ratio { get; }

        public ChanceParam(Action func, double ratio)
        {
            Func = func;
            Ratio = ratio;
        }
    }

    public class ChanceExecutor
    {
        public List<ChanceParam> Parameters { get; set; }
        private Random r;

        public double RatioSum
        {
            get { return Parameters.Sum(p => p.Ratio); }
        }

        public ChanceExecutor(List<ChanceParam> parameters)
        {
            Parameters = parameters;
            r = new Random();
        }

        public ChanceExecutor()
        {
            Parameters = new List<ChanceParam>();
            r = new Random();
        }

        public void Add(ChanceParam parameters)
        {
            /*
            Parameters = parameters;
            r = new Random();
            */

            Parameters.Add(parameters);
        }

        public void Execute()
        {
            double numericValue = r.NextDouble() * RatioSum;

            foreach (var parameter in Parameters)
            {
                numericValue -= parameter.Ratio;

                if (!(numericValue <= 0))
                    continue;

                parameter.Func();
                return;
            }
        }
    }
}

