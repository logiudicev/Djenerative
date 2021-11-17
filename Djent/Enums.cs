using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;

namespace Djent;

public static class Enums
{
    public enum NoteType
    {
        Harmonic = 127,
        Open = 100,
        Mute = 20
    }

    public enum Modes
    {
        HarmonicMinor = 0,
        Phyrigian = 1,
        Evil = 2,
        Agnostic = 3
    }
}