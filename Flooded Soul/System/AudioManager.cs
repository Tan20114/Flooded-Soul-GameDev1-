using System;
using System.Collections.Generic;
using Flooded_Soul;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

public class AudioManager : IDisposable
{
    public static AudioManager Instance { get; } = new AudioManager();

    private readonly Dictionary<string, Song> _bgm = new Dictionary<string, Song>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, SoundEffect> _sfx = new Dictionary<string, SoundEffect>(StringComparer.OrdinalIgnoreCase);

    private float _bgmVolume = 1f;
    private float _sfxVolume = 1f;

    public float BgmVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = MathHelper.Clamp(value, 0f, 1f);
            MediaPlayer.Volume = _bgmVolume * (_isMuted ? 0f : 1f);
        }
    }

    public float SfxVolume
    {
        get => _sfxVolume;
        set => _sfxVolume = MathHelper.Clamp(value, 0f, 1f);
    }

    private bool _isMuted = false;
    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            MediaPlayer.Volume = _isMuted ? 0f : _bgmVolume;
        }
    }

    private string _currentBgmKey = null;
    private Song _currentSong = null;

    private bool _isFading = false;
    private float _fadeTarget = 0f;
    private float _fadeSpeed = 0f;
    private Action _onFadeComplete = null;


    private AudioManager() { }

    public void LoadBgm(string key, string contentPath)
    {
        Song song = Game1.instance.Content.Load<Song>(contentPath);
        _bgm[key] = song;
    }

    public void LoadSfx(string key, string contentPath)
    {
        SoundEffect sfx = Game1.instance.Content.Load<SoundEffect>(contentPath);
        _sfx[key] = sfx;
    }

    public void RegisterBgm(string key, Song song)
    {
        _bgm[key] = song ?? throw new ArgumentNullException(nameof(song));
    }

    public void RegisterSfx(string key, SoundEffect sfx)
    {
        _sfx[key] = sfx ?? throw new ArgumentNullException(nameof(sfx));
    }

    public void PlayBgm(string key, bool loop = true, float? startVolume = null)
    {
        if (!_bgm.TryGetValue(key, out var song))
            throw new KeyNotFoundException($"BGM key not found: {key}");

        _currentBgmKey = key;
        _currentSong = song;

        MediaPlayer.IsRepeating = loop;
        if (startVolume.HasValue)
        {
            BgmVolume = MathHelper.Clamp(startVolume.Value, 0f, 1f);
        }
        MediaPlayer.Volume = _isMuted ? 0f : _bgmVolume;
        MediaPlayer.Play(song);
    }

    public void StopBgm()
    {
        MediaPlayer.Stop();
        _currentBgmKey = null;
        _currentSong = null;
        CancelFade();
    }

    public void PauseBgm() => MediaPlayer.Pause();

    public void ResumeBgm() => MediaPlayer.Resume();

    public void FadeToBgm(string newKey, float seconds, bool loop = true)
    {
        if (seconds <= 0f)
        {
            if (newKey == null) StopBgm();
            else PlayBgm(newKey, loop);
            return;
        }

        if (newKey == null)
        {
            StartFade(0f, seconds, () => StopBgm());
            return;
        }

        if (!_bgm.ContainsKey(newKey)) throw new KeyNotFoundException($"BGM key not found: {newKey}");

        StartFade(0f, seconds / 2f, () =>
        {
            StopBgm();
            float finalVolume = _bgmVolume;
            PlayBgm(newKey, loop, startVolume: 0f);
            StartFade(IsMuted ? 0f : 1f, seconds / 2f, null);
        });
    }

    public SoundEffectInstance PlaySfx(string key, float? volume = null, float pitch = 0f, float pan = 0f)
    {
        if (!_sfx.TryGetValue(key, out var sfx))
            throw new KeyNotFoundException($"SFX key not found: {key}");

        if (_isMuted || Math.Abs(_sfxVolume) < 0.0001f) return null;

        var inst = sfx.CreateInstance();
        inst.Volume = MathHelper.Clamp((volume ?? 1f) * _sfxVolume, 0f, 1f);
        inst.Pitch = MathHelper.Clamp(pitch, -1f, 1f);
        inst.Pan = MathHelper.Clamp(pan, -1f, 1f);
        inst.Play();
        return inst;
    }

    public void StartFade(float targetVolume, float durationSeconds, Action onComplete = null)
    {
        _isFading = true;
        _fadeTarget = MathHelper.Clamp(targetVolume, 0f, 1f);
        var start = MediaPlayer.Volume;
        _fadeSpeed = (_fadeTarget - start) / Math.Max(0.0001f, durationSeconds);
        _onFadeComplete = onComplete;
    }

    private void CancelFade()
    {
        _isFading = false;
        _fadeSpeed = 0f;
        _onFadeComplete = null;
    }

    public void Update(GameTime gameTime)
    {
        if (!_isFading) return;
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var newVol = MediaPlayer.Volume + _fadeSpeed * dt;
        var reached = (_fadeSpeed >= 0f && newVol >= _fadeTarget) || (_fadeSpeed < 0f && newVol <= _fadeTarget);
        MediaPlayer.Volume = MathHelper.Clamp(newVol, 0f, 1f);

        if (reached)
        {
            MediaPlayer.Volume = _fadeTarget;
            _isFading = false;
            var cb = _onFadeComplete;
            _onFadeComplete = null;
            cb?.Invoke();
        }
    }
    public void Dispose()
    {
        try { MediaPlayer.Stop(); } catch { }
        _bgm.Clear();
        _sfx.Clear();
    }
    public bool HasBgm(string key) => _bgm.ContainsKey(key);
    public bool HasSfx(string key) => _sfx.ContainsKey(key);
}
