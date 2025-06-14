using System;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Структура, описывающая действие игрока по отношению к объекту. </summary>
    [Serializable]
    public struct ActionDescription
    { 
        /// <summary> Название сущности, с которой происходит взаимодействие. </summary>
        [Tooltip("Название объекта, с которым происходит взаимодействие.")]
        public string Target;
        
        /// <summary> Название действия, что может совершить игрок. </summary>
        [Tooltip("Название действия, совершаемого игроком.")]
        public string Action;
    }
}