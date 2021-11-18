using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;

namespace Djent;

public static class Enums
{
    public enum NoteType
    {
        Bass = 127,
        Harmonic = 127,
        Open = 100,
        Mute = 20
    }

    public enum Modes
    {
        Custom = 0,
        Major = 1,
        Minor = 2,
        MelodicMinor = 3,
        HarmonicMinor = 4,
        Phyrigian = 5
        //HungarianMinor = 5,
    }
}