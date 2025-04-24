using RestauranteGestion.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RestauranteGestion.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        public ContentControl MainContentControl => MainContent;

    }
}