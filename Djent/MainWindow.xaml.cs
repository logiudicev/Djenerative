using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Note = Melanchall.DryWetMidi.MusicTheory.Note;

namespace Djent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Random Rand = new();

        public MainWindow()
        {
            InitializeComponent();

            CreateMidiFile(Enums.Modes.HarmonicMinor, Note.Get(NoteName.G, 2), 140, "test");
        }

        public static void CreateMidiFile(Enums.Modes mode, Note rootNote, double bpm, string fileName)
        {
            string file = $"{fileName}.mid";

            var midiFile = new MidiFile(new TrackChunk());
            var tempoMap = TempoMap.Create(Tempo.FromBeatsPerMinute(bpm));
            midiFile.ReplaceTempoMap(tempoMap);

            // Blank to allow tempo import
            midiFile.Chunks.Add(new TrackChunk());
             
            List<Pattern> patternList = new List<Pattern>();
            for (int i = 0; i < 512; i++)
            {
                Weighted.ChanceExecutor chanceExecutor = new Weighted.ChanceExecutor(
                    new Weighted.ChanceParam(() =>
                    {
                        patternList.Add(Patterns.RhythmMutedPattern(mode, rootNote));
                    }, 35),
                    new Weighted.ChanceParam(() =>
                    {
                        patternList.Add(Patterns.RhythmOpenPattern(mode, rootNote));
                    }, 35),
                    new Weighted.ChanceParam(() =>
                    {
                        patternList.Add(Patterns.LeadPattern(mode, rootNote));
                    }, 60),
                    new Weighted.ChanceParam(() =>
                    {
                        patternList.Add(Patterns.GapPattern());
                    }, 5)
                );
                chanceExecutor.Execute();
            }

            midiFile.Chunks.Add(ChunkBuilder(tempoMap, patternList));

            File.Delete(file);
            midiFile.Write(file);
        }

        public static Interval GetRandomInterval(Enums.Modes mode)
        {
            var seed = Rand.Next(0, 7);
            Interval interval = mode switch
            {
                Enums.Modes.Agnostic => Dictionaries.Agnostic[seed],
                Enums.Modes.Phyrigian => Dictionaries.Phyrigian[seed],
                Enums.Modes.HarmonicMinor => Dictionaries.HarmonicMinor[seed],
                Enums.Modes.Evil => Dictionaries.Evil[seed],
                _ => Interval.Zero
            };
            return interval;
        }

        private static TrackChunk ChunkBuilder(TempoMap tempoMap, List<Pattern> patterns)
        {
            var patternList = patterns.CombineInSequence();
            var trackChunk = patternList.ToTrackChunk(tempoMap);

            using var timedEventsManager = trackChunk.ManageTimedEvents();
            timedEventsManager.Events.AddEvent(
                new ProgramChangeEvent((SevenBitNumber)26), // 'Acoustic Guitar (steel)' in GM
                time: 0);

            return trackChunk;
        }

















        /*private static TrackChunk BuildSecondTrackChunk(TempoMap tempoMap)
        {
            // We can create a track chunk and put events in it via its constructor

            var trackChunk = new TrackChunk(
                new ProgramChangeEvent((SevenBitNumber)1)); // 'Acoustic Grand Piano' in GM

            // Insert notes via NotesManager class. See https://github.com/melanchall/drywetmidi/wiki/Notes
            // to learn more about managing notes

            using (var notesManager = trackChunk.ManageNotes())
            {
                var notes = notesManager.Notes;

                // Convert time span of 1 minute and 30 seconds to MIDI ticks. See
                // https://github.com/melanchall/drywetmidi/wiki/Time-and-length to learn more
                // about time and length representations and conversion between them

                var oneAndHalfMinute = TimeConverter.ConvertFrom(new MetricTimeSpan(0, 1, 30), tempoMap);

                // Insert two notes:
                // - A2 with length of 4/15 at 1 minute and 30 seconds from a file start
                // - B4 with length of 4 beats (1 beat = 1 quarter length at this case) at the start of a file

                notes.Add(new Note(noteName: NoteName.A,
                                   octave: 2,
                                   length: LengthConverter.ConvertFrom(new MusicalTimeSpan(4, 15),
                                                                       time: oneAndHalfMinute,
                                                                       tempoMap: tempoMap),
                                   time: oneAndHalfMinute),
                          new Note(noteName: NoteName.B,
                                   octave: 4,
                                   length: LengthConverter.ConvertFrom(new BarBeatTicksTimeSpan(0, 4),
                                                                       time: 0,
                                                                       tempoMap: tempoMap),
                                   time: 0));
            }

            // Insert chords via ChordsManager class. See https://github.com/melanchall/drywetmidi/wiki/Chords
            // to learn more about managing chords

            using (var chordsManager = trackChunk.ManageChords())
            {
                var chords = chordsManager.Chords;

                // Define notes for a chord:
                // - C2 with length of 30 seconds and 600 milliseconds
                // - C#3 with length of 300 milliseconds

                var notes = new[]
                {
            new Note(noteName: NoteName.C,
                     octave: 2,
                     length: LengthConverter.ConvertFrom(new MetricTimeSpan(0, 0, 30, 600),
                                                         time: 0,
                                                         tempoMap: tempoMap)),
            new Note(noteName: NoteName.CSharp,
                     octave: 3,
                     length: LengthConverter.ConvertFrom(new MetricTimeSpan(0, 0, 0, 300),
                                                         time: 0,
                                                         tempoMap: tempoMap))
        };

                // Insert the chord at different times:
                // - at the start of a file
                // - at 10 bars and 2 beats from a file start
                // - at 10 hours from a file start

                chords.Add(new Chord(notes,
                                     time: 0),
                           new Chord(notes,
                                     time: TimeConverter.ConvertFrom(new BarBeatTicksTimeSpan(10, 2),
                                                                     tempoMap)),
                           new Chord(notes,
                                     time: TimeConverter.ConvertFrom(new MetricTimeSpan(10, 0, 0),
                                                                     tempoMap)));
            }

            return trackChunk;
        }*/
    }
}
