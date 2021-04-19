using Cysharp.Threading.Tasks;
using HarmonyLib;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.RocketMod.Economy.Patches;
using Rocket.Core.Plugins;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

#pragma warning disable IDE0079 // Remove unnecessary suppression
namespace OpenMod.Unturned.RocketMod.Economy
{
    public class UconomyEconomyProvider : IEconomyProvider, IDisposable
    {
        private const BindingFlags c_BindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        private readonly IRocketModComponent m_RocketModComponent;
        private readonly IEventBus m_EventBus;
        private bool m_UconomyReady;
        private Harmony? m_HarmonyInstance;
        private Type? m_DatabaseType;
        private MethodInfo? m_GetBalanceMethod;
        private MethodInfo? m_IncreaseBalanceMethod;
        private FieldInfo? m_DatabaseField;
        private object? m_UconomyInstance;
        private object? m_DatabaseInstance;
        private FieldInfo? m_MoneySymbolField;
        private FieldInfo? m_MoneyNameField;
        private object? m_UconomyConfigurationInstance;

        public UconomyEconomyProvider(
            IRocketModComponent rocketModComponent,
            IEventBus eventBus)
        {
            m_RocketModComponent = rocketModComponent;
            m_EventBus = eventBus;
        }

        private void PatchUconomy()
        {
            m_HarmonyInstance = new Harmony(UconomyIntegration.HarmonyId);

            var patchMethod = m_DatabaseType!.GetMethod("IncreaseBalance", c_BindingFlags);
            var increaseBalancePostfix = typeof(UconomyBalanceIncreasePatch)
                .GetMethod(nameof(UconomyBalanceIncreasePatch.IncreaseBalancePostfix), c_BindingFlags);

            m_HarmonyInstance.Patch(patchMethod, postfix: new HarmonyMethod(increaseBalancePostfix));
            UconomyBalanceIncreasePatch.OnPostIncreaseBalance += OnPostBalanceUpdated;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private bool EnsureUconomyReady()
        {
            if (m_UconomyReady)
            {
                return true;
            }

            if (!RocketModIntegration.IsRocketModReady())
            {
                return false;
            }

            var uconomyAssembly = UconomyIntegration.GetUconomyAssembly();
            if (uconomyAssembly == null)
            {
                return false;
            }

            var uconomyType = uconomyAssembly.GetType("fr34kyn01535.Uconomy.Uconomy");
            if (uconomyType == null)
            {
                throw new Exception($"Failed to find Uconomy type in {uconomyAssembly}");
            }

            var instanceField = uconomyType.GetField("Instance", c_BindingFlags);
            if (instanceField == null)
            {
                throw new Exception($"Failed to find Instance field in {uconomyType}");
            }

            m_DatabaseField = uconomyType.GetField("Database", c_BindingFlags);
            m_DatabaseType = m_DatabaseField.FieldType;
            m_GetBalanceMethod = m_DatabaseType.GetMethod("GetBalance", c_BindingFlags);
            m_IncreaseBalanceMethod = m_DatabaseType.GetMethod("IncreaseBalance", c_BindingFlags);

            var uconomyConfigurationType = uconomyAssembly.GetType("fr34kyn01535.Uconomy.UconomyConfiguration");

            m_MoneySymbolField = uconomyConfigurationType.GetField("MoneySymbol", c_BindingFlags);
            m_MoneyNameField = uconomyConfigurationType.GetField("MoneyName", c_BindingFlags);

            m_UconomyInstance = instanceField.GetValue(null);
            if (m_UconomyInstance == null)
            {
                return false;
            }

            var pluginType = typeof(RocketPlugin<>).MakeGenericType(uconomyConfigurationType);
            var pluginConfigurationAssetField = pluginType.GetField("configuration", c_BindingFlags);
            var pluginConfigurationAssetInstance = pluginConfigurationAssetField.GetValue(m_UconomyInstance);

            var uconomyConfigurationInstanceProperty = pluginConfigurationAssetField.FieldType.GetProperty("Instance", c_BindingFlags);
            m_UconomyConfigurationInstance = uconomyConfigurationInstanceProperty.GetGetMethod()
                .Invoke(pluginConfigurationAssetInstance, new object[0]);

            m_DatabaseInstance = m_DatabaseField.GetValue(m_UconomyInstance);
            if (m_DatabaseInstance == null)
            {
                return false;
            }

            PatchUconomy();
            m_UconomyReady = true;
            return true;
        }

        private void OnPostBalanceUpdated(string id, decimal increaseBy, decimal amt)
        {
            var @event = new BalanceUpdatedEvent(id, KnownActorTypes.Player, amt - increaseBy, amt, null);
            AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_RocketModComponent, this, @event));
        }

        public string CurrencyName
        {
            get
            {
                return (string)m_MoneyNameField!.GetValue(m_UconomyConfigurationInstance);
            }
        }

        public string CurrencySymbol
        {
            get
            {
                return (string)m_MoneySymbolField!.GetValue(m_UconomyConfigurationInstance);
            }
        }

        public Task<decimal> GetBalanceAsync(string ownerId, string ownerType)
        {
            if (!EnsureUconomyReady())
            {
                return Task.FromException<decimal>(new Exception("Uconomy is not loaded."));
            }

            ValidateActorType(ownerType);

            async UniTask<decimal> GetBalance()
            {
                await UniTask.SwitchToMainThread();

                return (decimal)m_GetBalanceMethod!.Invoke(m_DatabaseInstance, new object[] { ownerId });
            }

            return GetBalance().AsTask();
        }

        public Task<decimal> UpdateBalanceAsync(string ownerId, string ownerType, decimal changeAmount, string? reason)
        {
            if (!EnsureUconomyReady())
            {
                return Task.FromException<decimal>(new Exception("Uconomy is not loaded."));
            }

            ValidateActorType(ownerType);

            async UniTask<decimal> UpdateBalance()
            {
                await UniTask.SwitchToMainThread();

                return (decimal)m_IncreaseBalanceMethod!.Invoke(m_DatabaseInstance, new object[] { ownerId, changeAmount });
            }

            return UpdateBalance().AsTask();
        }

        public Task SetBalanceAsync(string ownerId, string ownerType, decimal balance)
        {
            if (!EnsureUconomyReady())
            {
                return Task.FromException<decimal>(new Exception("Uconomy is not loaded."));
            }

            ValidateActorType(ownerType);

            async UniTask<decimal> SetBalance()
            {
                await UniTask.SwitchToMainThread();

                var currentBalance = (decimal)m_GetBalanceMethod!.Invoke(m_DatabaseInstance, new object[] { ownerId });

                return (decimal)m_IncreaseBalanceMethod!.Invoke(m_DatabaseInstance, new object[] { ownerId, balance - currentBalance });
            }

            return SetBalance().AsTask();
        }

        private void ValidateActorType(string ownerType)
        {
            if (!ownerType.Equals(KnownActorTypes.Player, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException($"Uconomy does not support actorType \"{ownerType}\"");
            }
        }

        public void Dispose()
        {
            UconomyBalanceIncreasePatch.OnPostIncreaseBalance -= OnPostBalanceUpdated;
            m_HarmonyInstance?.UnpatchAll(UconomyIntegration.HarmonyId);
        }
    }
}