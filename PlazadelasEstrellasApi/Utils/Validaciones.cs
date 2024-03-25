using NSspi.Contexts;
using PlazadelasEstrellasApi.Controllers;
using PlazadelasEstrellasApi.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace PlazadelasEstrellasApi.Utils
{
    
    public class Validaciones
    {
        public Validaciones(PlazaEstrellasDBContext context) {
            _context = context;
        }



        private readonly PlazaEstrellasDBContext _context;
        public Usocfdi ValidarCfdi(Cliente pCliente)
        {
            var caracter = pCliente.Rfc.Trim().Substring(3, 1);
            byte numero;

            UsocfdisController UsoCfdi = new UsocfdisController(_context);
            Usocfdi lUsoCfdi = (UsoCfdi.GetUsocfdi(pCliente.Cfdi).Result).Value;

            if (pCliente.Rfc.Trim().Length > 12 || !byte.TryParse(caracter, out numero))
            {
                if (!lUsoCfdi.Fisica)
                    throw new Exception("El valor seleccionado en Uso de CFDI, no aplica para persona fisica.");
            }
            else
            {
                if (!lUsoCfdi.Moral)
                    throw new Exception("El valor seleccionado en Uso de CFDI, no aplica para persona moral.");
            }

            return lUsoCfdi;
        }

        public RegimenFiscal ValidarRegimenFiscal(Cliente pCliente)
        {
            var caracter = pCliente.Rfc.Trim().Substring(3, 1);
            byte numero;

            RegimenFiscalController RegimenFiscal = new RegimenFiscalController(_context);
            RegimenFiscal lRegimenFiscal = (RegimenFiscal.GetRegimenFiscal(pCliente.RegimenFiscal).Result).Value;

            if (pCliente.Rfc.Trim().Length > 12 || !byte.TryParse(caracter, out numero))
            {
                
                if (!lRegimenFiscal.Fisica)
                    throw new Exception("El valor seleccionado en Regimen Fiscal, no aplica para persona fisica.");
            }
            else
            {
                if (!lRegimenFiscal.Moral)
                    throw new Exception("El valor seleccionado en Regimen Fiscal, no aplica para persona moral.");
            }

            return lRegimenFiscal;
        }
    }
}
