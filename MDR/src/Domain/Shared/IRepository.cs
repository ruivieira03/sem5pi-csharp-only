
namespace Hospital.Domain.Shared{
    public interface IRepository<TEntity, TEntityId>{
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TEntityId id);
        Task<List<TEntity>> GetByIdsAsync(List<TEntityId> ids);
        Task<TEntity> AddAsync(TEntity obj);
        Task SaveChangesAsync();
        void Remove(TEntity obj);
    }
}