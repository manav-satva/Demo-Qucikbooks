using System.ComponentModel.DataAnnotations;
namespace ModelDataLibrary.Models
{
    public class UserToken
    {
        [Key]
        public string RealMId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryDate { get; set; }
        public DateTime? AccessTokenExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } = null;
        public bool IsConnected { get; set; }
    }
}
