using System;
using System.Linq;

namespace Djent;

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
        public ChanceParam[] Parameters { get; }
        private Random r;

        public double RatioSum
        {
            get { return Parameters.Sum(p => p.Ratio); }
        }

        public ChanceExecutor(params ChanceParam[] parameters)
        {
            Parameters = parameters;
            r = new Random();
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

