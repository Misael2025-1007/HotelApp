using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Repositories.Base
{
    public interface IBaseRepository<TEntity> where TEntity : AuditEntity
    {
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
    }
}