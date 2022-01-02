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
        private readonly Random _rand;

        public double RatioSum
        {
            get { return Parameters.Sum(p => p.Ratio); }
        }

        public ChanceExecutor()
        {
            Parameters = new List<ChanceParam>();
            _rand = new Random();
        }

        public void Add(ChanceParam parameters)
        {
            Parameters.Add(parameters);
        }

        public void Execute()
        {
            double numericValue = _rand.NextDouble() * RatioSum;

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

