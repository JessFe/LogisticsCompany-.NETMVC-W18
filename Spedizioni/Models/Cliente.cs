using System.ComponentModel.DataAnnotations;


namespace Spedizioni.Models
{
    public class Cliente
    {
        public int IDCliente { get; set; }


        [Required(ErrorMessage = "Inserire Privato o Azienda")]
        [checkTipoCliente(AllowType = "Privato,Azienda", ErrorMessage = ("Scegli tra: 'Privato', 'Azienda'"))]
        public string TipoCliente { get; set; }



        [StringLength(50, MinimumLength = 2, ErrorMessage = "Inserire da 2 a 50 caratteri.")]
        public string Nome { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Inserire da 2 a 50 caratteri.")]
        public string Cognome { get; set; }

        [CustomValidation]
        [RegularExpression(@"^[A-Za-z]{6}[0-9]{2}[A-Za-z][0-9]{2}[A-Za-z][0-9]{3}[A-Za-z]$", ErrorMessage = "Formato Codice Fiscale non valido.")]
        public string CodiceFiscale { get; set; }



        [StringLength(100, MinimumLength = 2, ErrorMessage = "Inserire da 2 a 100 caratteri.")]
        public string NomeAzienda { get; set; }

        [CustomValidation]
        public string PartitaIVA { get; set; }




        [Required(ErrorMessage = "Inserire un indirizzo")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Inserire da 5 a 255 caratteri.")]
        public string Indirizzo { get; set; }

        public string Email { get; set; }
        public string Telefono { get; set; }


    }

}