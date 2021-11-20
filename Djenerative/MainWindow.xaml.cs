using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using AdonisUI.Controls;
using Djenerative.CustomUI;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Note = Melanchall.DryWetMidi.MusicTheory.Note;

#pragma warning disable CA2208 // Instantiate argument exceptions correctly

namespace Djenerative
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow
    {
        private readonly Presets Preset;

        public MainWindow()
        {
            InitializeComponent();

            // Sets rounded corners
            SetWindowStyle(Corner.Round);

            // Enumerate presets
            Preset = new Presets(PresetComboBox);
            GetPreset(Preset.LoadedPreset);

            // Link combo boxes to enum
            ModesComboBox.ItemsSource = Enum.GetValues(typeof(Enums.Modes));
            RootNoteComboBox.ItemsSource = Enum.GetValues(typeof(NoteName));
            
            //PresetComboBox.SelectedIndex = Preset.PresetList.FindIndex(s => s == "Default");
        }

        public void GetPreset(Presets.Preset preset)
        {
            Bpm.Value = preset.Bpm;
            ModesComboBox.SelectedIndex = preset.Mode;
            RootNoteComboBox.SelectedIndex = preset.Root;
            Octave.Value = preset.Octave;
            Notes.Value = preset.Notes;
            WeightRhythmMuted.Value = preset.WeightRhythmMuted;
            WeightRhythmOpen.Value = preset.WeightRhythmOpen;
            WeightLead.Value = preset.WeightLead;
            WeightGaps.Value = preset.WeightGap;
            WeightHarmonic.Value = preset.WeightHarmonic;
            WeightScaleRhythm1.Value = preset.WeightRhythm1;
            WeightScaleRhythm2.Value = preset.WeightRhythm2;
            WeightScaleRhythm3.Value = preset.WeightRhythm3;
            WeightScaleRhythm4.Value = preset.WeightRhythm4;
            WeightScaleRhythm5.Value = preset.WeightRhythm5;
            WeightScaleRhythm6.Value = preset.WeightRhythm6;
            WeightScaleRhythm7.Value = preset.WeightRhythm7;
            WeightScaleLead1.Value = preset.WeightLead1;
            WeightScaleLead2.Value = preset.WeightLead2;
            WeightScaleLead3.Value = preset.WeightLead3;
            WeightScaleLead4.Value = preset.WeightLead4;
            WeightScaleLead5.Value = preset.WeightLead5;
            WeightScaleLead6.Value = preset.WeightLead6;
            WeightScaleLead7.Value = preset.WeightLead7;
            Interval1.Value = preset.Interval1;
            Interval2.Value = preset.Interval2;
            Interval3.Value = preset.Interval3;
            Interval4.Value = preset.Interval4;
            Interval5.Value = preset.Interval5;
            Interval6.Value = preset.Interval6;
            Interval7.Value = preset.Interval7;
        }

        public async Task SavePreset(string name)
        {
            Presets.Preset preset = new()
            {
                Bpm = Bpm.Value,
                Mode = ModesComboBox.SelectedIndex,
                Root = RootNoteComboBox.SelectedIndex,
                Octave = Octave.Value,
                Notes = Notes.Value,
                WeightRhythmMuted = WeightRhythmMuted.Value,
                WeightRhythmOpen = WeightRhythmOpen.Value,
                WeightLead = WeightLead.Value,
                WeightGap = WeightGaps.Value,
                WeightHarmonic = WeightHarmonic.Value,
                WeightRhythm1 = WeightScaleRhythm1.Value,
                WeightRhythm2 = WeightScaleRhythm2.Value,
                WeightRhythm3 = WeightScaleRhythm3.Value,
                WeightRhythm4 = WeightScaleRhythm4.Value,
                WeightRhythm5 = WeightScaleRhythm5.Value,
                WeightRhythm6 = WeightScaleRhythm6.Value,
                WeightRhythm7 = WeightScaleRhythm7.Value,
                WeightLead1 = WeightScaleLead1.Value,
                WeightLead2 = WeightScaleLead2.Value,
                WeightLead3 = WeightScaleLead3.Value,
                WeightLead4 = WeightScaleLead4.Value,
                WeightLead5 = WeightScaleLead5.Value,
                WeightLead6 = WeightScaleLead6.Value,
                WeightLead7 = WeightScaleLead7.Value,
                Interval1 = Interval1.Value,
                Interval2 = Interval2.Value,
                Interval3 = Interval3.Value,
                Interval4 = Interval4.Value,
                Interval5 = Interval5.Value,
                Interval6 = Interval6.Value,
                Interval7 = Interval7.Value
            };

            await Preset.CreatePreset(preset, name);
        }

        public static Task CreateMidiFile(Scales.Intervals scale, Note rootNote, double bpm, uint length, Probability.Articulation probArticulation, Probability.Scale probScaleRhythm, Probability.Scale probScaleLead)
        {
            string file = $"{bpm}-{rootNote.NoteName}{rootNote.Octave}-{length}-{scale.Interval1}-{scale.Interval2}-{scale.Interval3}-{scale.Interval4}-{scale.Interval5}-{scale.Interval6}-{scale.Interval7}-{DateTime.Now:yyyyMMddHHmmss}.mid";

            var midiFile = new MidiFile(new TrackChunk());
            var tempoMap = TempoMap.Create(Tempo.FromBeatsPerMinute(bpm));
            //midiFile.ReplaceTempoMap(tempoMap);
            //midiFile.Chunks.Add(new TrackChunk());
            
            List<Pattern?> guitar1 = new();
            List<Pattern?> guitar2 = new();
            List<Pattern?> bass = new();
            List<Pattern?> drums = new();

            Patterns pattern = new(scale, rootNote, probScaleRhythm, probScaleLead);

            Patterns.NoteGroup group = new();

            Weighted.ChanceExecutor chanceExecutor = new();

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
            var rootOctave = Octave.Value;
            var bpm = (double) Bpm.Value;
            var notes = (uint) Notes.Value;

            if (bpm < 4) return;
            if (notes < 1) return;
            if (rootOctave is < 1 or > 3) return;

            Scales.Intervals scale;

            if (mode == Enums.Modes.Custom)
            {
                scale = new Scales.Intervals
                {
                    Interval1 = Interval1.Value!,
                    Interval2 = Interval2.Value!,
                    Interval3 = Interval3.Value!,
                    Interval4 = Interval4.Value!,
                    Interval5 = Interval5.Value!,
                    Interval6 = Interval6.Value!,
                    Interval7 = Interval7.Value!
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
                RhythmMuted = WeightRhythmMuted.Value,
                RhythmOpen = WeightRhythmOpen.Value,
                Lead = WeightLead.Value,
                Gap = WeightGaps.Value,
                Harmonic = WeightHarmonic.Value!
            };

            var probScaleRhythm = new Probability.Scale
            {
                Degree1 = WeightScaleRhythm1.Value,
                Degree2 = WeightScaleRhythm2.Value,
                Degree3 = WeightScaleRhythm3.Value,
                Degree4 = WeightScaleRhythm4.Value,
                Degree5 = WeightScaleRhythm5.Value,
                Degree6 = WeightScaleRhythm6.Value,
                Degree7 = WeightScaleRhythm7.Value
            };

            var probScaleLead = new Probability.Scale
            {
                Degree1 = WeightScaleLead1.Value,
                Degree2 = WeightScaleLead2.Value,
                Degree3 = WeightScaleLead3.Value,
                Degree4 = WeightScaleLead4.Value,
                Degree5 = WeightScaleLead5.Value,
                Degree6 = WeightScaleLead6.Value,
                Degree7 = WeightScaleLead7.Value
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _ = new InputBox(this, "Save Preset");
        }

        public async void Save(string name)
        {
            UpdateLastPreset(name);
            await SavePreset(name);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            GetPreset(Preset.LoadedPreset);
        }

        private void Interval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ModesComboBox.SelectedIndex = (int) Enums.Modes.Custom;
            e.Handled = true;
        }

        private void ModesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((Enums.Modes) ModesComboBox.SelectedIndex)
            {
                case Enums.Modes.Custom:
                    break;
                case Enums.Modes.Major:
                    Interval1.Value = Scales.Major().Interval1;
                    Interval2.Value = Scales.Major().Interval2;
                    Interval3.Value = Scales.Major().Interval3;
                    Interval4.Value = Scales.Major().Interval4;
                    Interval5.Value = Scales.Major().Interval5;
                    Interval6.Value = Scales.Major().Interval6;
                    Interval7.Value = Scales.Major().Interval7;
                    break;
                case Enums.Modes.Minor:
                    Interval1.Value = Scales.Minor().Interval1;
                    Interval2.Value = Scales.Minor().Interval2;
                    Interval3.Value = Scales.Minor().Interval3;
                    Interval4.Value = Scales.Minor().Interval4;
                    Interval5.Value = Scales.Minor().Interval5;
                    Interval6.Value = Scales.Minor().Interval6;
                    Interval7.Value = Scales.Minor().Interval7;
                    break;
                case Enums.Modes.MelodicMinor:
                    Interval1.Value = Scales.MelodicMinor().Interval1;
                    Interval2.Value = Scales.MelodicMinor().Interval2;
                    Interval3.Value = Scales.MelodicMinor().Interval3;
                    Interval4.Value = Scales.MelodicMinor().Interval4;
                    Interval5.Value = Scales.MelodicMinor().Interval5;
                    Interval6.Value = Scales.MelodicMinor().Interval6;
                    Interval7.Value = Scales.MelodicMinor().Interval7;
                    break;
                case Enums.Modes.HarmonicMinor:
                    Interval1.Value = Scales.HarmonicMinor().Interval1;
                    Interval2.Value = Scales.HarmonicMinor().Interval2;
                    Interval3.Value = Scales.HarmonicMinor().Interval3;
                    Interval4.Value = Scales.HarmonicMinor().Interval4;
                    Interval5.Value = Scales.HarmonicMinor().Interval5;
                    Interval6.Value = Scales.HarmonicMinor().Interval6;
                    Interval7.Value = Scales.HarmonicMinor().Interval7;
                    break;
                case Enums.Modes.HungarianMinor:
                    Interval1.Value = Scales.HungarianMinor().Interval1;
                    Interval2.Value = Scales.HungarianMinor().Interval2;
                    Interval3.Value = Scales.HungarianMinor().Interval3;
                    Interval4.Value = Scales.HungarianMinor().Interval4;
                    Interval5.Value = Scales.HungarianMinor().Interval5;
                    Interval6.Value = Scales.HungarianMinor().Interval6;
                    Interval7.Value = Scales.HungarianMinor().Interval7;
                    break;
                case Enums.Modes.Phyrigian:
                    Interval1.Value = Scales.Phyrigian().Interval1;
                    Interval2.Value = Scales.Phyrigian().Interval2;
                    Interval3.Value = Scales.Phyrigian().Interval3;
                    Interval4.Value = Scales.Phyrigian().Interval4;
                    Interval5.Value = Scales.Phyrigian().Interval5;
                    Interval6.Value = Scales.Phyrigian().Interval6;
                    Interval7.Value = Scales.Phyrigian().Interval7;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            e.Handled = true;
        }

        private void AdonisWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    DragMove();
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        private async void PresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = (string) PresetComboBox.SelectedItem;
            UpdateLastPreset(name);

            GetPreset(await Preset.LoadPreset(name));

            e.Handled = true;
        }

        #region Round Corners

        private void UpdateLastPreset(string name)
        {
            Properties.Preset.Default.LastPreset = name;
            Properties.Preset.Default.Save();
        }

        private void SetWindowStyle(Corner preference)
        {
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
            var attribute = DwmWindowAttribute.DwnCornerPreference;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern long DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute attribute, ref Corner pvAttribute, uint cbAttribute);

        public enum DwmWindowAttribute { DwnCornerPreference = 33 }

        public enum Corner
        {
            Default = 0,
            NoRound = 1,
            Round = 2,
            RoundSmall = 3
        }

        #endregion














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
