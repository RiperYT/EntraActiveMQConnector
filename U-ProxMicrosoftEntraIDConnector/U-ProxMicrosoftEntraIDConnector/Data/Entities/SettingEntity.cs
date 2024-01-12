using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace U_ProxMicrosoftEntraIDConnector.Data.Entities
{
    public class SettingEntity
    {
        [Key]
        [Column("domen_brocker")]
        public string DomenBrocker { get; set; }

        [Column("port_brocker")]
        public string PortBroker { get; set; }

        [Column("username_brocker")]
        public string UsernameBroker { get; set; }

        [Column("password_brocker")]
        public string PasswordBroker { get; set; }

        [Column("queue_name")]
        public string QueueName { get; set; }

        [Column("last_update")]
        public DateTime LastUpdate { get; set; }

        [Column("last_get")]
        public DateTime LastGet { get; set; }


        [Column("tenatId")]
        public string TenatId { get; set; }

        [Column("clientId")]
        public string ClientId { get; set; }

        [Column("client_secret")]
        public string ClientSecret { get; set; }

        //[Key]
        //[Column("tenantId_Entra")]
        //public string TenatIdEntra { get; set; }

        //[Column("clientId_Entra")]
        //public string ClientIdEntra { get; set; }


        public SettingEntity(string domenBrocker, string portBroker, string usernameBroker, string passwordBroker, string queueName,  DateTime lastUpdate, DateTime lastGet, string tenatId, string clientId, string clientSecret)
        {
            DomenBrocker = domenBrocker;
            PortBroker = portBroker;
            UsernameBroker = usernameBroker;
            PasswordBroker = passwordBroker;
            QueueName = queueName;
            LastUpdate = lastUpdate;
            LastGet = lastGet;

            TenatId = tenatId;
            ClientId = clientId;
            ClientSecret = clientSecret;

            //TenatIdEntra = tenatIdEntra;
            //ClientIdEntra = clientIdEntra;

        }
    }
}
