using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp
{
    public class StateContainer
    {
        public readonly Dictionary<int, object> ObjectTunnel = new();


        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
