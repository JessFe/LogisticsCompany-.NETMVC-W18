using System;
using System.ComponentModel.DataAnnotations;

namespace Spedizioni.Models
{
    public class Spedizione
    {
        public int IDSpedizione { get; set; }
        public int FK_IDCliente { get; set; }

        [Required(ErrorMessage = "Inserire la data di spedizione")]
        public DateTime DataSpedizione { get; set; }

        [Required(ErrorMessage = "Inserire il peso del pacco")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Il peso deve essere maggiore di 0.")]
        public decimal Peso { get; set; }

        [Required(ErrorMessage = "Inserire la città di destinazione")]
        public string CittaDest { get; set; }

        [Required(ErrorMessage = "Inserire l'indirizzo di destinazione")]
        public string IndirizzoDest { get; set; }

        [Required(ErrorMessage = "Inserire il nominativo del destinatario")]
        public string NominativoDest { get; set; }

        [Range(0.0, Double.MaxValue, ErrorMessage = "Il costo non può essere negativo.")]
        public decimal? CostoSpedizione { get; set; }
        public DateTime? DataConsegna { get; set; }
    }

}