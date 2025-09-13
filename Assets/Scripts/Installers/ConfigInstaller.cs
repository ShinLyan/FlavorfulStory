using UnityEngine;
using Zenject;
using FlavorfulStory.Notifications.Configs;
using FlavorfulStory.PickupSystem;
using FlavorfulStory.Windows;

namespace FlavorfulStory.Installers
{
    /// <summary> Инсталлер для биндинга ScriptableObject-конфигов. </summary>
    [CreateAssetMenu(fileName = "ConfigInstaller", menuName = "FlavorfulStory/Installers/ConfigInstaller")]
    public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
    {
        /// <summary> Адреса всех UI-окон (префабы). </summary>
        [SerializeField] private WindowAddresses _windowAddresses;

        /// <summary> Глобальные настройки системы уведомлений. </summary>
        [SerializeField] private NotificationsSettings _notificationSettings;

        /// <summary> Настройки затемнения фона и HUD при открытии окон. </summary>
        [SerializeField] private OverlayFadeSettings _overlayFadeSettings;

        /// <summary> Настройки затемнения фона и HUD при открытии окон. </summary>
        [SerializeField] private PickupSettings _pickupSettings;
        
        /// <summary> Привязывает конфиги в контейнер Zenject как singleton-инстансы. </summary>
        public override void InstallBindings()
        {
            Container.Bind<WindowAddresses>().FromInstance(_windowAddresses).AsSingle();
            Container.Bind<NotificationsSettings>().FromInstance(_notificationSettings).AsSingle();
            Container.Bind<OverlayFadeSettings>().FromInstance(_overlayFadeSettings).AsSingle();
            Container.Bind<PickupSettings>().FromInstance(_pickupSettings).AsSingle();
        }
    }
}