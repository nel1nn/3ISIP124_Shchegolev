using System.Globalization;
using System.Windows;

namespace Lesson42;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        OperationComboBox.ItemsSource = Calculator.Operations;
        OperationComboBox.SelectedIndex = 0;
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        ResultTextBox.Clear();

        if (!TryParseNumber(FirstNumberTextBox.Text, out decimal first))
        {
            ShowError("Первое число введено неверно.");
            FirstNumberTextBox.Focus();
            return;
        }

        if (!TryParseNumber(SecondNumberTextBox.Text, out decimal second))
        {
            ShowError("Второе число введено неверно.");
            SecondNumberTextBox.Focus();
            return;
        }

        if (OperationComboBox.SelectedItem is not string operation)
        {
            ShowError("Выберите математическое действие.");
            return;
        }

        try
        {
            decimal result = Calculator.Calculate(first, second, operation);
            ResultTextBox.Text = result.ToString(CultureInfo.CurrentCulture);
        }
        catch (DivideByZeroException)
        {
            ShowError("На ноль делить нельзя.");
            SecondNumberTextBox.Focus();
        }
        catch (OverflowException)
        {
            ShowError("Результат слишком большой.");
        }
        catch (ArgumentException ex)
        {
            ShowError(ex.Message);
        }
    }

    private static bool TryParseNumber(string text, out decimal number)
    {
        string normalized = text.Trim().Replace(',', '.');

        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out number);
    }

    private static void ShowError(string message)
    {
        MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
