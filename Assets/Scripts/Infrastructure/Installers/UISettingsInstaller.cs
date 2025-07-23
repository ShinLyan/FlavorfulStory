using FlavorfulStory.Infrastructure.Configs.Notifications;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Infrastructure.Installers
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Installers/UISettingsInstaller")]
    public class UISettingsInstaller : ScriptableObjectInstaller<UISettingsInstaller>
    {
        // Сюда хакидываем все UI-конфигиы
        public NotificationSystemSettings NotificationSettings;

        public override void InstallBindings()
        {
            // Биндим все, что закинули
            Container.BindInstance(NotificationSettings).AsSingle();
        }
    }
}