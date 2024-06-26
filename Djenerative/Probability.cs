﻿namespace Djenerative
{
    public class Probability
    {
        public class ProbabilityCollection
        {
            public Articulation Articulation { get; set; } = new();
            public Scale ScaleRhythm { get; set; } = new();
            public Scale ScaleLead { get; set; } = new();
            public Timing Timing { get; set; } = new();
            public LeadOctaves LeadOctaves { get; set; } = new();
        }

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

        public class Timing
        {
            public double SixtyFourth { get; set; }
            public double ThirtySecond { get; set; }
            public double Sixteenth { get; set; }
            public double Eighth { get; set; }
            public double Quarter { get; set; }
            public double Half { get; set; }
            public double Whole { get; set; }
        }

        public class LeadOctaves
        {
            public bool Enabled =>
                LeadOct1 > 0 ||
                LeadOct2 > 0 ||
                LeadOct3 > 0;

            public double LeadOct1 { get; set; }
            public double LeadOct2 { get; set; }
            public double LeadOct3 { get; set; }
        }
    }
}
