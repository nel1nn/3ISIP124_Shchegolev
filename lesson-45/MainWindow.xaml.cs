using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Lesson45;

public partial class MainWindow : Window
{
    private const decimal UnitPrice = 500m;
    private const decimal GiftPackagingPrice = 100m;
    private const decimal ExpressDeliveryPrice = 200m;

    private readonly bool _isReady;
    private string? _paymentMethod;

    public MainWindow()
    {
        InitializeComponent();
        _isReady = true;

        PriceInfoTextBlock.Text = $"Цена товара: {UnitPrice:N0} ₽ за штуку";
        UpdateTotal();
    }

    private int Quantity => (int)QuantitySlider.Value;

    private decimal CalculateTotal()
    {
        decimal total = UnitPrice * Quantity;

        if (GiftCheckBox.IsChecked == true)
        {
            total += GiftPackagingPrice;
        }

        if (ExpressCheckBox.IsChecked == true)
        {
            total += ExpressDeliveryPrice;
        }

        return total;
    }

    private void UpdateTotal()
    {
        QuantityTextBlock.Text = $"{Quantity} шт";
        TotalTextBlock.Text = $"Итого: {CalculateTotal():N0} ₽";
    }

    private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsAsciiDigit);
    }

    private void PhoneTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
    }

    private void PhoneTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetData(DataFormats.Text) is not string text || !text.All(char.IsAsciiDigit))
        {
            e.CancelCommand();
        }
    }

    private void PaymentRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            _paymentMethod = radioButton.Content?.ToString();
        }
    }

    private void ExtraCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (!_isReady)
        {
            return;
        }

        UpdateTotal();
    }

    private void QuantitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_isReady)
        {
            return;
        }

        UpdateTotal();
    }

    private void OrderButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameTextBox.Text.Trim();
        string phone = PhoneTextBox.Text.Trim();

        if (name.Length == 0)
        {
            ShowWarning("Введите имя.");
            NameTextBox.Focus();
            return;
        }

        if (phone.Length == 0)
        {
            ShowWarning("Введите телефон.");
            PhoneTextBox.Focus();
            return;
        }

        if (_paymentMethod == null)
        {
            ShowWarning("Выберите способ оплаты.");
            return;
        }

        List<string> extras = new List<string>();

        if (GiftCheckBox.IsChecked == true)
        {
            extras.Add("подарочная упаковка");
        }

        if (ExpressCheckBox.IsChecked == true)
        {
            extras.Add("срочная доставка");
        }

        string extrasText = extras.Count == 0 ? "нет" : string.Join(", ", extras);

        string message =
            $"Имя: {name}{Environment.NewLine}" +
            $"Телефон: {phone}{Environment.NewLine}" +
            $"Способ оплаты: {_paymentMethod}{Environment.NewLine}" +
            $"Дополнительно: {extrasText}{Environment.NewLine}" +
            $"Количество: {Quantity} шт{Environment.NewLine}" +
            $"Итого: {CalculateTotal():N0} ₽";

        MessageBox.Show(message, "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static void ShowWarning(string message)
    {
        MessageBox.Show(message, "Проверьте данные", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
