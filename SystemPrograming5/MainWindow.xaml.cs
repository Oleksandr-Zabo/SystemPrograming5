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

        private async void CalculatePower_Click(object sender, RoutedEventArgs e)
        {
            ResultBlock.Text = "Calculating...";

                try
                {
                    int.TryParse(InputNumber.Text, out int number);
                    int.TryParse(PowerNumber.Text, out int power);
                    
                    double result = await Task.Run(() => CalculatePower(number, power));
                    ResultBlock.Text = $"{number} ^ {power} = {result}";
                }
                catch (Exception ex)
                {
                    ResultBlock.Text = $"Error: {ex.Message}";
                }
        }

        private double CalculatePower(int number, int power)
        {
            double result = 1;
            bool negative_power = power < 0;
            if (power < 0)
            {
                power*= -1;
            }

            for (int i = 0; i < power; i++)
            {
                result *= number;
            }
            
            if (negative_power)
            {
                return 1 / result;
            }
            else
            {
                return result;
            }
        }
    }
}