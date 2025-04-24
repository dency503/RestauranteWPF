using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace RestauranteGestion.Views
{
    /// <summary>
    /// Lógica de interacción para ConfirmacionDialog.xaml
    /// </summary>
    public partial class ConfirmacionDialog : UserControl
    {
        public ConfirmacionDialog()
        {
            InitializeComponent();
        }
        // Método para confirmar la acción
        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(true, null); // Retorna 'true' si se confirma
        }

        // Método para cancelar la acción
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(false, null); // Retorna 'false' si se cancela
        }
    }
}
