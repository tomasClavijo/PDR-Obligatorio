using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Monitores
    {
        private readonly object MonitorUsuario = new object();
        private readonly object NoLecetores = new object();
        private readonly object NoEscritores = new object();


        private int cantidadLec = 0;
        private int cantidadEsc = 0;

        public void ComenzarLeer()
        {
            lock (MonitorUsuario)
            {
                while (cantidadEsc > 0)
                {
                    Monitor.Wait(NoEscritores);
                }

                cantidadLec++;
            }
        }

        public void FinLeer()
        {
            lock (MonitorUsuario)
            {
                cantidadLec--;
                if (cantidadLec == 0)
                {
                    Monitor.PulseAll(NoLecetores);
                }
            }
        }

        public void ComenzarEscribir()
        {
            lock (MonitorUsuario)
            {
                cantidadEsc++;
                if (cantidadEsc > 1)
                {
                    Monitor.Wait(NoEscritores);
                }
                if (cantidadLec > 0)
                {
                    Monitor.Wait(NoLecetores);
                }
            }
        }



        public void FinEscribir()
        {
            lock (MonitorUsuario)
            {
                cantidadEsc--;
                Monitor.PulseAll(NoEscritores);
            }
        }
    }
}
