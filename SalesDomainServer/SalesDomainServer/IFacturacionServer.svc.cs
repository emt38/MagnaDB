using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SalesDomainServer
{
    public class FacturacionServer : IFacturacionServer
    {
        public static Dictionary<string, int> domains = new Dictionary<string, int>();

        public bool CreateDominio(string domainName)
        {
            if (string.IsNullOrEmpty(domainName) || domains.ContainsKey(domainName))
                return false;

            domains.Add(domainName, 0);
            return true;
        }

        public int GetNoFactura(string key)
        {
            if (!domains.ContainsKey(key))
                return -1;

            return ++domains[key];
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
