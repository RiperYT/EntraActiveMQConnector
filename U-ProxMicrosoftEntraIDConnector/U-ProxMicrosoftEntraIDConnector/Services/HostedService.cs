﻿using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Graph.Models;
using System.Text.Json;
using U_ProxMicrosoftEntraIDConnector.Data;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;
using U_ProxMicrosoftEntraIDConnector.Data.Repositories;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Services
{
    public class HostedService
    {
        private const double _updateDeltaMinutes = 15;

        private readonly IEntraService _entraService;
        private readonly IBrockerService _brockerService;
        private readonly IUserRepository _userRepository;
        private readonly ISettingsRepository _settingsRepository;

        private DateTime _lastRead;
        private DateTime _lastSend;

        public HostedService()
        {
            var dataContext = new DataContext();
            _settingsRepository = new SettingsRepository(dataContext);
            _userRepository = new UserRepository(dataContext);
            _brockerService = new ArtemisService(_settingsRepository);
            _entraService = new EntraService();

            try
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                {
                    _brockerService.Connect(settings.DomenBrocker, settings.PortBroker, settings.UsernameBroker, settings.PasswordBroker);
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            _ = StartAsync();
        }
        public async Task StartAsync()
        {
            while (true)
            {
                if (await _entraService.CheckConnection())
                    break;
                else
                    Thread.Sleep(120000);
            }
            while (true)
            {
                Console.WriteLine("Start While");
                try
                {
                    var users = await _entraService.GetAllUsers();
                    var oldUsers = _userRepository.GetAll();

                    var newUsers = new List<UserEntity>();
                    var updateUsers = new List<UserEntity>();

                    foreach (var user in users)
                    {
                        var oldUser = oldUsers.First(t => t.Id.Equals(user.Id));
                        if (oldUser != null)
                        {
                            var p = false;
                            if (!oldUser.Name.Equals(user.Name))
                                p = true;
                            if (!oldUser.Surname.Equals(user.Surname))
                                p = true;
                            if (!oldUser.JobTitle.Equals(user.JobTitle))
                                p = true;
                            if (!oldUser.Mail.Equals(user.Mail))
                                p = true;
                            if (!oldUser.MobilePhone.Equals(user.MobilePhone))
                                p = true;

                            if (p == true)
                            {
                                updateUsers.Add(user);
                            }
                        }
                        else
                        {
                            newUsers.Add(user);
                        }
                    }
                    _userRepository.AddRange(newUsers);
                    _userRepository.UpdateRange(updateUsers);
                    _lastRead = DateTime.Now;

                    if (await _brockerService.CheckConnection())
                    {
                        var settings = _settingsRepository.Get();
                        if (settings != null)
                        {
                            var usersSend = _userRepository.GetAll().Select(t => t.UpdateTime > settings.LastUpdate);
                            await _brockerService.Send(JsonSerializer.Serialize(usersSend), settings.QueueName);
                            settings.LastUpdate = DateTime.Now;
                            _settingsRepository.Add(settings);
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }

                Thread.Sleep(1000);
                Console.WriteLine("End");
            }
        }
    }
}
