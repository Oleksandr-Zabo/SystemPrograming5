using System;
using System.Numerics;
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

        private async void CalculateFactorial_Click(object sender, RoutedEventArgs e)
        {
            ResultBlock.Text = "Calculating...";

            if (int.TryParse(InputNumber.Text, out int number) && number >= 0)
            {
                try
                {
                    BigInteger result = await Task.Run(() => CalculateFactorial(number));
                    ResultBlock.Text = $"Factorial of {number} is {result}";
                }
                catch (Exception ex)
                {
                    ResultBlock.Text = $"Error: {ex.Message}";
                }
            }
            else
            {
                ResultBlock.Text = "Please enter a valid non-negative integer.";
            }
        }

        private BigInteger CalculateFactorial(int number)
        {
            BigInteger factorial = 1;
            for (int i = 1; i <= number; i++)
            {
                factorial *= i;
            }
            return factorial;
        }
    }
}