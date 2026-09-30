using Warehouse.Application.Common;
using Warehouse.Application.DTO.CustomerOrder;

namespace Warehouse.Application.Interfaces;

public interface ICustomerOrderService
{
    Task<IReadOnlyList<CustomerOrderResponseDto>> GetAllAsync();
    Task<CustomerOrderResponseDto?> GetByIdAsync(int id);

    Task<OperationResult<CustomerOrderResponseDto>> AddAsync(
        CustomerOrderDto dto);
}