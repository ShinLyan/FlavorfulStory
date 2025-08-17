using FlavorfulStory.Notifications.Configs;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Infrastructure.Installers
{
    [CreateAssetMenu(fileName = "ConfigInstaller", menuName = "Installers/ConfigInstaller")]
    public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
    {
        [SerializeField] private WindowAdresses _windowAdresses;
        [SerializeField] private NotificationsSettings _notificationSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<WindowAdresses>().FromInstance(_windowAdresses).AsSingle();
            Container.Bind<NotificationsSettings>().FromInstance(_notificationSettings).AsSingle();
        }
    }
}