using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlavorfulStory.InputSystem
{
    /// <summary> Статический класс-обертка для работы с пользовательским вводом. </summary>
    public static class InputWrapper
    {
        /// <summary> Словарь для хранения состояний блокировки кнопок ввода. </summary>
        private static readonly Dictionary<InputButton, bool> _allowedButtons = new();

        /// <summary> Флаг инициализации класса. </summary>
        private static bool _isInitialized;

        /// <summary> Инициализирует словарь разрешенных кнопок ввода. </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            foreach (InputButton inputButton in Enum.GetValues(typeof(InputButton)))
            {
                _allowedButtons.Add(inputButton, true);
            }

            _isInitialized = true;
        }

        /// <summary> Блокирует указанную кнопку ввода. </summary>
        /// <param name="inputToBlock"> Кнопка для блокировки. </param>
        public static void BlockInput(InputButton inputToBlock) => _allowedButtons[inputToBlock] = false;

        /// <summary> Блокирует указанные кнопки ввода. </summary>
        ///<param name="inputToBlock"> Коллекция кнопок для блокировки. </param>
        public static void BlockInput(IEnumerable<InputButton> inputToBlock) =>
            inputToBlock.ToList().ForEach(BlockInput);

        /// <summary> Блокирует все кнопки ввода. </summary>
        public static void BlockAllInput() =>
            _allowedButtons.Keys.ToList().ForEach(BlockInput);

        /// <summary> Разблокирует указанную кнопку ввода. </summary>
        /// <param name="inputToUnlock"> Кнопка для разблокировки. </param>
        public static void UnblockInput(InputButton inputToUnlock) => _allowedButtons[inputToUnlock] = true;

        /// <summary> Разблокирует указанные кнопки ввода. </summary>
        ///<param name="inputToUnlock"> Коллекция кнопок для разблокировки. </param>
        public static void UnblockInput(IEnumerable<InputButton> inputToUnlock) =>
            inputToUnlock.ToList().ForEach(UnblockInput);

        /// <summary> Разблокирует все кнопки ввода. </summary>
        public static void UnblockAllInput() =>
            _allowedButtons.Keys.ToList().ForEach(UnblockInput);

        /// <summary> Проверяет отпускание кнопки в текущем кадре. </summary>
        ///<param name="button"> Проверяемая кнопка. </param>
        ///<returns> True, если кнопка была отпущена в текущем кадре. </returns>
        public static bool GetButtonUp(InputButton button) =>
            _allowedButtons[button] && Input.GetButtonUp(button.ToString());

        /// <summary> Проверяет нажатие кнопки в текущем кадре. </summary>
        ///<param name="button"> Проверяемая кнопка. </param>
        ///<returns> True, если кнопка была нажата в текущем кадре. </returns>
        public static bool GetButtonDown(InputButton button) =>
            _allowedButtons[button] && Input.GetButtonDown(button.ToString());

        /// <summary> Проверяет удержание кнопки. </summary>
        ///<param name="button"> Проверяемая кнопка. </param>
        ///<returns> True, если кнопка удерживается. </returns>
        public static bool GetButton(InputButton button) =>
            _allowedButtons[button] && Input.GetButton(button.ToString());

        /// <summary> Получает значение оси ввода. </summary>
        ///<param name="axis"> Проверяемая ось. </param>
        ///<returns> Значение оси в диапазоне от -1 до 1. </returns>
        public static float GetAxisRaw(InputButton axis) =>
            !_allowedButtons[axis] ? 0.0f : Input.GetAxisRaw(axis.ToString());

        /// <summary> Получает значение прокрутки колеса мыши. </summary>
        ///<returns> Целочисленное значение прокрутки колеса мыши. </returns>
        public static int GetMouseScrollDelta() =>
            !_allowedButtons[InputButton.MouseScroll] ? 0 : (int)Input.mouseScrollDelta.y;

        /// <summary> Получает текущую позицию курсора мыши. </summary>
        ///<returns> Вектор позиции курсора мыши. </returns>
        public static Vector3 GetMousePosition() =>
            !_allowedButtons[InputButton.MousePosition] ? Vector3.zero : Input.mousePosition;

        public static void BlockPlayerMovement() =>
            BlockInput(new[] { InputButton.Horizontal, InputButton.Vertical });

        public static void UnblockPlayerMovement() =>
            UnblockInput(new[] { InputButton.Horizontal, InputButton.Vertical });
    }
}