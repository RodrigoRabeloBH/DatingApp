using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity
    {
        protected readonly DataContext _context;

        public BaseRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(T entity)
        {
            _context.Set<T>().Remove(entity);

            var result = await _context.SaveChangesAsync();

            return result != 0;
        }

        public async Task<bool> Insert(T entity)
        {
            _context.Set<T>().Add(entity);

            var result = await _context.SaveChangesAsync();

            return result != 0;
        }

        public async Task<IEnumerable<T>> Select()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> SelectById(int id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> Update(T entity)
        {
            _context.Set<T>().Update(entity);

            var result = await _context.SaveChangesAsync();

            return result != 0;
        }
    }
}