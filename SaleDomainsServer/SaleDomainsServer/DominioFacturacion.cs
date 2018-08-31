using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;

namespace SaleDomainsServer
{
    [DataContract]
    public class DominioFacturacion
    {
        public DominioFacturacion(int conteoFacturas = 0)
        {
            ConteoFacturas = conteoFacturas;
        }

        public void LoadFromFile(string file)
        {

        }

        public void LoadFromFile(Stream stream)
        {
            BinaryFormatter fdf = new BinaryFormatter();
            fdf.Deserialize(stream);
        }

        public int ConteoFacturas { get; set; }

        [DataMember]
        public string ParteFijaCreditoFiscal { get; internal set; }

        [DataMember]
        public int ConteoCreditoFiscal { get; internal set; }

        [DataMember]
        public int TopeCreditoFiscal { get; internal set; }


        [DataMember]
        public string ParteFijaConsumidorFinal { get; internal set; }

        [DataMember]
        public int ConteoConsumidorFinal { get; internal set; }

        [DataMember]
        public int TopeConsumidorFinal { get; internal set; }

        [DataMember]
        public string ParteFijaNotaDeDebito { get; internal set; }

        [DataMember]
        public int ConteoNotaDeDebito { get; internal set; }

        [DataMember]
        public int TopeNotaDeDebito { get; internal set; }

        [DataMember]
        public string ParteFijaNotaDeCredito { get; internal set; }

        [DataMember]
        public int ConteoNotaDeCredito { get; internal set; }

        [DataMember]
        public int TopeNotaDeCredito { get; internal set; }

        [DataMember]
        public string ParteFijaProveedorInformal { get; internal set; }

        [DataMember]
        public int ConteoProveedorInformal { get; internal set; }

        [DataMember]
        public int TopeProveedorInformal { get; internal set; }


        [DataMember]
        public string ParteFijaUnicoIngreso { get; internal set; }

        [DataMember]
        public int ConteoUnicoIngreso { get; internal set; }

        [DataMember]
        public int TopeUnicoIngreso { get; internal set; }

        [DataMember]
        public string ParteFijaGastosMenores { get; internal set; }

        [DataMember]
        public int ConteoGastosMenores { get; internal set; }

        [DataMember]
        public int TopeGastosMenores { get; internal set; }

        [DataMember]
        public string ParteFijaRegimenEspecial { get; internal set; }

        [DataMember]
        public int ConteoRegimenEspecial { get; internal set; }

        [DataMember]
        public int TopeRegimenEspecial { get; internal set; }

        [DataMember]
        public string ParteFijaGubernamental { get; internal set; }

        [DataMember]
        public int ConteoGubernamental { get; internal set; }

        [DataMember]
        public int TopeGubernamental { get; internal set; }
    }
}