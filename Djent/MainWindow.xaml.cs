using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public MainWindow()
        {
            InitializeComponent();

            ModesComboBox.ItemsSource = Enum.GetValues(typeof(Enums.Modes));
            RootNoteComboBox.ItemsSource = Enum.GetValues(typeof(NoteName));

            GetSettings();
        }

        public void GetSettings()
        {
            BpmDoubleUpDown.Value = Properties.Preset.Default.BPM;
            ModesComboBox.SelectedIndex = Properties.Preset.Default.Mode;
            RootNoteComboBox.SelectedIndex = Properties.Preset.Default.Root;
            OctaveIntegerUpDown.Value = Properties.Preset.Default.Octave;
            NotesIntegerUpDown.Value = Properties.Preset.Default.Notes;
            WeightRhythmMuted.Value = Properties.Preset.Default.WeightRhythmMuted;
            WeightRhythmOpen.Value = Properties.Preset.Default.WeightRhythmOpen;
            WeightLead.Value = Properties.Preset.Default.WeightLead;
            WeightGap.Value = Properties.Preset.Default.WeightGap;
            WeightHarmonic.Value = Properties.Preset.Default.WeightHarmonic;
            WeightScaleRhythm1.Value = Properties.Preset.Default.WeightRhythm1;
            WeightScaleRhythm2.Value = Properties.Preset.Default.WeightRhythm2;
            WeightScaleRhythm3.Value = Properties.Preset.Default.WeightRhythm3;
            WeightScaleRhythm4.Value = Properties.Preset.Default.WeightRhythm4;
            WeightScaleRhythm5.Value = Properties.Preset.Default.WeightRhythm5;
            WeightScaleRhythm6.Value = Properties.Preset.Default.WeightRhythm6;
            WeightScaleRhythm7.Value = Properties.Preset.Default.WeightRhythm7;
            WeightScaleLead1.Value = Properties.Preset.Default.WeightLead1;
            WeightScaleLead2.Value = Properties.Preset.Default.WeightLead2;
            WeightScaleLead3.Value = Properties.Preset.Default.WeightLead3;
            WeightScaleLead4.Value = Properties.Preset.Default.WeightLead4;
            WeightScaleLead5.Value = Properties.Preset.Default.WeightLead5;
            WeightScaleLead6.Value = Properties.Preset.Default.WeightLead6;
            WeightScaleLead7.Value = Properties.Preset.Default.WeightLead7;
        }

        public void SaveSettings()
        {
            Properties.Preset.Default.BPM = (double) BpmDoubleUpDown.Value!;
            Properties.Preset.Default.Mode = ModesComboBox.SelectedIndex;
            Properties.Preset.Default.Root = RootNoteComboBox.SelectedIndex;
            Properties.Preset.Default.Octave = (int) OctaveIntegerUpDown.Value!;
            Properties.Preset.Default.Notes = (int) NotesIntegerUpDown.Value!;
            Properties.Preset.Default.WeightRhythmMuted = (int) WeightRhythmMuted.Value!;
            Properties.Preset.Default.WeightRhythmOpen = (int) WeightRhythmOpen.Value!;
            Properties.Preset.Default.WeightLead = (int) WeightLead.Value!;
            Properties.Preset.Default.WeightGap = (int) WeightGap.Value!;
            Properties.Preset.Default.WeightHarmonic = (int) WeightHarmonic.Value!;
            Properties.Preset.Default.WeightRhythm1 = (int) WeightScaleRhythm1.Value!;
            Properties.Preset.Default.WeightRhythm2 = (int) WeightScaleRhythm2.Value!;
            Properties.Preset.Default.WeightRhythm3 = (int) WeightScaleRhythm3.Value!;
            Properties.Preset.Default.WeightRhythm4 = (int) WeightScaleRhythm4.Value!;
            Properties.Preset.Default.WeightRhythm5 = (int) WeightScaleRhythm5.Value!;
            Properties.Preset.Default.WeightRhythm6 = (int) WeightScaleRhythm6.Value!;
            Properties.Preset.Default.WeightRhythm7 = (int) WeightScaleRhythm7.Value!;
            Properties.Preset.Default.WeightLead1 = (int) WeightScaleLead1.Value!;
            Properties.Preset.Default.WeightLead2 = (int) WeightScaleLead2.Value!;
            Properties.Preset.Default.WeightLead3 = (int) WeightScaleLead3.Value!;
            Properties.Preset.Default.WeightLead4 = (int) WeightScaleLead4.Value!;
            Properties.Preset.Default.WeightLead5 = (int) WeightScaleLead5.Value!;
            Properties.Preset.Default.WeightLead6 = (int) WeightScaleLead6.Value!;
            Properties.Preset.Default.WeightLead7 = (int) WeightScaleLead7.Value!;

            Properties.Preset.Default.Save();
        }

        public static Task CreateMidiFile(Scales.Intervals scale, Note rootNote, double bpm, uint length, Probability.Articulation probArticulation, Probability.Scale probScaleRhythm, Probability.Scale probScaleLead)
        {
            string file = $"{bpm}-{rootNote.NoteName}{rootNote.Octave}-{length}-{scale.Interval1}-{scale.Interval2}-{scale.Interval3}-{scale.Interval4}-{scale.Interval5}-{scale.Interval6}-{scale.Interval7}-{DateTime.Now:yyyyMMddHHmmss}.mid";

            var midiFile = new MidiFile(new TrackChunk());
            var tempoMap = TempoMap.Create(Tempo.FromBeatsPerMinute(bpm));
            //midiFile.ReplaceTempoMap(tempoMap);
            //midiFile.Chunks.Add(new TrackChunk());
            
            List<Pattern?> guitar1 = new List<Pattern?>();
            List<Pattern?> guitar2 = new List<Pattern?>();
            List<Pattern?> bass = new List<Pattern?>();
            List<Pattern?> drums = new List<Pattern?>();

            Patterns pattern = new Patterns(scale, rootNote, probScaleRhythm, probScaleLead);

            Patterns.NoteGroup group = new Patterns.NoteGroup();

            Weighted.ChanceExecutor chanceExecutor = new Weighted.ChanceExecutor();

            if (probArticulation.RhythmMuted != 0 && probScaleRhythm.Enabled)
            {
                chanceExecutor.Add(new Weighted.ChanceParam(() =>
                {
                    group = pattern.GenerateNote(Enums.NoteRequest.RhythmMute);
                }, probArticulation.RhythmMuted));
            }
            if (probArticulation.RhythmOpen != 0 && probScaleRhythm.Enabled)
            {
                chanceExecutor.Add(new Weighted.ChanceParam(() =>
                {
                    group = pattern.GenerateNote(Enums.NoteRequest.RhythmOpen);
                }, probArticulation.RhythmOpen));
            }
            if (probArticulation.Lead != 0 && probScaleLead.Enabled)
            {
                chanceExecutor.Add(new Weighted.ChanceParam(() =>
                {
                    group = pattern.GenerateNote(Enums.NoteRequest.Lead, true);
                }, probArticulation.Lead));
            }
            if (probArticulation.Harmonic != 0 && probScaleLead.Enabled)
            {
                chanceExecutor.Add(new Weighted.ChanceParam(() =>
                {
                    group = pattern.GenerateNote(Enums.NoteRequest.Harmonic, true);
                }, probArticulation.Harmonic));
            }
            if (probArticulation.Gap != 0)
            {
                chanceExecutor.Add(new Weighted.ChanceParam(() =>
                {
                    group = pattern.GenerateNote(Enums.NoteRequest.Gap, true);
                }, probArticulation.Gap));
            }

            for (int i = 0; i < length; i++)
            {
                chanceExecutor.Execute();

                guitar1.Add(group.Guitar1);
                guitar2.Add(group.Guitar2);
                bass.Add(group.Bass);
                drums.Add(group.Drums);
            }
            
            midiFile.Chunks.Add(ChunkBuilder(tempoMap, guitar1, Enums.GmInst.OverdrivenGuitar));
            midiFile.Chunks.Add(ChunkBuilder(tempoMap, guitar2, Enums.GmInst.OverdrivenGuitar));
            midiFile.Chunks.Add(ChunkBuilder(tempoMap, bass, Enums.GmInst.ElectricBassPick));
            midiFile.Chunks.Add(ChunkBuilder(tempoMap, drums, Enums.GmInst.ElectricBassPick));

            File.Delete(@"C:\Users\anon\Desktop\test.mid");
            midiFile.Write(@"C:\Users\anon\Desktop\test.mid");

            return Task.CompletedTask;
        }

        private static TrackChunk ChunkBuilder(TempoMap tempoMap, List<Pattern?> patterns, Enums.GmInst instrument)
        {
            var patternList = patterns.CombineInSequence();
            var trackChunk = patternList.ToTrackChunk(tempoMap);

            using var timedEventsManager = trackChunk.ManageTimedEvents();
            timedEventsManager.Events.AddEvent(
                new ProgramChangeEvent((SevenBitNumber) (byte) instrument),
                time: 0);

            return trackChunk;
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var mode = (Enums.Modes) ModesComboBox.SelectedItem;
            var rootNote = (NoteName) RootNoteComboBox.SelectedItem;
            var rootOctave = (int) OctaveIntegerUpDown.Value!;
            var bpm = (double) BpmDoubleUpDown.Value!;
            var notes = (uint) NotesIntegerUpDown.Value!;

            if (bpm < 4) return;
            if (notes < 1) return;
            if (rootOctave is < 1 or > 3) return;

            Scales.Intervals scale;

            if (mode == Enums.Modes.Custom)
            {
                scale = new Scales.Intervals
                {
                    Interval1 = (int) Interval1.Value!,
                    Interval2 = (int) Interval2.Value!,
                    Interval3 = (int) Interval3.Value!,
                    Interval4 = (int) Interval4.Value!,
                    Interval5 = (int) Interval5.Value!,
                    Interval6 = (int) Interval6.Value!,
                    Interval7 = (int) Interval7.Value!
                };
            }
            else
            {
                scale = mode switch
                {
                    Enums.Modes.Major => Scales.Major(),
                    Enums.Modes.Minor => Scales.Minor(),
                    Enums.Modes.MelodicMinor => Scales.MelodicMinor(),
                    Enums.Modes.HarmonicMinor => Scales.HarmonicMinor(),
                    Enums.Modes.HungarianMinor => Scales.HungarianMinor(),
                    Enums.Modes.Phyrigian => Scales.Phyrigian(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            var probArticulation = new Probability.Articulation
            {
                RhythmMuted = (double) WeightRhythmMuted.Value!,
                RhythmOpen = (double) WeightRhythmOpen.Value!,
                Lead = (double) WeightLead.Value!,
                Gap = (double) WeightGap.Value!,
                Harmonic = (double) WeightHarmonic.Value!
            };

            var probScaleRhythm = new Probability.Scale
            {
                Degree1 = (double) WeightScaleRhythm1.Value!,
                Degree2 = (double) WeightScaleRhythm2.Value!,
                Degree3 = (double) WeightScaleRhythm3.Value!,
                Degree4 = (double) WeightScaleRhythm4.Value!,
                Degree5 = (double) WeightScaleRhythm5.Value!,
                Degree6 = (double) WeightScaleRhythm6.Value!,
                Degree7 = (double) WeightScaleRhythm7.Value!
            };

            var probScaleLead = new Probability.Scale
            {
                Degree1 = (double) WeightScaleLead1.Value!,
                Degree2 = (double) WeightScaleLead2.Value!,
                Degree3 = (double) WeightScaleLead3.Value!,
                Degree4 = (double) WeightScaleLead4.Value!,
                Degree5 = (double) WeightScaleLead5.Value!,
                Degree6 = (double) WeightScaleLead6.Value!,
                Degree7 = (double) WeightScaleLead7.Value!
            };

            await CreateMidiFile(
                scale,
                Note.Get(rootNote, rootOctave),
                bpm,
                notes,
                probArticulation,
                probScaleRhythm,
                probScaleLead);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            GetSettings();
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
