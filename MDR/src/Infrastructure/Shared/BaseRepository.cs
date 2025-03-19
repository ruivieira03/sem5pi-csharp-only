using Microsoft.EntityFrameworkCore;
using Hospital.Domain.Shared;


namespace Hospital.Infrastructure.Shared{
    public class BaseRepository<TEntity,TEntityId> : IRepository<TEntity,TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : EntityId{
        protected readonly DbSet<TEntity> _objs;
        protected readonly HospitalDbContext _context; 
        
        public BaseRepository(HospitalDbContext context){
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _objs = context.Set<TEntity>();
        }

        public async Task<List<TEntity>> GetAllAsync(){
            return await this._objs.ToListAsync();
        }
        
        public async Task<TEntity> GetByIdAsync(TEntityId id){
            //return await this._context.Categories.FindAsync(id);
            return await this._objs
                .Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }
        public async Task<List<TEntity>> GetByIdsAsync(List<TEntityId> ids){
            return await this._objs
                .Where(x => ids.Contains(x.Id)).ToListAsync();
        }
        public async Task<TEntity> AddAsync(TEntity obj){
            var ret = await this._objs.AddAsync(obj);
            return ret.Entity;
        }

        public async Task SaveChangesAsync(){
            await this._context.SaveChangesAsync();
        }

        public void Remove(TEntity obj){
            this._objs.Remove(obj);
        }
    }
}