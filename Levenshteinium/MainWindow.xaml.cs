// File: MainWindow.xaml.cs
// Created: 04.04.2017
// 
// See <summary> tags for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Levenshteinium
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            Task.Run(() =>
            {
                Levenshtein.LoadDictionary(p =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.ProgressBar.Value = p * 100;
                        this.LabelLoading.Content = $"Loading dictionary: {Math.Round(p*100, 2)} %";
                    });
                });

                this.Dispatcher.Invoke(() =>
                {
                    this.ProgressBar.Visibility = Visibility.Collapsed;
                    this.LabelLoading.Visibility = Visibility.Collapsed;
                    this.InputField.Visibility = Visibility.Visible;
                    this.HintText.Visibility = Visibility.Visible;
                    this.ListSimilar.Visibility = Visibility.Visible;
                });
            });
        }

        private void InputField_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ListSimilar.Items.Clear();

            if (!string.IsNullOrWhiteSpace(this.InputField.Text))
            {
                foreach (var similar in Levenshtein.FindSimilar(this.InputField.Text))
                {
                    this.ListSimilar.Items.Add(similar);
                }
            }
        }
    }
}