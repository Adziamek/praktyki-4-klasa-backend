using Warehouse.Application.Common;
using Warehouse.Application.DTO.Category;

namespace Warehouse.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto?> GetByIdAsync(int id);

    Task<OperationResult<CategoryResponseDto>> AddAsync(CategoryDto dto);
    Task<OperationResult<CategoryResponseDto>> UpdateAsync(int id, CategoryDto dto);

    Task<OperationResult> DeleteAsync(int id);
}