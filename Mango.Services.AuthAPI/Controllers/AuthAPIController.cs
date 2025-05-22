using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.RabbitMQSender;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        //private readonly IMessageBus _messageBus;
        private readonly IRabbitMQAuthMessageSender _messageBus;
        protected ResponseDto _response;
        private readonly IConfiguration _configuration;
        public AuthAPIController(IAuthService authService,
            IRabbitMQAuthMessageSender messageBus,
            IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _response = new();
            _configuration = configuration;
        }


        // this for message bus azure code immplementation

        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        //{

        //    var errorMassage = await _authService.Register(model);
        //    if(!string.IsNullOrEmpty(errorMassage))
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = errorMassage;
        //        return BadRequest(_response);
        //    }
        //   await _messageBus.PublishMessage(model.Email,_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

        //    return Ok(_response);
        //}

        // this for message bus RabbitMQ code immplementation

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {

            var errorMassage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMassage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMassage;
                return BadRequest(_response);
            }
            _messageBus.SendMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

            return Ok(_response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var  loginResponse = await _authService.Login(model);   
            if(loginResponse.User== null)
            {
                _response.IsSuccess = false;
                _response.Message = "UserName or Password is incorrect";
                return BadRequest(_response);

            };

            _response.Result = loginResponse;
            return Ok(_response);
        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);

            };
            return Ok(_response);
        }


    }
}
