using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public interface ILocalizationUsageFinder
    {
        /// <summary> Получить список объектов, в которых используется данный ключ. </summary>
        List<Object> FindUsages(string key);
    }
}