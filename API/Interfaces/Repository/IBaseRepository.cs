using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces.Repository
{
    public interface IBaseRepository<T> where T : Entity
    {
        Task<bool> Insert(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Update(T entity);
        Task<T> SelectById(int id);
        Task<IEnumerable<T>> Select();
    }
}