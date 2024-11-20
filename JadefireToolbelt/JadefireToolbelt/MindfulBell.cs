using System.Configuration;
using NAudio.Wave;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using Timer = System.Timers.Timer;

namespace JadefireToolbelt
{
    internal class MindfulBell
    {
        private const int ONE_MINUTE = 60000;
        private readonly Timer _timer;
        private readonly string _soundPath;
        private WaveOutEvent? _outputDevice;
        private AudioFileReader? _audioFile;

        public MindfulBell(double interval) 
        {
            var soundPathSetting = ConfigurationManager.AppSettings["MindfulBell_SoundPath"];
            var volumeSetting = ConfigurationManager.AppSettings["MindfulBell_Volume"];

            if (string.IsNullOrWhiteSpace(soundPathSetting))
            {
                throw new ArgumentNullException(nameof(soundPathSetting));
            }

            var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
            _soundPath = projectRoot == null
                ? throw new ArgumentNullException(nameof(projectRoot))
                : Path.Combine(projectRoot, soundPathSetting);

            _timer = new Timer { Interval = ONE_MINUTE * interval };
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Stop();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (_outputDevice == null)
                {
                    _outputDevice = new WaveOutEvent();
                    _outputDevice.PlaybackStopped += OnPlaybackStopped;
                }
                if (_audioFile == null)
                {
                    var volumeSetting = ConfigurationManager.AppSettings["MindfulBell_Volume"];
                    var volume = !string.IsNullOrWhiteSpace(volumeSetting) ? float.Parse(volumeSetting) : 0.05f;
                    _audioFile = new AudioFileReader(_soundPath) { Volume = volume };
                    _outputDevice.Init(_audioFile);
                }
                _outputDevice.Play();
            }
            catch (Exception ex)
            {
                throw new SoundPlayFailureException(ex.Message);
            }
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            _outputDevice?.Dispose();
            _outputDevice = null;
            _audioFile?.Dispose();
            _audioFile = null;
        }

        public void Start()
        { _timer.Start(); }

        public void Stop() 
        { _timer.Stop(); }
    }

    internal class SoundPlayFailureException : Exception
    {
        public SoundPlayFailureException() { }

        public SoundPlayFailureException(string message)
            : base(message) { }
    }
}
