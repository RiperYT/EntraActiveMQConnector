using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Data.Abstractions
{
    public interface ISettingsRepository
    {
        public void Add(SettingEntity settings);

        public SettingEntity? Get();
        public void SaveChanges();
    }
}
