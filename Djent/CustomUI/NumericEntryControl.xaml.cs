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
        private DispatcherTimer _timer = new();
        private static int _delayRate = SystemParameters.KeyboardDelay;
        private static int _repeatSpeed = Math.Max(1, SystemParameters.KeyboardSpeed);

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

            _textbox.PreviewTextInput += _textbox_PreviewTextInput;
            _textbox.PreviewKeyDown += _textbox_PreviewKeyDown;
            _textbox.GotFocus += _textbox_GotFocus;
            _textbox.LostFocus += _textbox_LostFocus;

            buttonIncrement.PreviewMouseLeftButtonDown += buttonIncrement_PreviewMouseLeftButtonDown;
            buttonIncrement.PreviewMouseLeftButtonUp += buttonIncrement_PreviewMouseLeftButtonUp;

            buttonDecrement.PreviewMouseLeftButtonDown += buttonDecrement_PreviewMouseLeftButtonDown;
            buttonDecrement.PreviewMouseLeftButtonUp += buttonDecrement_PreviewMouseLeftButtonUp;

            _timer.Tick += _timer_Tick;
        }

        void buttonIncrement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            buttonIncrement.CaptureMouse();
            _timer.Interval = TimeSpan.FromMilliseconds(_delayRate * 250);
            _timer.Start();

            _isIncrementing = true;
        }

        void buttonIncrement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            buttonIncrement.ReleaseMouseCapture();
            IncrementValue();
        }

        void buttonDecrement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            buttonDecrement.CaptureMouse();
            _timer.Interval = TimeSpan.FromMilliseconds(_delayRate * 250);
            _timer.Start();

            _isIncrementing = false;
        }

        void buttonDecrement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            buttonDecrement.ReleaseMouseCapture();
            DecrementValue();
        }

        void _timer_Tick(object? sender, EventArgs e)
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

        void _textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            _previousValue = Value;
        }

        void _textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            int newValue = 0;
            if (int.TryParse(_textbox.Text, out newValue))
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

        void _textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true;
            }

            ButtonStatus();
        }

        private bool IsNumericInput(string text)
        {
            return text.All(char.IsDigit);
        }

        void _textbox_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void _textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Value > Maximum || Value < Minimum)
            {
                Value = _previousValue;
            }

            ButtonStatus();
        }
    }
}
