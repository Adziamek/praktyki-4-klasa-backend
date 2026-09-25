using Warehouse.Application.DTO.CustomerOrder;

namespace Warehouse.Application.Interfaces;

public interface ICustomerOrderService
{
    Task<IReadOnlyList<CustomerOrderResponseDto>> GetAllAsync();
    Task<CustomerOrderResponseDto?> GetByIdAsync(int id);
    Task<CustomerOrderOperationResult> AddAsync(CustomerOrderDto dto);
}
public record CustomerOrderOperationResult(CustomerOrderResponseDto? CustomerOrder, string? Error, int? StatusCode)
{
    public bool Success => CustomerOrder != null;
    public static CustomerOrderOperationResult Ok(CustomerOrderResponseDto customerOrder) =>
        new(customerOrder, null, null);
    public static CustomerOrderOperationResult Fail(string error, int statusCode) =>
        new(null, error, statusCode);
}