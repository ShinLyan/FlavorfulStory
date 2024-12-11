using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    /// <summary>Базовый класс для NPC. Включает в себя логику UNITY AI NavMeshAgent</summary>
    public class Npc : MonoBehaviour
    {
        #region Fields
        /// <summary>Уникальное Имя для NPC.</summary>
        [SerializeField] private string _name;
        #endregion

        #region Properties
        /// <summary>Свойство для получения имени NPC.</summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}