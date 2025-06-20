using System;
using System.Collections;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    public class DayEndManager : IInitializable
    {
        private readonly SummaryView _summaryView;
        private readonly Fader _fader;
        private Action _onCompleteCallback;
        private readonly MonoBehaviour _coroutineRunner;

        public DayEndManager(SummaryView summaryView,
            Fader fader,
            MonoBehaviour coroutineRunner)
        {
            _summaryView = summaryView;
            _fader = fader;
            _coroutineRunner = coroutineRunner;
        }

        public void Initialize() { }

        public void RequestEndDay(Action onCompleteCallback)
        {
            _onCompleteCallback = onCompleteCallback;
            _coroutineRunner.StartCoroutine(EndDayRoutine());
        }

        private IEnumerator EndDayRoutine()
        {
            yield return _fader.FadeOut(Fader.FadeOutTime);

            WorldTime.Pause();

            _summaryView.Show();
            bool continuePressed = false;
            _summaryView.OnContinuePressed = () => continuePressed = true;
            _summaryView.SetSummary(SummaryView.DefaultSummaryText);
            yield return new WaitUntil(() => continuePressed);

            _onCompleteCallback?.Invoke();

            _summaryView.Hide();
            WorldTime.Unpause();

            yield return _fader.FadeIn(Fader.FadeInTime);
        }
    }
}