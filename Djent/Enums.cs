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
        Major = 0,
        Minor = 1,
        MelodicMinor = 2,
        HarmonicMinor = 3,
        HungarianMinor = 4,
        Phyrigian = 5
    }
}