using Microsoft.EntityFrameworkCore;
using TicketResell.Repository.Core.Context;

namespace TicketResell.Repository.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly TicketResellManagementContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(TicketResellManagementContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<T> GetByIdAsync(string id)
        {
            T? entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Id not found");
            }
            return entity;
        }
        public async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteByIdAsync(string id)
        {
            T? entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Id not found");
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}