using Warehouse.Application.Common;
using Warehouse.Application.DTO.ReturnRequest;

namespace Warehouse.Application.Interfaces;

public interface IReturnRequestService
{
    Task<IReadOnlyList<ReturnRequestResponseDto>> GetAllAsync();
    Task<ReturnRequestResponseDto?> GetByIdAsync(int id);

    Task<OperationResult<ReturnRequestResponseDto>> AddAsync(
        ReturnRequestDto dto);
}