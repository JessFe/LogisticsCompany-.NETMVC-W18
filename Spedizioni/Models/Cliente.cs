using System.ComponentModel.DataAnnotations;

namespace Spedizioni.Models
{
    public class Cliente
    {
        public int IDCliente { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string NomeAzienda { get; set; }

        [Required(ErrorMessage = "Inserire un indirizzo")]
        public string Indirizzo { get; set; }
        public string CodiceFiscale { get; set; }
        public string PartitaIVA { get; set; }

        [Required(ErrorMessage = "Inserire Privato o Azienda")]
        [checkTipoCliente(AllowType = "Privato,Azienda", ErrorMessage = ("Scegli tra: 'Privato', 'Azienda'")]
        public string TipoCliente { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }

}