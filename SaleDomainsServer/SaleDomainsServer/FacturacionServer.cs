using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SaleDomainsServer
{
    public class FacturacionServer : IFacturacionServer
    {

        public static Dictionary<string, DominioFacturacion> domains = new Dictionary<string, DominioFacturacion>();

        public bool CreateDominio(string domainName, int conteoFactura = 0)
        {
            if (string.IsNullOrEmpty(domainName) || domains.ContainsKey(domainName))
                return false;

            domains.Add(domainName, new DominioFacturacion(conteoFactura));
            return true;
        }

        public int GetNoFactura(string key)
        {
            if (!domains.ContainsKey(key))
                return -1;

            return ++domains[key].ConteoFacturas;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
