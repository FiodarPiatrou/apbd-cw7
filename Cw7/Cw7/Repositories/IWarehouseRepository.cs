namespace Cw7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExistAsync(int id);
    Task<bool> DoesWarehouseExistAsync(int id);
    Task<bool> IsOrderedAndNotFulfilledAsync(int id, int amount,DateTime date);
    Task FulfillAsync(int id, int amount, string date);
    Task<int> InsertProductWarehouseAsync(int IdP, int idW, int Amount);
}