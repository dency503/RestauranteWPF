using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RestauranteGestion.Converters
{
    public class EstadoMesaToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string estado)
            {
                return estado switch
                {
                    "Disponible" => new SolidColorBrush(Colors.LightGreen),
                    "Ocupada" => new SolidColorBrush(Colors.IndianRed),
                    "Reservada" => new SolidColorBrush(Colors.Goldenrod),
                    _ => new SolidColorBrush(Colors.LightGray)
                };
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
