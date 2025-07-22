using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory.Utils
{
    /// <summary> Вспомогательный класс для загрузки аудио-ресурсов из Resources. </summary>
    public static class ResourcesLoader
    {
        /// <summary> Загружает все SfxData из папки Resources. </summary>
        /// <returns> Список SfxData из папки Resources. </returns>
        public static List<SfxData> LoadAllSfxData() =>
            Resources.LoadAll<SfxData>(string.Empty).Where(data => data).ToList();
    }
}