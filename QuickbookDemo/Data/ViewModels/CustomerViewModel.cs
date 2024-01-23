using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDataLibrary.ViewModels
{
    public class CustomerViewModel
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string? Name { get; set; }
        [MaxLength(100)]
        public string? CompanyName { get; set; }
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string? Email { get; set; }
        [MaxLength(30)]
        public string? Phone { get; set; }
        //public decimal Balance { get; set; }= decimal.Zero;
        public string RealMId { get; set; } = string.Empty;
        //public bool Active { get; set; }
        [RegularExpression(@"^(https?|ftp):\/\/(www\.[a-zA-Z0-9-]+(\.[a-zA-Z]{2,}){1,})+([/?].*)?$")]
        public string? Website { get; set; } = string.Empty;
        public string? SyncToken { get; set; }
    }
}
