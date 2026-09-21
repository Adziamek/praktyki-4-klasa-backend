namespace Warehouse.Application.DTO.Location;

public class LocationDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}