using Warehouse.Application.Common;
using Warehouse.Application.DTO.Product;

namespace Warehouse.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);

    Task<OperationResult<ProductResponseDto>> AddAsync(ProductDto dto);
    Task<OperationResult<ProductResponseDto>> UpdateAsync(int id, ProductDto dto);

    Task<OperationResult> DeleteAsync(int id);
}