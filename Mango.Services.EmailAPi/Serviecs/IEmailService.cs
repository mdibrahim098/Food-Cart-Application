using Mango.Services.EmailAPi.Message;
using Mango.Services.EmailAPi.Models.Dto;

namespace Mango.Services.EmailAPi.Serviecs
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}
