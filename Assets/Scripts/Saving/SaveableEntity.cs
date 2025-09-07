using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.Saving
{
    /// <summary> Сохраняемый объект. </summary>
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour, ISaveable
    {
        /// <summary> GUID. </summary>
        [SerializeField] private string _uniqueIdentifier;

        /// <summary> GUID. </summary>
        public string UniqueIdentifier => _uniqueIdentifier;

        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public object CaptureState()
        {
            var state = new Dictionary<string, object>();
            foreach (var saveable in GetComponents<ISaveable>())
            {
                if (ReferenceEquals(saveable, this)) continue;

                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public void RestoreState(object state)
        {
            if (state is not Dictionary<string, object> stateDict) return;

            foreach (var saveable in GetComponents<ISaveable>())
            {
                if (ReferenceEquals(saveable, this)) continue;

                string typeString = saveable.GetType().ToString();
                if (stateDict.TryGetValue(typeString, out object value)) saveable.RestoreState(value);
            }
        }

        #region Setting GUID

#if UNITY_EDITOR

        /// <summary> База GUID всех сохраняемых объектов на сцене. </summary>
        private static readonly Dictionary<string, SaveableEntity> _entityLookup = new();

        /// <summary> Является ли объект префабом?</summary>
        /// <remarks> В префабах не должен выставляться GUID. </remarks>
        private bool IsPrefab => string.IsNullOrEmpty(gameObject.scene.path);

        /// <summary> Установка GUID. </summary>
        private void Update()
        {
            if (Application.IsPlaying(gameObject) || IsPrefab) return;

            SetUniqueIdentifier();
        }

        /// <summary> Выставление GUID сущности на сцене. </summary>
        private void SetUniqueIdentifier()
        {
            var serializedObject = new SerializedObject(this);
            var property = serializedObject.FindProperty(nameof(_uniqueIdentifier));
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            _entityLookup[property.stringValue] = this;
        }

        /// <summary> Является ли GUID уникальным? </summary>
        /// <param name="candidate"> Кандидат для проверки. </param>
        /// <returns> Возвращает True - если GUID является уникальным, False - в противном случае. </returns>
        private bool IsUnique(string candidate)
        {
            if (!_entityLookup.ContainsKey(candidate) || _entityLookup[candidate] == this) return true;

            if (_entityLookup[candidate] && _entityLookup[candidate].UniqueIdentifier == candidate) return false;

            _entityLookup.Remove(candidate);
            return true;
        }

#endif

        #endregion
    }
}