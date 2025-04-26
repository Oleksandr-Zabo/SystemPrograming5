using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace SystemPrograming5
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;
        private ManualResetEventSlim pauseEvent;

        public MainWindow()
        {
            InitializeComponent();
            cancellationTokenSource = new CancellationTokenSource();
            pauseEvent = new ManualResetEventSlim(true);
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SourcePath.Text = openFileDialog.FileName;
            }
        }

        private void BrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                DestinationPath.Text = saveFileDialog.FileName;
            }
        }

        private void StartCopy_Click(object sender, RoutedEventArgs e)
        {
            string sourceFile = SourcePath.Text;
            string destinationFile = DestinationPath.Text;
            if (!File.Exists(sourceFile) || string.IsNullOrEmpty(destinationFile) || !int.TryParse(ThreadCount.Text, out int threadCount))
            {
                MessageBox.Show("Invalid inputs!");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            long fileSize = new FileInfo(sourceFile).Length;
            ProgressBar.Maximum = fileSize;

            Task.Run(() => CopyFile(sourceFile, destinationFile, threadCount, cancellationTokenSource.Token));
        }

        private void PauseCopy_Click(object sender, RoutedEventArgs e)
        {
            pauseEvent.Reset(); // Призупиняємо копіювання
        }

        private void ResumeCopy_Click(object sender, RoutedEventArgs e)
        {
            pauseEvent.Set(); // Відновлюємо копіювання
        }

        private void StopCopy_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel(); // Зупиняємо копіювання
            ProgressBar.Value = 0;
        }

        private void CopyFile(string sourceFile, string destinationFile, int threadCount, CancellationToken cancellationToken)
        {
            long fileSize = new FileInfo(sourceFile).Length;
            long chunkSize = fileSize / threadCount;

            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream destStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
            {
                Parallel.For(0, threadCount, new ParallelOptions { CancellationToken = cancellationToken }, i =>
                {
                    long start = chunkSize * i;
                    long end = (i == threadCount - 1) ? fileSize : chunkSize * (i + 1);

                    sourceStream.Seek(start, SeekOrigin.Begin);
                    byte[] buffer = new byte[end - start];
                    sourceStream.Read(buffer, 0, buffer.Length);

                    pauseEvent.Wait(); // Очікуємо, якщо копіювання призупинене
                    cancellationToken.ThrowIfCancellationRequested();

                    lock (destStream)
                    {
                        destStream.Seek(start, SeekOrigin.Begin);
                        destStream.Write(buffer, 0, buffer.Length);
                        Dispatcher.Invoke(() => ProgressBar.Value += buffer.Length);
                    }
                });
            }

            Dispatcher.Invoke(() => MessageBox.Show("Copy completed or stopped!"));
        }
    }
}
