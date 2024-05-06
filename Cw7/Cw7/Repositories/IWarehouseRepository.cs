namespace Cw7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExistAsync(int id);
    Task<bool> DoesWarehouseExistAsync(int id);
    Task<int> IsOrderedAndNotFulfilledAsync(int id, int amount,DateTime date);
    Task FulfillAsync(int idO);
    Task<int> InsertProductWarehouseAsync(int IdP, int idW, int Amount);
}