using UnityEngine;

namespace FlavorfulStory.Input
{
    ///<summary> Инициализатор пользовательской системы ввода. </summary>
    public class CustomInputSystemInitializer : MonoBehaviour
    {
        ///<summary> Инициализирует систему ввода при создании объекта. </summary>
        private void Awake()
        {
            InputWrapper.Initialize();
        }
    }
}