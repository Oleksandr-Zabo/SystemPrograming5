using System;
using System.Threading;
using System.Windows;

namespace SystemProgramming5
{
    public partial class MainWindow : Window
    {
        private Thread numberThread, letterThread, symbolThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartNumbers_Click(object sender, RoutedEventArgs e)
        {
            numberThread = new Thread(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    Dispatcher.Invoke(() => LogBox.AppendText($"Number: {i}\n"));
                    Thread.Sleep(100);
                }
            });
            numberThread.Start();
        }

        private void StartLetters_Click(object sender, RoutedEventArgs e)
        {
            letterThread = new Thread(() =>
            {
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    Dispatcher.Invoke(() => LogBox.AppendText($"Letter: {c}\n"));
                    Thread.Sleep(200);
                }
            });
            letterThread.Start();
        }

        private void StartSymbols_Click(object sender, RoutedEventArgs e)
        {
            symbolThread = new Thread(() =>
            {
                string symbols = "!@#$%^&*()";
                foreach (char symbol in symbols)
                {
                    Dispatcher.Invoke(() => LogBox.AppendText($"Symbol: {symbol}\n"));
                    Thread.Sleep(300);
                }
            });
            symbolThread.Start();
        }

        private void SetHighPriority_Click(object sender, RoutedEventArgs e)
        {
            SetThreadPriority(ThreadPriority.Highest);
        }

        private void SetNormalPriority_Click(object sender, RoutedEventArgs e)
        {
            SetThreadPriority(ThreadPriority.Normal);
        }

        private void SetLowPriority_Click(object sender, RoutedEventArgs e)
        {
            SetThreadPriority(ThreadPriority.Lowest);
        }

        private void SetThreadPriority(ThreadPriority priority)
        {
            if (numberThread != null && numberThread.IsAlive)
                numberThread.Priority = priority;
            if (letterThread != null && letterThread.IsAlive)
                letterThread.Priority = priority;
            if (symbolThread != null && symbolThread.IsAlive)
                symbolThread.Priority = priority;

            LogBox.AppendText($"Priority set to {priority}\n");
        }
    }
}
