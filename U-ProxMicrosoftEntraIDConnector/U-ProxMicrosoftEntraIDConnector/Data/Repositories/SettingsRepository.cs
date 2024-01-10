using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Data.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DataContext _context;

        public SettingsRepository(DataContext context) {
            _context = context;
        }

        public void Add(SettingEntity settings)
        {
            var settingsBefore = Get();
            if (settingsBefore != null)
                //_context.Settings.Remove(settingsBefore);
            _context.Set<SettingEntity>().Remove(settingsBefore);
            _context.SaveChanges();

            _context.Set<SettingEntity>().Add(settings);
            _context.SaveChanges();
            //_context.
        }

        public SettingEntity? Get()
        {
            //_context.Settings.ToList();
            return _context.Set<SettingEntity>().FirstOrDefault();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
