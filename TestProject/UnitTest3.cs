using NUnit.Framework;
using Moq;
using System;

public interface IAudioPlayer
{
    void Play(string soundId);
}

public class SoundPlayer
{
    private readonly IAudioPlayer _audioPlayer;

    public SoundPlayer(IAudioPlayer audioPlayer)
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

[TestFixture]
public class SoundPlayerTests
{
    private Mock<IAudioPlayer> _mockAudioPlayer;
    private SoundPlayer _soundPlayer;

    [SetUp]
    public void Setup()
    {
        _mockAudioPlayer = new Mock<IAudioPlayer>();
        _soundPlayer = new SoundPlayer(_mockAudioPlayer.Object);
    }

    [Test]
    public void PlaySound1_ShouldPlayHitSound()
    {
        // Act
        _soundPlayer.PlaySound1();

        // Assert
        _mockAudioPlayer.Verify(x => x.Play("hitSound"), Times.Once);
    }

    [Test]
    public void PlaySound2_ShouldPlayMissSound()
    {
        // Act
        _soundPlayer.PlaySound2();

        // Assert
        _mockAudioPlayer.Verify(x => x.Play("missSound"), Times.Once);
    }

    [Test]
    public void PlaySound1_ShouldNotPlayMissSound()
    {
        // Act
        _soundPlayer.PlaySound1();

        // Assert
        _mockAudioPlayer.Verify(x => x.Play("missSound"), Times.Never);
    }

    [Test]
    public void PlaySound2_ShouldNotPlayHitSound()
    {
        // Act
        _soundPlayer.PlaySound2();

        // Assert
        _mockAudioPlayer.Verify(x => x.Play("hitSound"), Times.Never);
    }
}