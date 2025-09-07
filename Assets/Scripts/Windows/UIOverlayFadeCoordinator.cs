using System;
using System.Collections.Generic;
using DG.Tweening;
using FlavorfulStory.InputSystem;
using FlavorfulStory.UI.Animation;
using Zenject;

namespace FlavorfulStory.Windows
{
    /// <summary> Координатор фейдов HUD и фонового затемнения при открытии и закрытии UI-окон. </summary>
    /// <remarks> Управляет порядком открытия окон через <see cref="IWindowOpenGate"/>. </remarks>
    public class UIOverlayFadeCoordinator : IInitializable, IDisposable, IWindowOpenGate
    {
        /// <summary> Сервис окон. </summary>
        private readonly IWindowService _windowService;

        /// <summary> Аниматор HUD-панели. </summary>
        private readonly CanvasGroupFader _hudFader;

        /// <summary> Аниматор затемняющего фона. </summary>
        private readonly CanvasGroupFader _backgroundFader;

        /// <summary> Настройки анимаций. </summary>
        private readonly OverlayFadeSettings _settings;

        /// <summary> Кол-во открытых окон. </summary>
        private int _openedWindows;

        /// <summary> Флаг, указывающий, что уже запущена подготовка к первому открытию. </summary>
        private bool _preparedForFirstOpen;

        /// <summary> Очередь отложенных open-запросов. </summary>
        private readonly List<Action> _queuedOpens = new();

        /// <summary> Конструктор с внедрением зависимостей. </summary>
        [Inject]
        public UIOverlayFadeCoordinator(IWindowService windowService, [Inject(Id = "HUD")] CanvasGroupFader hudFader,
            [Inject(Id = "UIBackground")] CanvasGroupFader backgroundFader, OverlayFadeSettings settings)
        {
            _windowService = windowService;
            _hudFader = hudFader;
            _backgroundFader = backgroundFader;
            _settings = settings;
        }

        /// <summary> Инициализация координатора: сбрасывает состояния и подписывается на события. </summary>
        public void Initialize()
        {
            _hudFader.SetState(_settings.HudMaxAlpha, true);
            _backgroundFader.SetState(0f, false);

            _windowService.WindowOpened += HandleWindowOpened;
            _windowService.WindowClosed += HandleWindowClosed;
        }

        /// <summary> Очистка подписок. </summary>
        public void Dispose()
        {
            _windowService.WindowOpened -= HandleWindowOpened;
            _windowService.WindowClosed -= HandleWindowClosed;
        }

        /// <summary> Обработка события открытия окна. </summary>
        private void HandleWindowOpened()
        {
            bool wasZero = _openedWindows == 0;
            _openedWindows++;

            if (!wasZero) return;

            if (_preparedForFirstOpen)
            {
                _preparedForFirstOpen = false;
                return;
            }

            _hudFader.Hide(_settings.HudFadeOutDuration, _settings.HudEase);
            _backgroundFader.FadeTo(
                _settings.BackgroundMaxAlpha, false, _settings.BackgroundBlocksRaycasts,
                _settings.BackgroundFadeInDuration, _settings.BackgroundEase);
        }

        /// <summary> Обработка события закрытия окна. </summary>
        private void HandleWindowClosed()
        {
            if (_openedWindows == 0) return;

            _openedWindows--;

            if (_openedWindows != 0) return;

            _preparedForFirstOpen = false;
            _queuedOpens.Clear();

            InputWrapper.BlockInput(InputButton.SwitchGameMenu);

            var sequence = DOTween.Sequence();
            sequence.Join(_backgroundFader.FadeTo(
                0f, false, false,
                _settings.BackgroundFadeOutDuration, _settings.BackgroundEase
            ));
            sequence.Join(_hudFader.Show(
                _settings.HudFadeInDuration, _settings.HudMaxAlpha, _settings.HudEase
            ));
            sequence.OnComplete(() => { InputWrapper.UnblockInput(InputButton.SwitchGameMenu); });
            sequence.OnKill(() =>
            {
                if (sequence.active) return;
                InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
            });
        }

        /// <summary> Запросить открытие окна: либо сразу, либо после фейдов (если это первое окно). </summary>
        /// <param name="openAction"> Действие при открытии. </param>
        public void RequestOpen(Action openAction)
        {
            if (_openedWindows > 0)
            {
                openAction?.Invoke();
                return;
            }

            if (_preparedForFirstOpen)
            {
                _queuedOpens.Add(openAction);
                return;
            }

            _preparedForFirstOpen = true;
            _queuedOpens.Add(openAction);

            InputWrapper.BlockInput(InputButton.SwitchGameMenu);

            var sequence = DOTween.Sequence();
            sequence.Join(_hudFader.Hide(_settings.HudFadeOutDuration, _settings.HudEase));
            sequence.Join(_backgroundFader.FadeTo(
                _settings.BackgroundMaxAlpha, false, _settings.BackgroundBlocksRaycasts,
                _settings.BackgroundFadeInDuration, _settings.BackgroundEase));

            sequence.OnComplete(() =>
            {
                var actions = _queuedOpens.ToArray();
                _queuedOpens.Clear();

                foreach (var action in actions) action?.Invoke();

                InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
            });

            sequence.OnKill(() =>
            {
                if (sequence.active) return;
                InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
            });
        }
    }
}