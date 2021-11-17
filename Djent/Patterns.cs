using System;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Note = Melanchall.DryWetMidi.MusicTheory.Note;

namespace Djent;

public class Patterns
{
    public static Pattern GapPattern()
    {
        return new PatternBuilder()
            .StepForward(MusicalTimeSpan.Sixteenth)
            .Build();
    }

    public static Pattern LeadPattern(Enums.Modes mode, Note rootNote)
    {
        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(rootNote)
            .Note(MainWindow.GetRandomInterval(mode))
            .Build();
    }

    public static Pattern RhythmMutedPattern(Enums.Modes mode, Note rootNote)
    {
        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(rootNote)
            .Note(MainWindow.GetRandomInterval(mode), new SevenBitNumber((byte) Enums.NoteType.Mute))

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
            .Build();
    }

    public static Pattern RhythmOpenPattern(Enums.Modes mode, Note rootNote)
    {
        return new PatternBuilder()
            .SetNoteLength(MusicalTimeSpan.Sixteenth)
            .SetRootNote(rootNote)
            .Note(MainWindow.GetRandomInterval(mode), new SevenBitNumber((byte)Enums.NoteType.Open))
            .Build();
    }
}