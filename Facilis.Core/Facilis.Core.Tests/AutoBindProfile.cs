using Facilis.Core.Tests.Models;
using Facilis.ExtendedEntities.Abstractions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.Json;

namespace Facilis.Core.Tests
{
    public class AutoBindProfile
    {
        private Instances instances { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.instances = new Instances();
        }

        [Test]
        public void TestAddUserWithProfile_AttributesAutoAdded()
        {
            // Arrange
            var users = this.instances.GetEntities<User>();
            var user = new User();

            user.SetProfile(new UserProfile() { Copy = new() });
            users.Add(user);

            // Act
            var profile = user.GetProfile<UserProfile>();
            var attributes = this.instances
                .GetEntities<ExtendedAttribute>()
                .WhereEnabled(entity => entity.ScopedId == user.Id)
                .ToDictionary(entity => entity.Key);

            // Assert
            Assert.AreEqual(profile.FirstName, attributes["FirstName"].Value);
            Assert.AreEqual(profile.LastName, attributes["LastName"].Value);
            Assert.AreEqual(profile.Phone, attributes["Phone"].Value);

            Assert.AreEqual(profile.Age, JsonSerializer.Deserialize<int>(attributes["Age"].Value));
            Assert.AreEqual(profile.HeightInCm, JsonSerializer.Deserialize<decimal>(attributes["HeightInCm"].Value));
            Assert.AreEqual(profile.LastSignInAtUtc, JsonSerializer.Deserialize<DateTime>(attributes["LastSignInAtUtc"].Value));

            Assert.AreEqual(profile.Avatar, attributes["Avatar"].Value);
            Assert.NotNull(JsonSerializer.Deserialize<UserProfile>(attributes["Copy"].Value).FirstName);

            Assert.Pass();
        }
    }
}