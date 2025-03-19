
namespace Hospital.Domain.Shared{
    public interface IUnitOfWork{
        Task<int> CommitAsync();
    }
}