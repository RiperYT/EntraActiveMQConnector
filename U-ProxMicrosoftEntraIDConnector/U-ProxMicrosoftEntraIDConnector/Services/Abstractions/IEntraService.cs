﻿using Azure.Identity;
using Microsoft.Graph;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Services.Abstractions
{
    public interface IEntraService
    {
        public string Connect(string tenantId, string clientId);

        public Task<bool> CheckConnection();

        public Task<List<UserEntity>> GetAllUsers();

        //ToDO
        public Task<List<UserEntity>> GetDeltaUsers();
    }
}
