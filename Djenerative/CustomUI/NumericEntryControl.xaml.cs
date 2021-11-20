using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Djent
{
    /// <summary>
    /// Interaction logic for NumericEntryControl.xaml
    /// </summary>
    public partial class NumericEntryControl : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label",
            typeof(string), typeof(NumericEntryControl),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(int), typeof(NumericEntryControl),
            new PropertyMetadata(0));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum",
            typeof(int), typeof(NumericEntryControl),
            new PropertyMetadata(100));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum",
                    typeof(int), typeof(NumericEntryControl),
                    new PropertyMetadata(0));

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment",
            typeof(int), typeof(NumericEntryControl),
            new PropertyMetadata(1));

        public static readonly DependencyProperty LargeIncrementProperty = DependencyProperty.Register("LargeIncrement",
            typeof(int), typeof(NumericEntryControl),
            new PropertyMetadata(5));

        private int _previousValue;
        private readonly DispatcherTimer _timer = new();
        private static readonly int _delayRate = SystemParameters.KeyboardDelay;
        private static readonly int _repeatSpeed = Math.Max(1, SystemParameters.KeyboardSpeed);

        private bool _isIncrementing;

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public int Minimum
        {
            get => (int)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public int Increment
        {
            get => (int)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public int LargeIncrement
        {
            get => (int)GetValue(LargeIncrementProperty);
            set => SetValue(LargeIncrementProperty, value);
        }

        public NumericEntryControl()
        {
            InitializeComponent();

            _textbox.PreviewTextInput += Textbox_PreviewTextInput;
            _textbox.PreviewKeyDown += Textbox_PreviewKeyDown;
            _textbox.GotFocus += Textbox_GotFocus;
            _textbox.LostFocus += Textbox_LostFocus;

            buttonIncrement.PreviewMouseLeftButtonDown += ButtonIncrement_PreviewMouseLeftButtonDown;
            buttonIncrement.PreviewMouseLeftButtonUp += ButtonIncrement_PreviewMouseLeftButtonUp;

            buttonDecrement.PreviewMouseLeftButtonDown += ButtonDecrement_PreviewMouseLeftButtonDown;
            buttonDecrement.PreviewMouseLeftButtonUp += ButtonDecrement_PreviewMouseLeftButtonUp;

            _timer.Tick += TimerTick;
        }

        private void ButtonIncrement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            buttonIncrement.CaptureMouse();
            _timer.Interval = TimeSpan.FromMilliseconds(_delayRate * 250);
            _timer.Start();

            _isIncrementing = true;
        }

        private void ButtonIncrement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            buttonIncrement.ReleaseMouseCapture();
            IncrementValue();
        }

        private void ButtonDecrement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            buttonDecrement.CaptureMouse();
            _timer.Interval = TimeSpan.FromMilliseconds(_delayRate * 250);
            _timer.Start();

            _isIncrementing = false;
        }

        private void ButtonDecrement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            buttonDecrement.ReleaseMouseCapture();
            DecrementValue();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            if (_isIncrementing)
            {
                IncrementValue();
            }
            else
            {
                DecrementValue();
            }
            _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / _repeatSpeed);

        }

        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            _previousValue = Value;
        }

        private void Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(_textbox.Text, out int newValue))
            {
                if (newValue > Maximum)
                {
                    newValue = Maximum;
                }
                else if (newValue < Minimum)
                {
                    newValue = Minimum;
                }
            }
            else
            {
                newValue = _previousValue;
            }
            _textbox.Text = newValue.ToString();
        }

        private void Textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true;
            }

            ButtonStatus();
        }

        private static bool IsNumericInput(string text)
        {
            return text.All(char.IsDigit);
        }

        private void Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    IncrementValue();
                    break;
                case Key.Down:
                    DecrementValue();
                    break;
                case Key.Left:
                    DecrementValue();
                    break;
                case Key.Right:
                    IncrementValue();
                    break;
                case Key.PageUp:
                    Value = Math.Min(Value + LargeIncrement, Maximum);
                    break;
                case Key.PageDown:
                    Value = Math.Max(Value - LargeIncrement, Minimum);
                    break;
                default:
                    //do nothing
                    break;
            }
        }

        private void IncrementValue()
        {
            Value = Math.Min(Value + Increment, Maximum);
            ButtonStatus();
        }

        private void DecrementValue()
        {
            Value = Math.Max(Value - Increment, Minimum);
            ButtonStatus();
        }

        private void ButtonStatus()
        {
            buttonIncrement.IsEnabled = Value != Maximum;
            buttonDecrement.IsEnabled = Value != Minimum;

            if (Label == string.Empty)
            {
                _label.Visibility = Visibility.Collapsed;
            }
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Value > Maximum || Value < Minimum)
            {
                Value = _previousValue;
            }

            ButtonStatus();
        }
    }
}
