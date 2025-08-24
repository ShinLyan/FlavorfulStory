using System;
using System.Collections.Generic;
using DG.Tweening;
using Zenject;
using FlavorfulStory.InputSystem;
using FlavorfulStory.UI.Animation;

namespace FlavorfulStory
{
    public sealed class UIOverlayFadeCoordinator : IInitializable, IDisposable, IWindowOpenGate
    {
        private readonly IWindowService _windowService;
        private readonly CanvasGroupFader _hudFader;
        private readonly CanvasGroupFader _backgroundFader;
        private readonly OverlayFadeSettings _settings;

        private int _openWindows;
        private bool _preparedForFirstOpen;
        private readonly List<Action> _queuedOpens = new();

        [Inject]
        public UIOverlayFadeCoordinator(
            IWindowService windowService,
            [Inject(Id = "HUD")] CanvasGroupFader hudFader,
            [Inject(Id = "UIBackground")] CanvasGroupFader backgroundFader,
            OverlayFadeSettings settings)
        {
            _windowService   = windowService;
            _hudFader        = hudFader;
            _backgroundFader = backgroundFader;
            _settings        = settings;
        }

        public void Initialize()
        {
            _hudFader.SetState(_settings.HudMaxAlpha, true, true);
            _backgroundFader.SetState(0f, false, false);

            _windowService.OnWindowOpened += HandleWindowOpened;
            _windowService.OnWindowClosed += HandleWindowClosed;
        }

        public void Dispose()
        {
            _windowService.OnWindowOpened -= HandleWindowOpened;
            _windowService.OnWindowClosed -= HandleWindowClosed;
        }
        
        private void HandleWindowOpened(BaseWindow _)
        {
            var wasZero = _openWindows == 0;
            _openWindows++;

            if (wasZero)
            {
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
        }

        private void HandleWindowClosed(BaseWindow _)
        {
            if (_openWindows == 0) return;
            _openWindows--;

            if (_openWindows == 0)
            {
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
                sequence.OnComplete(() =>
                {
                    InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
                });
                sequence.OnKill(() =>
                {
                    if (sequence.active) return;
                    InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
                });
            }
        }

        public void RequestOpen(BaseWindow window, Action openAction)
        {
            if (_openWindows > 0)
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