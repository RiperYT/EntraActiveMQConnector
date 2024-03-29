﻿using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using U_ProxMicrosoftEntraIDConnector.Common;
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
            _brockerService = new ArtemisService(); //new ActiveMQClassicService(_settingsRepository);
            _entraService = new EntraService();

            try
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                {
                    if (!settings.DomenBrocker.IsNullOrEmpty())
                        _brockerService.Connect(settings.DomenBrocker, settings.PortBroker, settings.UsernameBroker, settings.PasswordBroker);
                    if (!settings.TenatId.IsNullOrEmpty())
                        _entraService.Connect(settings.TenatId, settings.ClientId, settings.ClientSecret);
                }
            }
            catch (Exception ex) {
                //Console.WriteLine(ex.Message);
                StaticConnections.Logger.Error(ex);
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
                    Thread.Sleep(10000);
            }
            while (true)
            {
                //Console.WriteLine("Start While");
                try
                {
                    var settingsTime = _settingsRepository.Get();
                    var date = DateTime.MinValue;
                    if (settingsTime != null)
                        date = settingsTime.LastGet;

                    var dateGet = DateTime.UtcNow;
                    var users = await _entraService.GetAllUsers();
                    //var oldUsers = _userRepository.GetAll();

                    var newUsers = new List<UserEntity>();
                    var updateUsers = new List<UserEntity>();

                    foreach (var user in users)
                    {
                        //var oldUser = oldUsers.FirstOrDefault(t => t.Id.Equals(user.Id));
                        var oldUser = _userRepository.Get(user.Id);
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
                                //Console.WriteLine("Update" + user.Id);
                                updateUsers.Add(user);
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Added" + user.Id);
                            newUsers.Add(user);
                        }
                    }
                    //Console.WriteLine(users.Count);
                    _userRepository.AddRange(newUsers);
                    //Console.WriteLine("Added");
                    _userRepository.UpdateRange(updateUsers);
                    //Console.WriteLine("Updated users");
                    StaticConnections.Logger.Info("Read EntraId");
                    _lastRead = DateTime.UtcNow;

                    if (await _brockerService.CheckConnection())
                    {
                        var settings = _settingsRepository.Get();
                        if (settings != null)
                        {
                            var usersSend = _userRepository.GetAll().Where(t => t.UpdateTime > settings.LastUpdate);
                            if (usersSend.Any())
                            {
                                await _brockerService.Send(JsonSerializer.Serialize(usersSend), settings.QueueName);
                                settings.LastUpdate = DateTime.UtcNow;
                                settings.LastGet = dateGet;
                                _settingsRepository.Add(settings);
                                StaticConnections.Logger.Info("Sent to brocker");
                            }
                            else
                                StaticConnections.Logger.Info("Nothing to sent to brocker");
                        }
                        else
                        {
                            StaticConnections.Logger.Info("Settings for brocker is null");
                        }
                    }
                    else
                    {
                        StaticConnections.Logger.Info("Brocker is not connected");
                    }
                }
                catch (Exception ex) {
                    //Console.WriteLine(ex.Message);
                    StaticConnections.Logger.Error(ex);
                }

                Thread.Sleep((int)_updateDeltaMinutes*60 * 1000);
                //Thread.Sleep(_lastRead.Millisecond + TimeSpan.FromMinutes(_updateDeltaMinutes).Milliseconds - DateTime.UtcNow.Millisecond);
                //Console.WriteLine("End");
            }
        }
    }
}
