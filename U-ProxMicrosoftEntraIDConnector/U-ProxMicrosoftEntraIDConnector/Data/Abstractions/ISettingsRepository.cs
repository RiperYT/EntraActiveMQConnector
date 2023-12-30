using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Data.Abstractions
{
    public interface ISettingsRepository
    {
        public void Add(SettingsEntity settings);

        public SettingsEntity? Get();
    }
}
