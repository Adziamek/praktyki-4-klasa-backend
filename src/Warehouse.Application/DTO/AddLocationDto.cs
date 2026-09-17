namespace Warehouse.Application.DTO;

public class AddLocationDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}