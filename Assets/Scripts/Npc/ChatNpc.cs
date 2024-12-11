using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    /// <summary>Класс описывающий логику NPC с диалогами .</summary>
    public class ChatNpc : Npc, INpcIteractive
    {
        /// <summary>Словарь с фразами .</summary>
        Dictionary<string, string> phrases;

        /// <summary>Метод для получения фразы по ключу  .</summary>
        /// <param name="phraseKey"> . Ключ нужной фразы</param>
        public string GetPhrase(string phraseKey)
        {
            if (phrases.ContainsKey(phraseKey))
                return phrases[phraseKey];
            return "";
        }

        /// <summary>Метод для запуска взаимодействия с NPC. Реализация интерфейса IIteractiveNpc .</summary>
        public void Interact()
        {
            //Debug.Log("Hi, I'm " + this.Name);
        }
    }
}