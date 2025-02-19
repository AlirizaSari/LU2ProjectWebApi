using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectLU2.WebApi.Models
{
    public class Environment2D
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(25)]
        public string Name { get; set; } = string.Empty;

        [StringLength(450)]
        public string? OwnerUserId { get; set; }

        [Range(1, 10000, ErrorMessage = "MaxLength moet tussen 1 en 10.000 liggen.")]
        public int? MaxLength { get; set; }

        [Range(1, 10000, ErrorMessage = "MaxHeight moet tussen 1 en 10.000 liggen.")]
        public int? MaxHeight { get; set; }
    }
}
