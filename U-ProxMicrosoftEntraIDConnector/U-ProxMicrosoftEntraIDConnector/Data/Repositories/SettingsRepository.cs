﻿using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Data.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DataContext _context;

        public SettingsRepository(DataContext context) {
            _context = context;
        }

        public void Add(SettingsEntity settings)
        {
            var settingsBefore = Get();
            if (settingsBefore != null)
                //_context.Settings.Remove(settingsBefore);
            _context.Set<SettingsEntity>().Remove(settingsBefore);

            _context.Set<SettingsEntity>().Add(settings);
            _context.SaveChanges();
            //_context.
        }

        public SettingsEntity? Get()
        {
            //_context.Settings.ToList();
            return _context.Set<SettingsEntity>().FirstOrDefault();
        }
    }
}
