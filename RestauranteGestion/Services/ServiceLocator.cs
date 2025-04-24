using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Services
{
    public static class ServiceLocator
    {
        public static INavigationService NavigationService { get; set; }
    }

}
