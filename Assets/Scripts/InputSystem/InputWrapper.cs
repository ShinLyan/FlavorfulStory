using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FlavorfulStory.InputSystem
{
    /// <summary> Статический класс-обертка для работы с пользовательским вводом. </summary>
    public static class InputWrapper
    {
        /// <summary> Словарь для хранения состояний блокировки кнопок ввода. </summary>
        private static readonly Dictionary<InputButton, bool> _allowedButtons;

        /// <summary> Инициализирует словарь разрешенных кнопок ввода. </summary>
        static InputWrapper()
        {
            _allowedButtons = new Dictionary<InputButton, bool>();
            foreach (InputButton inputButton in Enum.GetValues(typeof(InputButton)))
                _allowedButtons.Add(inputButton, true);
        }

        /// <summary> Блокирует указанные кнопки ввода. </summary>
        /// <param name="buttonsToBlock"> Кнопки для блокировки. </param>
        public static void BlockInput(params InputButton[] buttonsToBlock)
        {
            foreach (var button in buttonsToBlock) _allowedButtons[button] = false;
        }

        /// <summary> Разблокирует указанные кнопки ввода. </summary>
        /// <param name="buttonsToUnblock"> Кнопки для разблокировки.</param>
        public static void UnblockInput(params InputButton[] buttonsToUnblock)
        {
            foreach (var button in buttonsToUnblock) _allowedButtons[button] = true;
        }

        /// <summary> Блокирует все кнопки ввода. </summary>
        public static void BlockAllInput()
        {
            foreach (var key in _allowedButtons.Keys.ToArray()) _allowedButtons[key] = false;
        }

        /// <summary> Разблокирует все кнопки ввода. </summary>
        public static void UnblockAllInput()
        {
            foreach (var key in _allowedButtons.Keys.ToArray()) _allowedButtons[key] = true;
        }

        /// <summary> Разблокирует указанные кнопки ввода на следующем кадре. </summary>
        /// <param name="buttonsToUnblock">Кнопки для разблокировки. </param>
        public static void UnblockInputNextFrame(params InputButton[] buttonsToUnblock) =>
            UnblockNextFrameAsync(buttonsToUnblock).Forget();

        /// <summary> Асинхронный метод разблокировки кнопок на следующем кадре. </summary>
        /// <param name="buttonsToUnblock">Кнопки для разблокировки. </param>
        private static async UniTaskVoid UnblockNextFrameAsync(InputButton[] buttonsToUnblock)
        {
            await UniTask.Yield(); // Ждем следующий кадр
            UnblockInput(buttonsToUnblock);
        }

        /// <summary> Проверяет отпускание кнопки в текущем кадре. </summary>
        /// <param name="button"> Проверяемая кнопка. </param>
        /// <returns> True, если кнопка была отпущена в текущем кадре. </returns>
        public static bool GetButtonUp(InputButton button) =>
            _allowedButtons[button] && Input.GetButtonUp(button.ToString());

        /// <summary> Проверяет нажатие кнопки в текущем кадре. </summary>
        /// <param name="button"> Проверяемая кнопка. </param>
        /// <returns> True, если кнопка была нажата в текущем кадре. </returns>
        public static bool GetButtonDown(InputButton button) =>
            _allowedButtons[button] && Input.GetButtonDown(button.ToString());

        /// <summary> Проверяет удержание кнопки. </summary>
        /// <param name="button"> Проверяемая кнопка. </param>
        /// <returns> True, если кнопка удерживается. </returns>
        public static bool GetButton(InputButton button) =>
            _allowedButtons[button] && Input.GetButton(button.ToString());

        /// <summary> Получает значение оси ввода. </summary>
        /// <param name="axis"> Проверяемая ось. </param>
        /// <returns> Значение оси в диапазоне от -1 до 1. </returns>
        public static float GetAxisRaw(InputButton axis) =>
            !_allowedButtons[axis] ? 0.0f : Input.GetAxisRaw(axis.ToString());

        /// <summary> Проверяет нажатие левой кнопки мыши в текущем кадре. </summary>
        /// <returns> True, если кнопка была нажата в текущем кадре. </returns>
        public static bool GetLeftMouseButton() =>
            _allowedButtons[InputButton.LeftMouse] && Input.GetMouseButton(0);

        /// <summary> Проверяет нажатие правой кнопки мыши в текущем кадре. </summary>
        /// <returns> True, если кнопка была нажата в текущем кадре. </returns>
        public static bool GetRightMouseButton() =>
            _allowedButtons[InputButton.RightMouse] && Input.GetMouseButton(1);

        /// <summary> Получает значение прокрутки колеса мыши. </summary>
        /// <returns> Целочисленное значение прокрутки колеса мыши. </returns>
        public static int GetMouseScrollDelta() =>
            !_allowedButtons[InputButton.MouseScroll] ? 0 : (int)Input.mouseScrollDelta.y;

        /// <summary> Получает текущую позицию курсора мыши. </summary>
        /// <returns> Вектор позиции курсора мыши. </returns>
        public static Vector3 GetMousePosition() =>
            !_allowedButtons[InputButton.MousePosition] ? Vector3.zero : Input.mousePosition;

        /// <summary> Заблокировать управление игрока (перемещение, скролл). </summary>
        public static void BlockPlayerInput()
        {
            BlockInput(InputButton.Horizontal, InputButton.Vertical);
            BlockInput(InputButton.MouseScroll);
        }

        /// <summary> Разблокировать управление игрока (перемещение, скролл). </summary>
        public static void UnblockPlayerInput()
        {
            UnblockInput(InputButton.Horizontal, InputButton.Vertical);
            UnblockInput(InputButton.MouseScroll);
        }
    }
}