using System.Reflection.PortableExecutable;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;

        public CartAPIController( AppDbContext db, IMapper mapper)
        {
;
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFormDb = await _db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u =>u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFormDb == null)
                {
                    // create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();

                }
                else
                {
                    // if header is not null
                    // check if details has same product
                    var cartDetailsFormDb = await _db.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.First().ProductId
                        && u.CartHeaderId == cartHeaderFormDb.CartHeaderId);
                    if(cartDetailsFormDb == null)
                    {
                        // create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFormDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        // update count in cartdetails
                        cartDto.CartDetails.First().Count += cartDetailsFormDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFormDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFormDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }

                }

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.ToString();
                _response.IsSuccess = false;

            }

            return _response;
        }

    }
}
