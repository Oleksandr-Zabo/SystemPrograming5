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

        public MainWindow()
        {
            InitializeComponent();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = SelectFolder();
            if (!string.IsNullOrEmpty(sourcePath))
            {
                SourcePath.Text = sourcePath;
            }
        }

        private void BrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            string destinationPath = SelectFolder();
            if (!string.IsNullOrEmpty(destinationPath))
            {
                DestinationPath.Text = destinationPath;
            }
        }

        private string SelectFolder()
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false, // Allow folder selection
                Filter = "Folder Selection|*.folder",
                FileName = "Select Folder"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return Path.GetDirectoryName(dialog.FileName);
            }
            return null;
        }

        private void StartCopy_Click(object sender, RoutedEventArgs e)
        {
            string sourceDirectory = SourcePath.Text;
            string destinationDirectory = DestinationPath.Text;

            if (!Directory.Exists(sourceDirectory) || string.IsNullOrEmpty(destinationDirectory) || !int.TryParse(ThreadCount.Text, out int threadCount))
            {
                MessageBox.Show("Invalid inputs!");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            ProgressBar.Maximum = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories).Length;

            Task.Run(() => CopyDirectory(sourceDirectory, destinationDirectory, threadCount, cancellationTokenSource.Token));
        }

        private void CopyDirectory(string sourceDir, string destDir, int threadCount, CancellationToken cancellationToken)
        {
            try
            {
                string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
                string[] directories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);

                Parallel.ForEach(directories, new ParallelOptions { MaxDegreeOfParallelism = threadCount, CancellationToken = cancellationToken }, directory =>
                {
                    string relativePath = Path.GetRelativePath(sourceDir, directory);
                    string destinationPath = Path.Combine(destDir, relativePath);

                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                });

                Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = threadCount, CancellationToken = cancellationToken }, file =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string relativePath = Path.GetRelativePath(sourceDir, file);
                    string destinationPath = Path.Combine(destDir, relativePath);

                    File.Copy(file, destinationPath, true);

                    Dispatcher.Invoke(() => ProgressBar.Value += 1);
                });

                Dispatcher.Invoke(() => MessageBox.Show("Copy completed!"));
            }
            catch (OperationCanceledException)
            {
                Dispatcher.Invoke(() => MessageBox.Show("Copying canceled!"));
            }
        }
    }
}
