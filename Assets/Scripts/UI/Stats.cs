using UnityEngine;
using System;

public class Stats {

    public static float gameStartTime;
    public static float gameStopTime;
    public static int killsCount;


    public static void Init() {
        gameStartTime = Time.time;
        killsCount = 0;
    }

    public static void StopTimer() {
        gameStopTime = Time.time;
    }

    public static string GetGameTime() {
        return TimeSpan.FromSeconds(gameStopTime - gameStartTime).ToString(@"hh\:mm\:ss");
    }
}
