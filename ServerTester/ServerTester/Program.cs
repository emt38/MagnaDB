using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var klk = new ServiceReference5.SalesServerClient())
            {
                //klk.CreateDominio("KLK", "F5803071");
                //klk.SetTopeComprobante("YbmsDefaultSalesDomain", ServiceReference5.TipoComprobante.ConsumidorFinal, 50000);
                //klk.SetTopeComprobante("YbmsDefaultSalesDomain", ServiceReference5.TipoComprobante.NotaCredito, 50000);
                //klk.SetTopeComprobante("YbmsDefaultSalesDomain", ServiceReference5.TipoComprobante.NotaDebito, 50000);
                //klk.SetTopeComprobante("YbmsDefaultSalesDomain", ServiceReference5.TipoComprobante.Gubernamental, 50000);
                //klk.SetTopeComprobante("YbmsDefaultSalesDomain", ServiceReference5.TipoComprobante.RegimenEspecial, 50000);
                //klk.SetTopeComprobante("YbmsDefaultSalesDomain", ServiceReference5.TipoComprobante.CreditoFiscal, 50000);
                var neo = klk.GetDomain("YbmsDefaultSalesDomain");
                while (true)
                {
                    Console.WriteLine(klk.GetNextComprobante("KLK", ServiceReference5.TipoComprobante.CreditoFiscal));
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }
}
