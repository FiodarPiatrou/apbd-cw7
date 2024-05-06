using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Cw7.Repositories;

public class WarehouseRepository : IWarehouseRepository
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

    public async Task<int> IsOrderedAndNotFulfilledAsync(int id, int amount, DateTime date)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT TOP 1 idOrder FROM Order o " +
                              "WHERE @amount=o.Amount AND o.IdProduct=@id AND @date>o.CreatedAt AND o.FulfilledAt is null";
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@date", date);
        var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var ordinal = reader.GetOrdinal("idOrder");
            return reader.GetInt32(ordinal);
        }

        return -1;
    }

    public async Task FulfillAsync(int idO)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        using SqlCommand command = new SqlCommand();
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = (SqlTransaction)transaction;
        try
        {
            command.CommandText = "UPDATE Order SET Fulfilled=@dateNow " +
                                  "where idOrder=@idO";
            command.Parameters.AddWithValue("@dateNow", DateTime.Now);
            command.Parameters.AddWithValue("@idO", idO);
            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }
        catch (SqlException e)
        {
            await transaction.RollbackAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
        }
    }

    public async Task<int> InsertProductWarehouseAsync(int idP, int idW, int idO, int amount)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        using SqlCommand command = new SqlCommand();
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = (SqlTransaction)transaction;
        int result = -1;
        try
        {
            command.CommandText = "Select Price from Product where IdProduct=@idP";
            command.Parameters.AddWithValue("@idP", idP);
            var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            double price = reader.GetDouble(reader.GetOrdinal("IdProduct"));
            command.Parameters.Clear();
            command.CommandText = "Insert into Product_Warehouse " +
                                  "(IdWarehouse,IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                                  "values (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*@Price, @CreatedAt)";
            command.Parameters.AddWithValue("@IdWarehouse",idW);
            command.Parameters.AddWithValue("@IdOrder", idO);
            command.Parameters.AddWithValue("@IdProduct", idP);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@CreateAt", DateTime.Now);
            command.Parameters.AddWithValue("@Price", price);
            await command.ExecuteNonQueryAsync();
            command.CommandText = "SELECT @@IDENTITY as NewId";
            reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            result = reader.GetInt32(reader.GetOrdinal("NewId"));
            await transaction.CommitAsync();
        }
        catch (SqlException e)
        {
            await transaction.RollbackAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
        }

        return result;
    }
}