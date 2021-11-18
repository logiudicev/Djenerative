using System;
using System.Configuration;
using System.Linq;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Note = Melanchall.DryWetMidi.MusicTheory.Note;

namespace Djent;

public class Patterns
{
    private int OctaveCache { get; set; }
    private Interval? IntervalCache { get; set; }
    private Enums.Modes Scale { get; }
    private Note RootNote { get; }
    private Probability.Scale ProbScaleRhythm { get; }
    private Probability.Scale ProbScaleLead { get; }

    public Patterns(Enums.Modes mode, Note rootNote, Probability.Scale probScaleRhythm, Probability.Scale probScaleLead)
    {
        Scale = mode;
        RootNote = rootNote;
        ProbScaleRhythm = probScaleRhythm;
        ProbScaleLead = probScaleLead;
    }

    private Interval GetRandomInterval(Probability.Scale scale, bool skipZero = false)
    {
        int seed = 0;

        Weighted.ChanceExecutor chanceExecutor = new Weighted.ChanceExecutor(
            new Weighted.ChanceParam(() =>
            {
                seed = skipZero ? Randomise.Run(1, 7) : 0;
            }, scale.Degree1),
            new Weighted.ChanceParam(() =>
            {
                seed = 1;
            }, scale.Degree2),
            new Weighted.ChanceParam(() =>
            {
                seed = 2;
            }, scale.Degree3),
            new Weighted.ChanceParam(() =>
            {
                seed = 3;
            }, scale.Degree4),
            new Weighted.ChanceParam(() =>
            {
                seed = 4;
            }, scale.Degree5),
            new Weighted.ChanceParam(() =>
            {
                seed = 5;
            }, scale.Degree6),
            new Weighted.ChanceParam(() =>
            {
                seed = 6;
            }, scale.Degree7),
            new Weighted.ChanceParam(() =>
            {
                seed = 7;
            }, scale.Degree8)
        );
        chanceExecutor.Execute();


        return GetInterval(seed);
    }

    private Interval GetInterval(int seed)
    {
        Interval interval = Scale switch
        {
            Enums.Modes.Agnostic => Dictionaries.Agnostic[seed],
            Enums.Modes.Phyrigian => Dictionaries.Phyrigian[seed],
            Enums.Modes.HarmonicMinor => Dictionaries.HarmonicMinor[seed],
            Enums.Modes.Evil => Dictionaries.Evil[seed],
            _ => Interval.Zero
        };
        return interval;
    }

    public Pattern Rhythm(Enums.NoteType type, bool harmony = false, int addRange = 0)
    {
        if (harmony)
        {
            CreateHarmony();
        }
        else
        {
            int octave = RootNote.Octave;
            OctaveCache = addRange == 0 ? octave : Randomise.Run(octave + 1, octave + addRange);
            bool skipZero = RootNote.Octave != octave;
            IntervalCache = GetRandomInterval(ProbScaleRhythm, skipZero);
        }

        Note root = Note.Get(RootNote.NoteName, OctaveCache);

        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(root)
            .Note(IntervalCache, new SevenBitNumber((byte) type))
            .Build();
    }

    public Pattern Lead(bool harmony = false)
    {
        if (harmony)
        {
            CreateHarmony();
        }
        else
        {
            int octave = RootNote.Octave;
            OctaveCache = Randomise.Run(octave + 1, octave + 3);
            IntervalCache = GetRandomInterval(ProbScaleLead);
        }

        Note root = Note.Get(RootNote.NoteName, OctaveCache);

        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(root)
            .Note(IntervalCache, new SevenBitNumber((byte) Enums.NoteType.Open))
            .Build();
    }

    public Pattern Harmonic(bool harmony = false)
    {
        if (harmony)
        {
            CreateHarmony();
        }
        else
        {
            int octave = RootNote.Octave;
            OctaveCache = octave + 2;
            IntervalCache = GetRandomInterval(ProbScaleLead);
        }

        Note root = Note.Get(RootNote.NoteName, OctaveCache);

        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(root)
            .Note(IntervalCache, new SevenBitNumber((byte) Enums.NoteType.Harmonic))
            .Build();
    }

    public Pattern Gap()
    {
        return new PatternBuilder()
            .StepForward(MusicalTimeSpan.Sixteenth)
            .Build();
    }

    private void CreateHarmony()
    {
        int position = Scale switch
        {
            Enums.Modes.Agnostic => Dictionaries.Agnostic.FirstOrDefault(x => x.Value == IntervalCache).Key,
            Enums.Modes.HarmonicMinor => Dictionaries.HarmonicMinor.FirstOrDefault(x => x.Value == IntervalCache).Key,
            Enums.Modes.Phyrigian => Dictionaries.Phyrigian.FirstOrDefault(x => x.Value == IntervalCache).Key,
            Enums.Modes.Evil => Dictionaries.Evil.FirstOrDefault(x => x.Value == IntervalCache).Key,
            _ => 0
        };

        var positionNew = (position + 2) % 7; // TODO Replace 7 with dictionary length
        IntervalCache = GetInterval(positionNew);

        if (positionNew < position)
        {
            OctaveCache++;
        }
    }

    // Insert a pause with length of 2 bars
    //.StepForward(new BarBeatTicksTimeSpan(2, 0))

    // Insert a chord defined by intervals relative to the root note specified
    // as an argument of Chord method (B2)

    /*
        .Chord(new[]
            {
                Interval.Two, // B2 + 2
                Interval.Three, // B2 + 3
                -Interval.Twelve, // B2 - 12
            },
            Octave.Get(2).B)

        // Insert a pause of single dotted triplet half length

        .StepForward(MusicalTimeSpan.Half.Triplet().SingleDotted())

        // Insert a chord defined by the specified notes

        .Chord(new[]
        {
            Melanchall.DryWetMidi.MusicTheory.Note.Get(NoteName.B, 3), // B3
            Melanchall.DryWetMidi.MusicTheory.Note.Get(NoteName.C, 4), // C4
        })
        */
}