using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectLU2.WebApi.Models
{
    public class Environment2D
    {
        public const int MaxNumberOfEnvironments = 5;

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
        [MaxLength(25, ErrorMessage = "Name cannot be longer than 25 characters.")]
        public string? Name { get; set; }

        [StringLength(450)]
        public string? OwnerUserId { get; set; }

        [Range(20, 200, ErrorMessage = "MaxLength must be between 20 and 200.")]
        public int? MaxLength { get; set; }

        [Range(10, 100, ErrorMessage = "MaxHeight must be between 10 and 100.")]
        public int? MaxHeight { get; set; }
    }
}
