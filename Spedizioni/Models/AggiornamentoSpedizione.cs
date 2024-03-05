using System;
using System.ComponentModel.DataAnnotations;

namespace Spedizioni.Models
{
    public class AggiornamentoSpedizione
    {
        public int IDAggiornamento { get; set; }
        public int FK_IDSpedizione { get; set; }

        [checkStatoSped(AllowState = "In Transito,In Consegna,Consegnato,Non Consegnato", ErrorMessage = ("Scegli tra: 'In Transito', 'In Consegna', 'Consegnato', 'Non Consegnato'"))]
        public string StatoSped { get; set; }

        [StringLength(255, ErrorMessage = "Inserire massimo 255 caratteri.")]
        public string LuogoPacco { get; set; }

        [StringLength(255, ErrorMessage = "Inserire massimo 255 caratteri.")]
        public string DescrizEvento { get; set; }
        public DateTime UltimoAggiornamento { get; set; }
    }

}