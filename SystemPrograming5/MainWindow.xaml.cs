using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SystemPrograming5
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void AnalyzeText_Click(object sender, RoutedEventArgs e)
        {
            string text = InputText.Text;

            if (string.IsNullOrEmpty(text))
            {
                ResultBlock.Text = "Please enter some text!";
                return;
            }

            ResultBlock.Text = "Analyzing...";

            try
            {
                // Виконання аналізу асинхронно
                var results = await Task.Run(() => AnalyzeText(text));
                ResultBlock.Text = $"Vowels: {results.vowels}, Consonants: {results.consonants}, Symbols: {results.symbols}";
            }
            catch (Exception ex)
            {
                ResultBlock.Text = $"Error: {ex.Message}";
            }
        }

        private (int vowels, int consonants, int symbols) AnalyzeText(string text)
        {
            int vowels = 0, consonants = 0, symbols = 0;

            // Масив голосних
            char[] vowelArray = { 'a', 'e', 'i', 'o', 'u', 'y', 'а', 'е', 'є', 'и', 'і', 'о', 'у', 'ї', 'ю', 'я' };

            Thread vowelThread = new Thread(() =>
            {
                vowels = text.Count(c => vowelArray.Contains(Char.ToLower(c)));
            });

            Thread consonantThread = new Thread(() =>
            {
                consonants = text.Count(c => Char.IsLetter(c) && !vowelArray.Contains(Char.ToLower(c)));
            });

            Thread symbolThread = new Thread(() =>
            {
                symbols = text.Count(c => !Char.IsLetterOrDigit(c));
            });

            vowelThread.Start();
            consonantThread.Start();
            symbolThread.Start();

            vowelThread.Join();
            consonantThread.Join();
            symbolThread.Join();

            return (vowels, consonants, symbols);
        }
    }
}
