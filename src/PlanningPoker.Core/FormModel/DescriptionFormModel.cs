using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Core.FormModel
{
    public class DescriptionFormModel
    {
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
