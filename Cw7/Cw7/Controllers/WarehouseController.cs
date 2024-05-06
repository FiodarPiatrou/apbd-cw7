using Microsoft.AspNetCore.Mvc;

namespace Cw7.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController: ControllerBase
{
    [HttpPost("/warehouses")]
    public IActionResult PostWarehouse(int idProduct,int idWarehouse, int amount,string date)
    {
        return Ok();
    }
    
}