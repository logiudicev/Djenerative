using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Djenerative
{
    public class Presets
    {
        public Preset LoadedPreset { get; set; } = new();
        private const string PresetDirectoryName = "Presets";
        private const string PresetDefault = "Default";
        private readonly string _presetDirectoryFullPath = Path.Combine(Environment.CurrentDirectory, PresetDirectoryName);
        public List<string> PresetList = new();
        private ComboBox ComboBox { get; }

        public Presets(ComboBox comboBox)
        {
            ComboBox = comboBox;
            _ = CheckPresets();
        }

        public async Task CheckPresets()
        {
            Directory.CreateDirectory(PresetDirectoryName);
            await CreatePreset(new Preset(), PresetDefault);

            var filesPaths = Directory.GetFiles(_presetDirectoryFullPath);
            PresetList.Add(PresetDefault);
            foreach (var file in filesPaths)
            {
                if (file.EndsWith(".djp") && !file.Contains(PresetDefault))
                {
                    PresetList.Add(Path.GetFileNameWithoutExtension(file));
                }
            }

            ComboBox.ItemsSource = PresetList;
        }

        public async Task CreatePreset(Preset preset, string name)
        {
            string fileName = Path.Combine(_presetDirectoryFullPath, $"{name}.djp");
            await using FileStream createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, preset);
            await createStream.DisposeAsync();
        }

        public class Preset
        {
            public int Bpm { get; set; } = 120;
            public int Mode { get; set; } = 4;
            public int Root { get; set; } = 7;
            public int Octave { get; set; } = 2;
            public int Notes { get; set; } = 8192;
            public int WeightRhythmMuted { get; set; } = 35;
            public int WeightRhythmOpen { get; set; } = 25;
            public int WeightLead { get; set; } = 60;
            public int WeightGap { get; set; } = 5;
            public int WeightHarmonic { get; set; } = 0;
            public int WeightRhythm1 { get; set; } = 1;
            public int WeightRhythm2 { get; set; } = 1;
            public int WeightRhythm3 { get; set; } = 1;
            public int WeightRhythm4 { get; set; } = 1;
            public int WeightRhythm5 { get; set; } = 1;
            public int WeightRhythm6 { get; set; } = 1;
            public int WeightRhythm7 { get; set; } = 1;
            public int WeightLead1 { get; set; } = 1;
            public int WeightLead2 { get; set; } = 1;
            public int WeightLead3 { get; set; } = 1;
            public int WeightLead4 { get; set; } = 1;
            public int WeightLead5 { get; set; } = 1;
            public int WeightLead6 { get; set; } = 1;
            public int WeightLead7 { get; set; } = 1;
            public int Interval1 { get; set; } = 0;
            public int Interval2 { get; set; } = 2;
            public int Interval3 { get; set; } = 3;
            public int Interval4 { get; set; } = 5;
            public int Interval5 { get; set; } = 7;
            public int Interval6 { get; set; } = 8;
            public int Interval7 { get; set; } = 10;
        }
    }
}
