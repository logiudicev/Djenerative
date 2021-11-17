using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;

namespace Djent;

public static class Dictionaries
{
    public static readonly Dictionary<int, Interval> Agnostic = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.Three },
        { 2, Interval.Five },
        { 3, Interval.Seven },
        { 4, Interval.Eight },
        { 5, Interval.Ten },
        { 6, Interval.Twelve },
        //{ 7, Interval.Two },
    };

    public static readonly Dictionary<int, Interval> Phyrigian = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.One },
        { 2, Interval.Three },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Eight },
        { 6, Interval.Ten },
        { 7, Interval.Twelve },
    };

    public static readonly Dictionary<int, Interval> HarmonicMinor = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.Two },
        { 2, Interval.Three },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Eight },
        { 6, Interval.Eleven },
        { 7, Interval.Twelve },
    };

    public static readonly Dictionary<int, Interval> Evil = new()
    {
        { 0, Interval.Zero },
        { 1, Interval.One },
        { 2, Interval.Four },
        { 3, Interval.Five },
        { 4, Interval.Seven },
        { 5, Interval.Eight },
        { 6, Interval.Eleven },
        { 7, Interval.Twelve },
    };
}