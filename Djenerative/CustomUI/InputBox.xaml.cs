using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using AdonisUI.Controls;

namespace Djenerative.CustomUI
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : AdonisWindow
    {
        public MainWindow MainWindow;

        public InputBox(MainWindow window, string title)
        {
            InitializeComponent();

            MainWindow = window;

            Title = title;
            InputText.Focus();

            Show();
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

        private void InputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ValidateInput())
                {
                    if (InputText.Text != string.Empty)
                    {
                        MainWindow.Save(InputText.Text);
                        Close();
                    }
                }
                else
                {
                    InputText.Text = "Alpha_Numeric";
                }
            }
        }

        private bool ValidateInput()
        {

            if (Regex.IsMatch(InputText.Text, @"^[a-zA-Z0-9_]+$"))
            {
                return true;
            }

            return false;
        }
    }
}
