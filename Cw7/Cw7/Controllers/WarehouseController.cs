using Cw7.Models.DTOs;
using Cw7.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Cw7.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController: ControllerBase
{

    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    [HttpPost("/warehouses")]
    public async Task<IActionResult> PostWarehouse(PostWarehouse body)
    {
        
        if (! await _warehouseRepository.DoesProductExistAsync(body.IdProduct) 
            || ! await _warehouseRepository.DoesWarehouseExistAsync(body.IdWarehouse))
        {
            return NotFound("No records found with given IdProduct or IdWarehouse or both");
        }

        if (body.Amount<=0)
        {
            return BadRequest("Amount needs to be >0");
        }

        var idO = await _warehouseRepository.IsOrderedAndNotFulfilledAsync(body.IdProduct, body.Amount, body.CreatedAt);
        if ( idO== -1)
        {
            return NotFound("No such order");
        }

        var newId = await _warehouseRepository.InsertProductWarehouseAsync(body.IdProduct, body.IdWarehouse, idO,
            body.Amount);
        if (newId==-1)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,"error on a server side");
        }
        
        return Created("Registered product",newId);
    }
    
}