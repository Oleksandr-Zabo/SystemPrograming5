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
        public MainWindow()
        {
            InitializeComponent();
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

            long fileSize = new FileInfo(sourceFile).Length;
            ProgressBar.Maximum = fileSize;

            Task.Run(() => CopyFile(sourceFile, destinationFile, threadCount));
        }

        private void CopyFile(string sourceFile, string destinationFile, int threadCount)
        {
            long fileSize = new FileInfo(sourceFile).Length;
            long chunkSize = fileSize / threadCount;

            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream destStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
            {
                object progressLock = new object();
                Parallel.For(0, threadCount, i =>
                {
                    long start = chunkSize * i;
                    long end = (i == threadCount - 1) ? fileSize : chunkSize * (i + 1);

                    sourceStream.Seek(start, SeekOrigin.Begin);
                    byte[] buffer = new byte[end - start];
                    sourceStream.Read(buffer, 0, buffer.Length);

                    lock (progressLock)
                    {
                        destStream.Seek(start, SeekOrigin.Begin);
                        destStream.Write(buffer, 0, buffer.Length);
                        Dispatcher.Invoke(() => ProgressBar.Value += buffer.Length);
                    }
                });
            }

            Dispatcher.Invoke(() => MessageBox.Show("Copy completed!"));
        }
    }
}
