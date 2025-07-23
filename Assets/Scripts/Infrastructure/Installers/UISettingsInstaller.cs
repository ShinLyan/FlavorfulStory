using UnityEngine;
using Zenject;
using FlavorfulStory.Infrastructure.Configs.Notifications;

namespace FlavorfulStory.Infrastructure.Installers
{
    /// <summary> Инсталлер настроек UI, включая конфигурацию системы уведомлений. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Installers/UISettingsInstaller")]
    public class UISettingsInstaller : ScriptableObjectInstaller<UISettingsInstaller>
    {
        /// <summary> Настройки системы уведомлений. </summary>
        [field: SerializeField, Tooltip("Настройки системы уведомлений")]
        public NotificationSystemSettings NotificationSettings { get; private set; }

        /// <summary> Регистрирует зависимости UI в контейнере Zenject. </summary>
        public override void InstallBindings()
        {
            Container.BindInstance(NotificationSettings).AsSingle();
        }
    }
}