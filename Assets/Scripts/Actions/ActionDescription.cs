using System;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Действие игрока по отношению к объекту. </summary>
    [Serializable]
    public struct ActionDescription
    {
        /// <summary> Название действия, что может совершить игрок. </summary>
        [field: Tooltip("Название действия, совершаемого игроком."), SerializeField]
        public string Action { get; internal set; }

        /// <summary> Название сущности, с которой происходит взаимодействие. </summary>
        [field: Tooltip("Название объекта, с которым происходит взаимодействие."), SerializeField]
        public string Target { get; internal set; }
    }
}