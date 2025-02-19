using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLU2.WebApi.Models
{
    public class Object2D
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid EnvironmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string PrefabId { get; set; } = string.Empty;

        [Required]
        public float PositionX { get; set; }

        [Required]
        public float PositionY { get; set; }

        [Required]
        public float ScaleX { get; set; }

        [Required]
        public float ScaleY { get; set; }

        [Required]
        public float RotationZ { get; set; }

        [Required]
        public int SortingLayer { get; set; }

    }
}
