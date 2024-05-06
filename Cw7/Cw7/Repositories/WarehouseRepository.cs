using Microsoft.Data.SqlClient;

namespace Cw7.Repositories;

public class WarehouseRepository:IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> DoesProductExistAsync(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM Product p " +
                              "WHERE p.IdProduct=@idProduct";
        command.Parameters.AddWithValue("@idProduct", id);
        var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    public async Task<bool> DoesWarehouseExistAsync(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM Warehouse w " +
                              "WHERE w.IdWarehouse=@idWarehouse";
        command.Parameters.AddWithValue("@idWarehouse", id);
        var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    public async Task<bool> IsOrderedAndNotFulfilledAsync(int id, int amount,DateTime date)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM Order o " +
                              "WHERE @amount=o.Amount AND o.IdProduct=@id AND @date>o.CreatedAt AND o.FulfilledAt is null";
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@date", date);
        var reader = await command.ExecuteReaderAsync();
        
        return await reader.ReadAsync();
    }

    public Task FulfillAsync(int id, int amount, string date)
    {
        throw new NotImplementedException();
    }

    public Task<int> InsertProductWarehouseAsync(int IdP, int idW, int Amount)
    {
        throw new NotImplementedException();
    }
}