using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaleDomainsServer
{
    public enum TipoComprobante
    {
        CreditoFiscal = 1,
        ConsumidorFinal = 2,
        NotaDebito = 3,
        NotaCredito = 4,
        ProveedorInformal = 11,
        UnicoIngreso = 12,
        GastosMenores = 13,
        RegimenEspecial = 14,
        Gubernamental = 15
    }

    public enum EstadoComprobante
    {
        Utilizado,
        Liberado,
        Anulado
    }
}