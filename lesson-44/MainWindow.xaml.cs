using System.Windows;

namespace Lesson44;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OrderButton_Click(object sender, RoutedEventArgs e)
    {
        string size = SmallRadioButton.IsChecked == true
            ? "Маленькая"
            : MediumRadioButton.IsChecked == true ? "Средняя" : "Большая";

        List<string> extras = new List<string>();

        if (CheeseCheckBox.IsChecked == true)
        {
            extras.Add("Сыр");
        }

        if (MushroomsCheckBox.IsChecked == true)
        {
            extras.Add("Грибы");
        }

        if (PepperoniCheckBox.IsChecked == true)
        {
            extras.Add("Пепперони");
        }

        string extrasText = extras.Count == 0 ? "без добавок" : string.Join(", ", extras);
        int quantity = (int)QuantitySlider.Value;
        string comment = CommentTextBox.Text.Trim();
        string commentText = comment.Length == 0 ? "нет" : comment;

        string message =
            $"Размер: {size}{Environment.NewLine}" +
            $"Добавки: {extrasText}{Environment.NewLine}" +
            $"Количество: {quantity} шт.{Environment.NewLine}" +
            $"Комментарий: {commentText}";

        MessageBox.Show(message, "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
