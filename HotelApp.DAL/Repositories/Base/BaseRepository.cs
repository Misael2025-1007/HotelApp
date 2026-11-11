using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using HotelApp.DAL.Data;
using HotelApp.DAL.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.DAL.Repositories.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : AuditEntity
    {
        protected readonly HotelDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(HotelDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
            => await _dbSet.AsNoTracking().ToListAsync();

        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter)
            => await _dbSet.AsNoTracking().Where(filter).ToListAsync();

        public virtual async Task<TEntity?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
            => await _dbSet.AnyAsync(filter);

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            entity.ModifyDate = DateTime.Now;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}