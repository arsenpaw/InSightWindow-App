using System.ComponentModel.DataAnnotations;

namespace AXProductApp.Models.Dto
{
    public record DeviceDto
    {
        [Required]
        public Guid Id { get;  set; }    

        [Required]
        public string DeviceType { get; set; }
    }
}
