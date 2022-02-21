using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text.Json;

namespace Facilis.Core.Tests.Models
{
    public class User : IEntityWithId, IEntityWithStatus, IEntityWithProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public StatusTypes Status { get; set; }

        [NotMapped]
        public object Profile { get; set; }

        public string SerializedProfile { get; set; }

        #region Constructor(s)

        public User()
        {
            this.Profile = this.GetProfile<UserProfile>();
        }

        #endregion Constructor(s)

        public T GetProfile<T>()
        {
            return this.SerializedProfile == null ? default :
                JsonSerializer.Deserialize<T>(this.SerializedProfile);
        }

        public void SetProfile(object profile)
        {
            this.Profile = profile;
            this.SerializedProfile = JsonSerializer.Serialize(profile);
        }
    }

    public class UserProfile
    {
        public string FirstName { get; set; } = "Tony";
        public string LastName { get; set; } = "Stark";
        public string Phone { get; set; }
        public int Age { get; set; } = 48;
        public string[] AddressLines { get; set; } = { "890 Fifth Avenue", "Manhattan", "New York City" };
        public decimal HeightInCm { get; set; } = 185.42m;
        public DateTime LastSignInAtUtc { get; set; } = DateTime.UtcNow;
        public FileInfo Avatar { get; set; }
        public UserProfile Copy { get; set; }
    }
}