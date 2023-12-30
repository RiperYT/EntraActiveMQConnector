using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Data.Abstractions
{
    public interface IUserRepository
    {
        public UserEntity? Get(string id);
        public List<UserEntity> GetAll();

        public void AddRange(List<UserEntity> users);

        public void UpdateRange(List<UserEntity> users);
    }
}
