using System;
using System.Collections.Generic;
using FlavorfulStory.Actions;
using UnityEngine;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Набор предметов для определенного грейда. </summary>
    /// <remarks> Используется в <see cref="DestroyableResourceContainer" />. </remarks>
    [Serializable]
    public struct DropItemsForGrade
    {
        /// <summary> Список предметов. </summary>
        [field: Tooltip("Список предметов этого грейда"), SerializeField]
        public List<DropItem> Items { get; private set; }
    }
}