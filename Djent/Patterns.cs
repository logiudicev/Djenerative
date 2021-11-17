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
    private static int _octaveCache;
    private static Interval? _intervalCache;

    public static Pattern Rhythm(Enums.Modes mode, Note rootNote, Enums.NoteType type, bool harmony = false, int addRange = 0)
    {
        if (harmony)
        {
            CreateHarmony(mode);
        }
        else
        {
            int octave = rootNote.Octave;
            _octaveCache = addRange == 0 ? octave : Randomise.Run(octave + 1, octave + addRange);
            bool skipZero = rootNote.Octave != octave;
            _intervalCache = MainWindow.GetRandomInterval(mode, skipZero);
        }

        Note root = Note.Get(rootNote.NoteName, _octaveCache);

        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(root)
            .Note(_intervalCache, new SevenBitNumber((byte) type))
            .Build();
    }

    public static Pattern Lead(Enums.Modes mode, Note rootNote, bool harmony = false)
    {
        if (harmony)
        {
            CreateHarmony(mode);
        }
        else
        {
            int octave = rootNote.Octave;
            _octaveCache = Randomise.Run(octave + 1, octave + 3);
            _intervalCache = MainWindow.GetRandomInterval(mode);
        }

        Note root = Note.Get(rootNote.NoteName, _octaveCache);

        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(root)
            .Note(_intervalCache, new SevenBitNumber((byte) Enums.NoteType.Open))
            .Build();
    }

    public static Pattern Harmonic(Enums.Modes mode, Note rootNote, bool harmony = false)
    {
        if (harmony)
        {
            CreateHarmony(mode);
        }
        else
        {
            int octave = rootNote.Octave;
            _octaveCache = octave + 2;
            _intervalCache = MainWindow.GetRandomInterval(mode);
        }

        Note root = Note.Get(rootNote.NoteName, _octaveCache);

        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(root)
            .Note(_intervalCache, new SevenBitNumber((byte) Enums.NoteType.Harmonic))
            .Build();
    }

    public static Pattern Gap()
    {
        return new PatternBuilder()
            .StepForward(MusicalTimeSpan.Sixteenth)
            .Build();
    }

    public static void CreateHarmony(Enums.Modes mode)
    {
        int position = mode switch
        {
            Enums.Modes.Agnostic => Dictionaries.Agnostic.FirstOrDefault(x => x.Value == _intervalCache).Key,
            Enums.Modes.HarmonicMinor => Dictionaries.HarmonicMinor.FirstOrDefault(x => x.Value == _intervalCache).Key,
            Enums.Modes.Phyrigian => Dictionaries.Phyrigian.FirstOrDefault(x => x.Value == _intervalCache).Key,
            Enums.Modes.Evil => Dictionaries.Evil.FirstOrDefault(x => x.Value == _intervalCache).Key,
            _ => 0
        };

        var positionNew = (position + 2) % 7; // TODO Replace 7 with dictionary length
        _intervalCache = MainWindow.GetInterval(mode, positionNew);

        if (positionNew < position)
        {
            _octaveCache++;
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