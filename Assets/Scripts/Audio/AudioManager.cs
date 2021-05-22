using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicTheme { None, MainMenu, Credits, NormalFloors, Merchant, Boss }
public enum SFX { MenuHover = 1, MenuSelect = 2, GameOver = 3,
    PlayerHealingPotion = 10, PlayerHit = 11, PlayerPickUpCoin = 12, ShieldBlock = 13,
    MobHit = 20, MobDie = 21,
    OrcBossSmash = 30,
    PowerUp = 40, Shoot = 41, OpenChest = 42, OpenGate = 43
}

/**
 * ### SFX ###
 * To play a SFX, use `FindObjectOfType<AudioManager>().Play(SFX.***);`
 * Replace *** by the type of the audio you want to play. You can find types above.
 * 
 * ### MUSICS ###
 * To change the background music, use `FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.***);`
 * Replace *** by one of the predefined themes, in the MusicTheme enum above.
 */
public class AudioManager : MonoBehaviour {

    public MusicTheme musicTheme;
    private MusicTheme previousUpdateMusicTheme = MusicTheme.None;
    private GameMusic currentMusic = null;

    [Range(1f, 10f)]
    public float musicFadeTime = 5f;
    [Range(0f, 1f)]
    public float globalVolume = 0.1f;
    private float musicVolume; // Set in the Settings Load() method
    private float sfxVolume; // Set in the Settings Load() method

    // Musics
    public GameMusic[] mainMenuMusics;
    public GameMusic[] normalRoomMusics;
    public GameMusic[] merchantRoomMusics;
    public GameMusic[] bossRoomMusics;
    private List<GameMusic> musics = new List<GameMusic>();

    // SFX
    public GameSFX[] menuSFX;
    public GameSFX[] playerSFX;
    public GameSFX[] mobSFX;
    public GameSFX[] bossSFX;
    public GameSFX[] weaponAndPowerUpSFX;
    public GameSFX[] ambianceSFX;
    private List<GameSFX> sfx = new List<GameSFX>();

    // Singleton pattern
    public static AudioManager instance;


    // Awake is called before Start. Initialize the singleton and the audio sources
    private void Awake() {

        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        Settings.Load();

        DontDestroyOnLoad(gameObject);

        // Musics
        InitAudioSources(mainMenuMusics, false);
        InitAudioSources(normalRoomMusics, false);
        InitAudioSources(merchantRoomMusics, false);
        InitAudioSources(bossRoomMusics, false);

        // SFX
        InitAudioSources(menuSFX, true);
        InitAudioSources(playerSFX, true);
        InitAudioSources(mobSFX, true);
        InitAudioSources(bossSFX, true);
        InitAudioSources(weaponAndPowerUpSFX, true);
        InitAudioSources(ambianceSFX, true);
    }

    // Start is called before the first frame update
    void Start() {
        UpdateMusic();
    }

    // Update is called once per frame
    void Update() {
        UpdateMusic();
    }

    // Add an AudioSource component to each of our sounds, and initialize it properly
    private void InitAudioSources(Sound[] sounds, bool isSFX) {
        foreach (Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            //sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;

            if (isSFX)
                sfx.Add((GameSFX) sound);
            else
                musics.Add((GameMusic) sound);
        }
    }

    // Change the music if the theme is different from the last update AudioManager's theme
    private void UpdateMusic() {
        if (currentMusic == null || previousUpdateMusicTheme != musicTheme) {
            if (musicTheme == MusicTheme.None)
                StopCurrentMusic();
            else
                SwitchToMusic(musicTheme);
        }
        if (currentMusic != null && currentMusic.source.isPlaying == false)
            currentMusic.source.Play(); // Force the music to play if there is one (bug fix)
    }

    private bool StopCurrentMusic() {
        // Fade out the current music
        if (currentMusic != null) {
            StartCoroutine(FadeAudio(currentMusic, false));
            currentMusic = null;
        }
        previousUpdateMusicTheme = MusicTheme.None;
        return true;
    }

    private void SwitchToMusic(MusicTheme type) {
        // Fade out the current music
        StopCurrentMusic();

        // Fade in the new music
        GameMusic newMusic = SelectRandomMusicOfType(type);
        if (newMusic != null) {
            StartCoroutine(FadeAudio(newMusic, true));
            currentMusic = newMusic;
            previousUpdateMusicTheme = musicTheme;
        }
    }

    private GameMusic SelectRandomMusicOfType(MusicTheme type) {
        List<GameMusic> themeMusics = musics.FindAll(s => s.type == type);
        if (themeMusics.Count == 0) {
            Debug.LogWarning("No music of type '" + type + "' found in the AudioManager!");
            return null;
        }
        int index = Random.Range(0, themeMusics.Count);
        return themeMusics[index];
    }

    private GameSFX SelectRandomSFXOfType(SFX type) {
        List<GameSFX> contextSfx = sfx.FindAll(s => s.type == type);
        if (contextSfx.Count == 0) {
            Debug.LogWarning("No SFX of type '" + type + "' found in the AudioManager!");
            return null;
        }
        int index = Random.Range(0, contextSfx.Count);
        //Debug.Log("SFX index: " + index + "/" + (contextSfx.Count - 1));
        return contextSfx[index];
    }

    private static IEnumerator FadeAudio(Sound music, bool fadeIn) {
        float currentTime = 0;

        float fromVolume, toVolume;
        if (fadeIn) { // Fade in
            music.source.Play(); // Start the music so it fades in
            fromVolume = 0f;
            toVolume = music.volume * instance.globalVolume * instance.musicVolume;
        }
        else { // Fade out
            fromVolume = music.volume * instance.globalVolume * instance.musicVolume;
            toVolume = 0f;
        }

        while (currentTime < instance.musicFadeTime) {
            currentTime += Time.deltaTime;
            music.source.volume = Mathf.Lerp(fromVolume, toVolume, currentTime / instance.musicFadeTime);
            yield return null;
        }

        if (!fadeIn) {
            music.source.Stop(); // Stop the music at the end of the fade out
        }

        yield break;
    }

    // Play a sound
    public void Play(SFX type) {
        GameSFX sfx = SelectRandomSFXOfType(type);
        if (sfx != null) {
            sfx.source.volume = sfx.volume * globalVolume * sfxVolume;
            sfx.source.Play();
        }
    }

    public void Play(SFX type, float volume) {
        GameSFX sfx = SelectRandomSFXOfType(type);
        if (sfx != null) {
            sfx.source.volume = volume * sfx.volume * globalVolume * sfxVolume;
            sfx.source.Play();
        }
    }

    // Change the background music
    public void SetMusicTheme(MusicTheme theme) {
        musicTheme = theme;
    }

    // Change the musics volume settings
    public void UpdateMusicsVolume(int volume) {
        musicVolume = (volume / 100f);
        if (currentMusic != null) {
            currentMusic.source.volume = currentMusic.volume * globalVolume * musicVolume;
        }
    }

    public void UpdateMusicsVolumeMuted(int volume) {
        musicVolume = (volume / 100f);
    }

    // Change the sounds volume settings
    public void UpdateSoundsVolume(int volume) {
        sfxVolume = (volume / 100f);
    }
}
