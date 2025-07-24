using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.Stats;
using FlavorfulStory.Utils;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Инструмент, используемый игроком для взаимодействия с объектами. </summary>
    /// <remarks> Может выполнять действия, специфичные для типа инструмента. </remarks>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Tool")]
    public class Tool : InventoryItem, IUsable
    {
        #region Fields and Properties

        /// <summary> Тип инструмента. </summary>
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Кнопка использования инструмента. </summary>
        [field: Tooltip("Кнопка использования инструмента."), SerializeField]
        public UseActionType UseActionType { get; private set; }

        /// <summary> Тип SFX использования. </summary>
        [field: Tooltip("Тип SFX использования."), SerializeField]
        public SfxType SfxType { get; private set; }

        /// <summary> Стоимость использования по выносливости. </summary>
        [field: Tooltip("Стоимость использования по выносливости."), SerializeField]
        public float StaminaCost { get; private set; }

        // TODO: если появится
        // [field: SerializeField] public int Level { get; private set; }

        /// <summary> Максимальная дистанция взаимодействия инструментом. </summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> Радиус использования инструмента. </summary>
        private const float UseRadius = 1.5f;

        #endregion

        /// <summary> Использовать инструмент для взаимодействия с объектами. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        /// <param name="hitableLayers"> Слои, по которым будем делать удар. </param>
        public bool Use(PlayerController player, LayerMask hitableLayers)
        {
            if (!RaycastUtils.TryGetScreenPointToWorld(
                    InputWrapper.GetMousePosition(),
                    ~(1 << player.gameObject.layer),
                    out var targetPosition))
                return false;

            var stamina = player.GetComponent<PlayerStats>().GetStat<Stamina>();
            if (stamina == null || stamina.CurrentValue < StaminaCost) return false;
            bool didHit = UseToolInDirection(targetPosition, player, hitableLayers);
            if (!didHit) return false;

            player.RotateTowards(targetPosition);
            player.TriggerAnimation($"Use{ToolType}");
            InputWrapper.BlockPlayerInput();

            return true;
        }

        /// <summary> Использовать инструмент в заданном направлении. </summary>
        /// <param name="targetPosition"> Целевая позиция для взаимодействия. </param>
        /// <param name="player"> Контроллер игрока. </param>
        /// <param name="hitableLayers"> Слой объектов, с которыми можно взаимодействовать. </param>
        private bool UseToolInDirection(Vector3 targetPosition, PlayerController player, LayerMask hitableLayers)
        {
            var origin = player.transform.position;
            var direction = (targetPosition - origin).normalized;
            var interactionCenter = origin + direction * (MaxInteractionDistance / 2);

            //TODO: Тута бага. Получает удар только первый блжайшй. Плохой рейкаст сферой по ударяемым прколам!!!
            var hitColliders = Physics.OverlapSphere(interactionCenter, UseRadius, hitableLayers);
            foreach (var collider in hitColliders)
                if (collider.transform.parent.TryGetComponent<IHitable>(out var hitable))
                {
                    hitable.TakeHit(ToolType);
                    player.GetComponent<PlayerStats>().GetStat<Stamina>().ChangeValue(-StaminaCost);
                    return true;
                }

            return hitColliders.Length > 0;
        }
    }
}