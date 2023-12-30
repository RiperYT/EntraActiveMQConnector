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
            return _context.Set<UserEntity>().Where(t => t.Id.Equals(id)).FirstOrDefault();
        }

        public List<UserEntity> GetAll()
        {
            return _context.Set<UserEntity>().ToList();
        }

        public void AddRange(List<UserEntity> users)
        {
            _context.Set<UserEntity>().AddRange(users);
        }

        public void UpdateRange(List<UserEntity> users)
        {
            _context.Set<UserEntity>().UpdateRange(users);
        }
    }
}
