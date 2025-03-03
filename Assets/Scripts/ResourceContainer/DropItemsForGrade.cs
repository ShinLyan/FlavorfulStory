using System;
using System.Collections.Generic;
using FlavorfulStory.Actions;
using UnityEngine;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Набор предметов для определенного грейда. </summary>
    /// <remarks> Используется в <see cref="DestroyableResourceContainer" />. </remarks>
    [Serializable]
    internal struct DropItemsForGrade
    {
        /// <summary> Список предметов. </summary>
        [Tooltip("Список предметов этого грейда")]
        public List<DropItem> Items;
    }
}