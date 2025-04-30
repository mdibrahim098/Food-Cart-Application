using System.Reflection.PortableExecutable;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
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
        private IProductService _productService;
        public CartAPIController( AppDbContext db, IMapper mapper,
                 IProductService productService)
        {
;
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {

                CartDto cart = new()
                {
                      CartHeader  = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {

                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += item.Count * item.Product.Price;
                }
                _response.Result = cart;
            }
            catch (Exception ex)
           
              
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();           
            }

            return _response;
        }


        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }


        //[HttpPost("RemoveCoupon")]
        //public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        //{
        //    try
        //    {
        //        var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
        //        cartFromDb.CouponCode = "";
        //        _db.CartHeaders.Update(cartFromDb);
        //        await _db.SaveChangesAsync();
        //        _response.Result = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.ToString();
        //    }
        //    return _response;
        //}


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




        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cardDetailsId)
        {
            try
            {
                CartDetails cartDetails =  _db.CartDetails
                    .First(u => u.CartDetailsId == cardDetailsId);


                int totalCountofCartItem = _db.CartDetails
                    .Where(u => u.CartHeaderId == cartDetails.CartHeaderId)
                    .Count();
                _db.CartDetails.Remove(cartDetails);    
                if (totalCountofCartItem ==1)
                {
                    var cartHeaderTpRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                  
                     _db.CartHeaders.Remove(cartHeaderTpRemove);
                }
                await _db.SaveChangesAsync();

                _response.Result = true;
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
