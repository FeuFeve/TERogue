using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum Quality { Low, Medium, High };

// Initialized (loaded) in the AudioManager Awake() method
[System.Serializable]
public class Settings {

    private static string filePath = Application.persistentDataPath + "/settings.bin";

    public Quality quality = Quality.High;
    public int musicsVolume = 50;
    public int soundsVolume = 50;

    // Singleton pattern
    public static Settings instance;


    private Settings() { }

    public static void Load() {
        if (instance == null) {
            if (File.Exists(filePath)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(filePath, FileMode.Open);
                instance = formatter.Deserialize(stream) as Settings;
                stream.Close();
            }
            else { // When first launching the game, create the file default settings
                instance = new Settings();
                Save();
            }
            Update();
        }
    }

    private static void Save() {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filePath, FileMode.Create);
        formatter.Serialize(stream, instance);
        stream.Close();
    }

    private static void Update() {
        QualitySettings.SetQualityLevel((int) instance.quality);
        AudioManager.instance.UpdateMusicsVolumeMuted(instance.musicsVolume);
        AudioManager.instance.UpdateSoundsVolume(instance.soundsVolume);
    }

    public void IncreaseQuality() {
        quality++;
        if (quality > Quality.High)
            quality = Quality.Low;

        // Apply the changes to the Unity built-in QualitySettings of the game
        QualitySettings.SetQualityLevel((int) quality);
        Save();
    }

    public void DecreaseQuality() {
        quality--;
        if (quality < Quality.Low)
            quality = Quality.High;

        // Apply the changes to the Unity built-in QualitySettings of the game
        QualitySettings.SetQualityLevel((int) quality);
        Save();
    }

    public void IncreaseMusicsVolume() {
        musicsVolume += 10;
        if (musicsVolume > 100)
            musicsVolume = 0;

        // Apply the changes to the AudioManager
        AudioManager.instance.UpdateMusicsVolume(musicsVolume);
        Save();
    }

    public void DecreaseMusicsVolume() {
        musicsVolume -= 10;
        if (musicsVolume < 0)
            musicsVolume = 100;

        // Apply the changes to the AudioManager
        AudioManager.instance.UpdateMusicsVolume(musicsVolume);
        Save();
    }

    public void IncreaseSoundsVolume() {
        soundsVolume += 10;
        if (soundsVolume > 100)
            soundsVolume = 0;

        // Apply the changes to the AudioManager
        AudioManager.instance.UpdateSoundsVolume(soundsVolume);
        Save();
    }

    public void DecreaseSoundsVolume() {
        soundsVolume -= 10;
        if (soundsVolume < 0)
            soundsVolume = 100;

        // Apply the changes to the AudioManager
        AudioManager.instance.UpdateSoundsVolume(soundsVolume);
        Save();
    }
}
