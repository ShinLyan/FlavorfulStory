using System;
using System.Collections;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    public class DayEndManager : IInitializable, IDisposable
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
            WorldTime.OnDayEnded += OnDayEnded;
        }

        public void Initialize() { }

        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        private void OnDayEnded(DateTime date) => RequestEndDay(() => { });

        public void RequestEndDay(Action onCompleteCallback)
        {
            _onCompleteCallback = onCompleteCallback;
            _coroutineRunner.StartCoroutine(EndDayRoutine());
        }

        private IEnumerator EndDayRoutine()
        {
            WorldTime.Pause();

            yield return _fader.FadeOut(Fader.FadeOutTime);

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