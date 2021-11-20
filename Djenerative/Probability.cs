namespace Djenerative
{
    public class Probability
    {
        public class Articulation
        {
            public double RhythmMuted { get; set; }
            public double RhythmOpen { get; set; }
            public double Lead { get; set; }
            public double Gap { get; set; }
            public double Harmonic { get; set; }
        }

        public class Scale
        {
            public bool Enabled =>
                Degree1 > 0 ||
                Degree2 > 0 ||
                Degree3 > 0 ||
                Degree4 > 0 ||
                Degree5 > 0 ||
                Degree6 > 0 ||
                Degree7 > 0;

            public double Degree1 { get; set; }
            public double Degree2 { get; set; }
            public double Degree3 { get; set; }
            public double Degree4 { get; set; }
            public double Degree5 { get; set; }
            public double Degree6 { get; set; }
            public double Degree7 { get; set; }
        }
    }
}
