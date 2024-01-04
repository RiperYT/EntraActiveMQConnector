using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Graph.Models;
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

        private readonly IEntraService _entraService;
        //private readonly EntraService _entraService;
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
            _entraService = new EntraService();
            //_entraService = new EntraService();

            try
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                {
                    _brockerService.Connect(settings.DomenBrocker, settings.PortBroker, settings.UsernameBroker, settings.PasswordBroker);
                    //_entraService.Connect(settings.TenatIdEntra, settings.ClientIdEntra);
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
                /*var p = false;
                try
                {
                    await _entraService.GetAllUsers();
                    p = true;
                }
                catch (Exception _) { }*/

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
                    //Console.WriteLine($"{users.Count} users");
                    var oldUsers = _userRepository.GetAll();
                    //Console.WriteLine($"{oldUsers.Count} old users ");

                    //var newUsers = users.Where(user => oldUsers.First(userOld => userOld.Id.Equals(user.Id)) == null).ToList();
                    //_userRepository.AddRange(newUsers);

                    var newUsers = new List<UserEntity>();
                    var updateUsers = new List<UserEntity>();

                    //users = users.Where(user => oldUsers.First(userOld => userOld.Id.Equals(user.Id)) != null).ToList();
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

                    if (_brockerService.CheckConnection())
                    {
                        var settings = _settingsRepository.Get();
                        if (settings != null)
                        {
                            var usersSend = _userRepository.GetAll().Select(t => t.UpdateTime > settings.LastUpdate);
                            _brockerService.Send(JsonSerializer.Serialize(usersSend), "q");
                            settings.LastUpdate = DateTime.Now;
                            _settingsRepository.Add(settings);
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }

                //Thread.Sleep(_lastRead.AddMinutes(_updateDeltaMinutes).Microsecond - DateTime.Now.Microsecond);
                Thread.Sleep(1000);
                Console.WriteLine("End");
            }
        }

        /*public async Task StopAsync(CancellationToken cancellationToken)
        {
        }*/
    }
}
