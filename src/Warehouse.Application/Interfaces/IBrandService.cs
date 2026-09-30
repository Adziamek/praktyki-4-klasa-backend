using Warehouse.Application.Common;
using Warehouse.Application.DTO.Brand;

namespace Warehouse.Application.Interfaces;

public interface IBrandService
{
    Task<IReadOnlyList<BrandResponseDto>> GetAllAsync();
    Task<BrandResponseDto> GetByIdAsync(int id);

    Task<OperationResult<BrandResponseDto>> AddAsync(BrandDto dto);
    Task<OperationResult<BrandResponseDto>> UpdateAsync(int id, BrandDto dto);

    Task<OperationResult> DeleteAsync(int id);
}