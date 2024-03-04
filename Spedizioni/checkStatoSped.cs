using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Spedizioni
{
    public class checkStatoSped : ValidationAttribute
    {
        public string AllowState { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            System.Diagnostics.Debug.WriteLine("StatoSped: " + value);
            string[] allowedStates = AllowState.ToString().Split(',');
            if (allowedStates.Contains(value.ToString()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Scegli tra: 'In Transito', 'In Consegna', 'Consegnato', 'Non Consegnato'");
            }
        }
    }

}