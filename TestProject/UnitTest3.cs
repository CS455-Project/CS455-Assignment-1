using NUnit.Framework;
using Moq;
using System;

namespace Audio
{
    public interface IAudioPlayer
    {
        void Play(string soundId);
    }
}

namespace GameLogic
{
    public class SoundPlayer
    {
        private readonly Audio.IAudioPlayer _audioPlayer;

        public SoundPlayer(Audio.IAudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        public void PlaySound1()
        {
            _audioPlayer.Play("hitSound");
        }

        public void PlaySound2()
        {
            _audioPlayer.Play("missSound");
        }
    }
}

namespace SoundPlayerTests
{
    [TestFixture]
    public class SoundPlayerTests
    {
        private Mock<Audio.IAudioPlayer>? _mockAudioPlayer;
        private GameLogic.SoundPlayer? _soundPlayer;

        [SetUp]
        public void Setup()
        {
            _mockAudioPlayer = new Mock<Audio.IAudioPlayer>();
            _soundPlayer = new GameLogic.SoundPlayer(_mockAudioPlayer.Object);
        }

        [Test]
        public void PlaySound1_ShouldPlayHitSound()
        {
            // Act
            _soundPlayer!.PlaySound1();

            // Assert
            _mockAudioPlayer!.Verify(x => x.Play("hitSound"), Times.Once);
        }

        [Test]
        public void PlaySound2_ShouldPlayMissSound()
        {
            // Act
            _soundPlayer!.PlaySound2();

            // Assert
            _mockAudioPlayer!.Verify(x => x.Play("missSound"), Times.Once);
        }

        [Test]
        public void PlaySound1_ShouldNotPlayMissSound()
        {
            // Act
            _soundPlayer!.PlaySound1();

            // Assert
            _mockAudioPlayer!.Verify(x => x.Play("missSound"), Times.Never);
        }

        [Test]
        public void PlaySound2_ShouldNotPlayHitSound()
        {
            // Act
            _soundPlayer!.PlaySound2();

            // Assert
            _mockAudioPlayer!.Verify(x => x.Play("hitSound"), Times.Never);
        }
    }
}
