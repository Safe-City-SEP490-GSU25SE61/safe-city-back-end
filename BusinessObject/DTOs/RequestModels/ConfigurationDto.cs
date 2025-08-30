using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class ConfigurationCreateDto
    {
        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot be longer than 50 characters.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Key is required.")]
        [StringLength(50, ErrorMessage = "Key cannot be longer than 50 characters.")]
        public string Key { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        [StringLength(200, ErrorMessage = "Value cannot be longer than 200 characters.")]
        public string? Value { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }
    }

    public class ConfigurationUpdateDto
    {
        [Required(ErrorMessage = "Id is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be greater than or equal to 0.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot be longer than 50 characters.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Key is required.")]
        [StringLength(50, ErrorMessage = "Key cannot be longer than 50 characters.")]
        public string Key { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        [StringLength(200, ErrorMessage = "Value cannot be longer than 200 characters.")]
        public string? Value { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }
    }

    public class ConfigurationResponseDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Key { get; set; }
        public string? Value { get; set; }
        public string Description { get; set; }
    }
}
