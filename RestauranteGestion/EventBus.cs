using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion
{
    public static class EventBus
    {
        // Evento que se disparará cuando el login sea exitoso
        public static event Action LoginSucceeded;

        // Método para invocar el evento
        public static void RaiseLoginSucceeded() => LoginSucceeded?.Invoke();
    }
}
