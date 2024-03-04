using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Spedizioni
{
    public class checkTipoCliente : ValidationAttribute
    {
        public string AllowType { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            System.Diagnostics.Debug.WriteLine("TipoCliente: " + value);
            string[] allowedTypes = AllowType.ToString().Split(',');
            if (allowedTypes.Contains(value.ToString()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Scegli tra: 'Privato', 'Azienda'");
            }
        }
    }
}