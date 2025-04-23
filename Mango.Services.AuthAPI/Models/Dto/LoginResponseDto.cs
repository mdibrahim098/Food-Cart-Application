namespace Mango.Services.AuthAPI.Models.Dto
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserDto user { get; set; }

    }
}
