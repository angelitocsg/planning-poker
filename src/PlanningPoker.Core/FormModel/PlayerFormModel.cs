using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Core.FormModel
{
    public class PlayerFormModel
    {
        [Required]
        public string PlayerName { get; set; } = string.Empty;
    }
}
