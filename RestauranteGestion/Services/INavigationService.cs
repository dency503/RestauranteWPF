using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace RestauranteGestion.Services
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : UserControl;
        void GoBack();
        void CloseCurrentView();
        void ShowModal<T>() where T : UserControl;
    }

    public class NavigationService : INavigationService
    {
        private readonly Stack<UserControl> _navigationStack = new();
        private readonly ContentControl _mainContentHolder;
        private readonly Dictionary<Type, UserControl> _viewCache = new();

        public NavigationService(ContentControl mainContentHolder)
        {
            _mainContentHolder = mainContentHolder;
        }

        public void NavigateTo<T>() where T : UserControl
        {
            Type viewType = typeof(T);

            if (!_viewCache.ContainsKey(viewType))
            {
                var viewInstance = Activator.CreateInstance<T>();
                _viewCache[viewType] = viewInstance;
            }

            var view = _viewCache[viewType];
            _navigationStack.Push(view);
            _mainContentHolder.Content = view;
        }
        public void CloseCurrentView()
        {
            if (_navigationStack.Count > 0)
            {
                var currentView = _navigationStack.Pop();

                // También puedes eliminarlo del caché si no lo vas a reutilizar
                _viewCache.Remove(currentView.GetType());
            }

            _mainContentHolder.Content = _navigationStack.Count > 0
                ? _navigationStack.Peek()
                : null;
        }

        public void GoBack()
        {
            if (_navigationStack.Count > 1)
            {
                _navigationStack.Pop();
                _mainContentHolder.Content = _navigationStack.Peek();
            }
        }

        public void ShowModal<T>() where T : UserControl
        {
            var dialog = Activator.CreateInstance<T>();
            DialogHost.Show(dialog, "RootDialog");
        }
    }

}
