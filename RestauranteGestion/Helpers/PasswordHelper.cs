using System.Windows;
using System.Windows.Controls;

namespace RestauranteGestion.Helpers
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordHelper),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnPasswordPropertyChanged));

        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // Evita actualizaciones innecesarias si el valor es el mismo
                if (passwordBox.Password != (string)e.NewValue)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                    passwordBox.Password = (string)e.NewValue;
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Actualiza la propiedad adjunta solo si el valor realmente cambió
                if (GetPassword(passwordBox) != passwordBox.Password)
                {
                    SetPassword(passwordBox, passwordBox.Password);
                }
            }
        }
    }
}