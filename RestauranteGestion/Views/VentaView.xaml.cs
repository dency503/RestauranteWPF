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
using RestauranteGestion.ViewModels;

namespace RestauranteGestion.Views
{
    /// <summary>
    /// Lógica de interacción para ProductoDialog.xaml
    /// </summary>
    public partial class VentaView : UserControl
    {
        public VentaView()
        {
            InitializeComponent();
            DataContext = new VentasViewModel(); // ← conecta el ViewModel
        }
    }
}
