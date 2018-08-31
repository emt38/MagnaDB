using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaleDomainsServer
{
    public class NCF
    {
        public int No { get; set; }
        public DominioFacturacion Dominio { get; set; }
        public TipoComprobante Tipo { get; set; }
        public EstadoComprobante Estado { get; set; }

        public NCF(string dominio)
        {

        }
    }
}