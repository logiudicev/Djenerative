namespace Djent
{
    public class Scales
    {
        public class Intervals
        {
            public int Interval1 { get; set; }
            public int Interval2 { get; set; }
            public int Interval3 { get; set; }
            public int Interval4 { get; set; }
            public int Interval5 { get; set; }
            public int Interval6 { get; set; }
            public int Interval7 { get; set; }
        }

        public static Intervals Minor()
        {
            return new Intervals
            {
                Interval1 = 0,
                Interval2 = 2,
                Interval3 = 3,
                Interval4 = 5,
                Interval5 = 7,
                Interval6 = 8,
                Interval7 = 10
            };
        }

        public static Intervals Major()
        {
            return new Intervals
            {
                Interval1 = 0,
                Interval2 = 2,
                Interval3 = 4,
                Interval4 = 5,
                Interval5 = 7,
                Interval6 = 9,
                Interval7 = 11
            };
        }

        public static Intervals MelodicMinor()
        {
            return new Intervals
            {
                Interval1 = 0,
                Interval2 = 2,
                Interval3 = 3,
                Interval4 = 5,
                Interval5 = 7,
                Interval6 = 9,
                Interval7 = 11
            };
        }

        public static Intervals HarmonicMinor()
        {
            return new Intervals
            {
                Interval1 = 0,
                Interval2 = 2,
                Interval3 = 3,
                Interval4 = 5,
                Interval5 = 7,
                Interval6 = 8,
                Interval7 = 11
            };
        }

        public static Intervals HungarianMinor()
        {
            return new Intervals
            {
                Interval1 = 0,
                Interval2 = 2,
                Interval3 = 3,
                Interval4 = 6,
                Interval5 = 7,
                Interval6 = 8,
                Interval7 = 11
            };
        }

        public static Intervals Phyrigian()
        {
            return new Intervals
            {
                Interval1 = 0,
                Interval2 = 1,
                Interval3 = 3,
                Interval4 = 5,
                Interval5 = 7,
                Interval6 = 8,
                Interval7 = 10
            };
        }
    }
}
