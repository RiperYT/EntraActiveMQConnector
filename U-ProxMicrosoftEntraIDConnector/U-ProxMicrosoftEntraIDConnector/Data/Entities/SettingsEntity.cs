﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace U_ProxMicrosoftEntraIDConnector.Data.Entities
{
    public class SettingsEntity
    {
        [Column("domen_brocker")]
        public string DomenBrocker { get; set; }

        [Column("port_brocker")]
        public string PortBroker { get; set; }

        [Column("username_brocker")]
        public string UsernameBroker { get; set; }

        [Column("password_brocker")]
        public string PasswordBroker { get; set; }

        [Key]
        [Column("tenantId_Entra")]
        public string TenatIdEntra { get; set; }

        [Column("clientId_Entra")]
        public string ClientIdEntra { get; set; }


        public SettingsEntity(string domenBrocker, string portBroker, string usernameBroker, string passwordBroker, string tenatIdEntra, string clientIdEntra)
        {
            DomenBrocker = domenBrocker;
            PortBroker = portBroker;
            UsernameBroker = usernameBroker;
            PasswordBroker = passwordBroker;
            TenatIdEntra = tenatIdEntra;
            ClientIdEntra = clientIdEntra;
        }
    }
}