namespace Djenerative;

internal static class Enums
{
    public enum NoteVelocity
    {
        Full = 127,
        Harmonic = 127,
        Open = 100,
        Mute = 20
    }

    public enum NoteRequest
    {
        Gap = 0,
        RhythmOpen = 1,
        RhythmMute = 2,
        Lead = 3,
        Harmonic = 4
    }

    public enum Modes
    {
        Custom = 0,
        Major = 1,
        Minor = 2,
        MelodicMinor = 3,
        HarmonicMinor = 4,
        HungarianMinor = 5,
        Phyrigian = 6
    }

    public enum GmInst
    {
        AcousticGuitarNylon = 25,
        AcousticGuitarSteel = 26,
        ElectricGuitarJazz = 27,
        ElectricGuitarClean = 28,
        ElectricGuitarMuted = 29,
        OverdrivenGuitar = 30,
        DistortionGuitar = 31,
        Guitarharmonics = 32,
        AcousticBass = 33,
        ElectricBassFinger = 34,
        ElectricBassPick = 35,
        FretlessBass = 36,
        SlapBass1 = 37,
        SlapBass2 = 38
    }
}