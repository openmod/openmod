using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenMod.API.Users;
using OpenMod.Core.Users;

namespace OpenMod.Core.Tests.Users
{
    [TestClass]
    public class UserCommandParameterResolveProviderTests
    {
        private const string c_DefaultActorType = "user";
        private const char c_Separator = ':';

        private Mock<IUserManager> m_UserManagerMock;
        private UserCommandParameterResolveProvider m_UserCommandParameterResolveProvider;

        private static IEnumerable<object[]> GetRawAndSeparatedInput()
        {
            yield return new object[]
            {
                c_DefaultActorType + c_Separator + "foo",
                c_DefaultActorType,
                "foo"
            };
            yield return new object[]
            {
                "player" + c_Separator + "bar",
                "player",
                "bar"
            };
            yield return new object[]
            {
                c_DefaultActorType + c_Separator + "foo" + c_Separator + "bar",
                c_DefaultActorType,
                "foo" + c_Separator + "bar"
            };
            yield return new object[]
            {
                c_Separator + "foo",
                c_DefaultActorType,
                c_Separator + "foo"
            };
            yield return new object[]
            {
                "foo" + c_Separator,
                c_DefaultActorType,
                "foo" + c_Separator
            };
            yield return new object[]
            {
                "foo",
                c_DefaultActorType,
                "foo"
            };
        }

        [TestInitialize]
        public void Initialize()
        {
            m_UserManagerMock = new Mock<IUserManager>();
            m_UserCommandParameterResolveProvider =
                new UserCommandParameterResolveProvider(m_UserManagerMock.Object, c_DefaultActorType, c_Separator);
        }

        [DataTestMethod]
        [DataRow(typeof(IUser))]
        [DataRow(typeof(UserBase))]
        public void Supports_ShouldReturnTrue_WhenTypeIsSupported(Type type)
        {
            // Act
            bool supports = m_UserCommandParameterResolveProvider.Supports(type);

            // Assert
            Assert.IsTrue(supports);
        }

        [DataTestMethod]
        [DataRow(typeof(int))]
        [DataRow(typeof(string))]
        public void Supports_ShouldReturnFalse_WhenTypeIsNotSupported(Type type)
        {
            // Act
            bool supports = m_UserCommandParameterResolveProvider.Supports(type);

            // Assert
            Assert.IsFalse(supports);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetRawAndSeparatedInput), DynamicDataSourceType.Method)]
        public async Task ResolveAsync_ShouldReturnUser_WhenUserExists(
            string input, string actorType, string actorNameOrId)
        {
            // Arrange
            Type type = typeof(IUser);
            IUser expectedUser = Mock.Of<IUser>();

            m_UserManagerMock
                .Setup(x => x.FindUserAsync(actorType, actorNameOrId, It.IsAny<UserSearchMode>()))
                .ReturnsAsync(expectedUser);

            // Act
            IUser actualUser = (IUser)await m_UserCommandParameterResolveProvider.ResolveAsync(type, input);

            // Assert
            Assert.AreEqual(expectedUser, actualUser);
        }

        [DataTestMethod]
        [DataRow(typeof(int))]
        [DataRow(typeof(string))]
        public async Task ResolveAsync_ShouldThrowArgumentException_WhenTypeIsNotSupported(Type type)
        {
            // Arrange
            string input = "foo";

            // Act
            Task<object> resolveTask = m_UserCommandParameterResolveProvider.ResolveAsync(type, input);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await resolveTask, "The given type is not supported");
        }
    }
}
