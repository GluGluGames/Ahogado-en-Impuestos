using System.Timers;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception that returns false until a determined amount of time passes.
    /// </summary>
    public class TimerPerception : Perception
    {
        /// <summary>
        /// The amount of time that must pass after the perception initializes to return true.
        /// </summary>
        public float Time;

        Timer _timer;

        bool _isTimeout;

        /// <summary>
        /// Create a new timer perception with an specified time.
        /// </summary>
        /// <param name="time">The time value.</param>
        public TimerPerception(float time)
        {
            Time = time;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Initialize the internal timer.
        /// </summary>
        public override void Initialize()
        {
            _isTimeout = false;
            _timer = new Timer(Time * 1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Enabled = true;
            _timer.Start();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Returns true if the timer's time has elapsed.</returns>
        public override bool Check()
        {
            return _isTimeout;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Dispose the timer.
        /// </summary>
        public override void Reset()
        {
            _isTimeout = false;
            _timer?.Dispose();
            _timer = null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the timer.
        /// </summary>
        public override void Pause()
        {
            _timer?.Stop();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Resumes the timer.
        /// </summary>
        public override void Unpause()
        {
            _timer?.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs evt)
        {
            _isTimeout = true;
        }
    }
}
