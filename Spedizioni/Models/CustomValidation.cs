using System.ComponentModel.DataAnnotations;

namespace Spedizioni.Models
{
    public class CustomValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var cliente = validationContext.ObjectInstance as Cliente;
            if (cliente == null)
            {
                return new ValidationResult("Oggetto Cliente non valido");
            }

            if (cliente.TipoCliente == "Privato")
            {
                if (string.IsNullOrWhiteSpace(cliente.Nome) || string.IsNullOrWhiteSpace(cliente.Cognome) || string.IsNullOrWhiteSpace(cliente.CodiceFiscale))
                {
                    return new ValidationResult("I campi NOME, COGNOME e CODICE FISCALE sono obbligatori per i clienti privati.");
                }


            }
            else if (cliente.TipoCliente == "Azienda")
            {
                if (string.IsNullOrWhiteSpace(cliente.NomeAzienda) || string.IsNullOrWhiteSpace(cliente.PartitaIVA))
                {
                    return new ValidationResult("I campi NOME AZIENDA e P.IVA sono obbligatori per le aziende.");
                }


            }

            return ValidationResult.Success; // Se tutto è valido, ritorna successo
        }
    }
}
