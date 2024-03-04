using System;

namespace Spedizioni.Models
{
    public class AggiornamentoSpedizione
    {
        public int IDAggiornamento { get; set; }
        public int FK_IDSpedizione { get; set; }

        [checkStatoSped(AllowState = "In Transito, In Consegna, Consegnato, Non Consegnato", ErrorMessage = ("Scegli tra: 'In Transito', 'In Consegna', 'Consegnato', 'Non Consegnato'")]
        public string StatoSped { get; set; }
        public string LuogoPacco { get; set; }
        public string DescrizEvento { get; set; }
        public DateTime UltimoAggiornamento { get; set; }
    }

}