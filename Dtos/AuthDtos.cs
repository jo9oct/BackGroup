using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }

    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
      public class UserLoginResponseDto
{
public string Token { get; set; } = string.Empty;
public DateTime Expiration { get; set; }
public string Username { get; set; } = string.Empty;
}
}