using Microsoft.EntityFrameworkCore;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public UserEntity? Get(string id)
        {
            var user = _context.Set<UserEntity>().AsNoTracking().Where(t => t.Id.Equals(id)).FirstOrDefault();
            _context.SaveChanges();
            return user;
        }

        public List<UserEntity> GetAll()
        {
            var users = _context.Set<UserEntity>().AsNoTracking().ToList();
            _context.SaveChanges();
            return users;
        }

        public void AddRange(List<UserEntity> users)
        {
            _context.Set<UserEntity>().AddRange(users);
            _context.SaveChanges();
        }

        public void UpdateRange(List<UserEntity> users)
        {
            _context.Set<UserEntity>().UpdateRange(users);
            _context.SaveChanges();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
