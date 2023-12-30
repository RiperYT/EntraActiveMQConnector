using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace U_ProxMicrosoftEntraIDConnector.Data.Entities
{
    public class UserEntity
    {
        [Key]
        [Column("user_id")]
        public string Id { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("jobTitle")]
        public string JobTitle { get; set; }

        [Column("mail")]
        public string Mail { get; set; }

        [Column("mobilePhone")]
        public string MobilePhone { get; set; }

        [Column("updateTime")]
        public DateTime UpdateTime {  get; set; }

        public UserEntity(string id, string surname, string name, string jobTitle, string mail, string mobilePhone, DateTime updateTime)
        {
            Id = id;
            Surname = surname;
            Name = name;
            JobTitle = jobTitle;
            Mail = mail;
            MobilePhone = mobilePhone;
            UpdateTime = updateTime;
        }
    }
}
