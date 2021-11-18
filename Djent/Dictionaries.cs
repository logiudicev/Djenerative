using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;

namespace Djent;

public static class Dictionaries
{
    public static readonly Dictionary<int, Interval> Minor = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.Two },
        { 2, Interval.Three },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Eight },
        { 6, Interval.Ten }
    };

    public static readonly Dictionary<int, Interval> Major = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.Two },
        { 2, Interval.Four },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Nine },
        { 6, Interval.Eleven }
    };

    public static readonly Dictionary<int, Interval> MelodicMinor = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.Two },
        { 2, Interval.Three },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Nine },
        { 6, Interval.Eleven }
    };

    public static readonly Dictionary<int, Interval> HarmonicMinor = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.Two },
        { 2, Interval.Three },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Eight },
        { 6, Interval.Eleven }
    };

    public static readonly Dictionary<int, Interval> HungarianMinor = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.Two },
        { 2, Interval.Three },
        { 3, Interval.Six },
        { 4, Interval.Seven },
        { 5, Interval.Eight },
        { 6, Interval.Eleven }
    };

    public static readonly Dictionary<int, Interval> Phyrigian = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.One },
        { 2, Interval.Three },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Eight },
        { 6, Interval.Ten }
    };
}