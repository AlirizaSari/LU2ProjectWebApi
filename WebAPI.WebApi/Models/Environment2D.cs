using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectLU2.WebApi.Models
{
    public class Environment2D
    {
        public const int MaxNumberOfEnvironments = 5;

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(1)]
        [MaxLength(25)]
        public string? Name { get; set; }

        [StringLength(450)]
        public string? OwnerUserId { get; set; }

        [Range(20, 200, ErrorMessage = "MaxLength moet tussen 20 en 200 liggen.")]
        public int? MaxLength { get; set; }

        [Range(20, 200, ErrorMessage = "MaxHeight moet tussen 20 en 200")]
        public int? MaxHeight { get; set; }
    }
}
