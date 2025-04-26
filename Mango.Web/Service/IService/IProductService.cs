using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {


        Task<ResponseDto?> GetProductAsync(string productCode);
        Task<ResponseDto?> GetAllProductAsync();
        Task<ResponseDto?> GetProductByIdAsync(int id);
        Task<ResponseDto?> CreateProductsAsync(ProductDto productCode);
        Task<ResponseDto?> UpdateProductAsync(ProductDto productCode);
        Task<ResponseDto?> DeleteProductAsync(int id);
    }
}
