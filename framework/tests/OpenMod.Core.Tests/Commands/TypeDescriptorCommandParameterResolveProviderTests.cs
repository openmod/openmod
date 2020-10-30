using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenMod.Core.Commands;

namespace OpenMod.Core.Tests.Commands
{
    [TestClass]
    public class TypeDescriptorCommandParameterResolveProviderTests
    {
        private TypeDescriptorCommandParameterResolveProvider m_TypeDescriptorCommandParameterResolveProvider;

        [TestInitialize]
        public void Initialize()
        {
            IServiceProvider serviceProvider = Mock.Of<IServiceProvider>();

            m_TypeDescriptorCommandParameterResolveProvider =
                new TypeDescriptorCommandParameterResolveProvider(serviceProvider);
        }

        [DataTestMethod]
        [DataRow(typeof(bool))]
        [DataRow(typeof(sbyte))]
        [DataRow(typeof(byte))]
        [DataRow(typeof(char))]
        [DataRow(typeof(short))]
        [DataRow(typeof(ushort))]
        [DataRow(typeof(int))]
        [DataRow(typeof(uint))]
        [DataRow(typeof(float))]
        [DataRow(typeof(long))]
        [DataRow(typeof(ulong))]
        [DataRow(typeof(double))]
        [DataRow(typeof(string))]
        public void Supports_ShouldReturnTrue_WhenTypeIsSupported(Type type)
        {
            // Act
            bool supports = m_TypeDescriptorCommandParameterResolveProvider.Supports(type);

            // Assert
            Assert.IsTrue(supports);
        }

        [DataTestMethod]
        [DataRow(typeof(System.Console))]
        public void Supports_ShouldReturnFalse_WhenTypeIsNotSupported(Type type)
        {
            // Act
            bool supports = m_TypeDescriptorCommandParameterResolveProvider.Supports(type);

            // Assert
            Assert.IsFalse(supports);
        }

        [DataTestMethod]
        [DataRow("true", true)]
        [DataRow("1", (sbyte)1)]
        [DataRow("1", (byte)1)]
        [DataRow("1", '1')]
        [DataRow("1", (short)1)]
        [DataRow("1", (ushort)1)]
        [DataRow("1", 1)]
        [DataRow("1", 1u)]
        [DataRow("1", 1f)]
        [DataRow("1", 1L)]
        [DataRow("1", 1uL)]
        [DataRow("1", 1d)]
        [DataRow("1", "1")]
        public async Task ResolveAsync_ShouldReturnValidObject(string input, object expected)
        {
            // Arrange
            Type type = expected.GetType();

            // Act
            object actual = await m_TypeDescriptorCommandParameterResolveProvider.ResolveAsync(type, input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(typeof(System.Console))]
        public async Task ResolveAsync_ShouldThrowArgumentException_WhenTypeIsNotSupported(Type type)
        {
            // Arrange
            string input = "foo";

            // Act
            Task<object> resolveTask = m_TypeDescriptorCommandParameterResolveProvider.ResolveAsync(type, input);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await resolveTask, "The given type is not supported");
        }
    }
}
