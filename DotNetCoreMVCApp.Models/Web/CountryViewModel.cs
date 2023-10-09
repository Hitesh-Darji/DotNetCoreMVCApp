using System.ComponentModel.DataAnnotations;

namespace DotNetCoreMVCApp.Models.Web
{
    public class CountryViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(10)]
        public string Code { get; set; }
    }
}
