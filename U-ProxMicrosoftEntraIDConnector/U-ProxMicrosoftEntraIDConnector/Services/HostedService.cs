﻿using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.Json;
using U_ProxMicrosoftEntraIDConnector.Data;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;
using U_ProxMicrosoftEntraIDConnector.Data.Repositories;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Services
{
    public class HostedService /*: IHostedService*/
    {
        private const double _updateDeltaMinutes = 15;

        private readonly IEntraService _entraservice;
        private readonly IBrockerService _brockerService;
        private readonly IUserRepository _userRepository;
        private readonly ISettingsRepository _settingsRepository;

        private DateTime _lastRead;
        private DateTime _lastSend;

        public HostedService(/*ISettingsRepository settings, IUserRepository userRepository, IBrockerService brockerService, IEntraService entraService*/)
        {
            /*_settingsRepository = settings;
            _userRepository = userRepository;   
            _brockerService = brockerService;
            _entraservice = entraService;*/
            var dataContext = new DataContext();
            _settingsRepository = new SettingsRepository(dataContext);
            _userRepository = new UserRepository(dataContext);
            _brockerService = new ArtemisService();
            _entraservice = new EntraService();
            //new Thread(() => StartAsync()).Start();
            _ = StartAsync();
        }
        public async Task StartAsync()
        {
            while (true)
            {
                if (_entraservice.CheckConnection())
                    break;
                else
                    Thread.Sleep(5000);
            }
            while (true)
            {
                var users = await _entraservice.GetAllUsers();
                var oldUsers = _userRepository.GetAll();

                //var newUsers = users.Where(user => oldUsers.First(userOld => userOld.Id.Equals(user.Id)) == null).ToList();
                //_userRepository.AddRange(newUsers);

                var newUsers = new List<UserEntity>();
                var updateUsers = new List<UserEntity>();

                //users = users.Where(user => oldUsers.First(userOld => userOld.Id.Equals(user.Id)) != null).ToList();
                foreach (var user in users )
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

                if (_brockerService.CheckConnection())
                {
                    var usersSend = _userRepository.GetAll().Select(t => t.UpdateTime > _lastSend);
                    _brockerService.Send(JsonSerializer.Serialize(usersSend), "q");
                    _lastSend = DateTime.Now;
                }
                Thread.Sleep(_lastRead.AddMinutes(_updateDeltaMinutes).Microsecond - DateTime.Now.Microsecond);
            }
        }

        /*public async Task StopAsync(CancellationToken cancellationToken)
        {
        }*/
    }
}
