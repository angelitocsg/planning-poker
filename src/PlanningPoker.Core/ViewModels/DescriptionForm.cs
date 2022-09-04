using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Core.ViewModels
{
    public class DescriptionForm
    {
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
