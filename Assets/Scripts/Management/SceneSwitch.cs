using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitch : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!SceneSwitcher.alreadyLoading) {
            if (collision.gameObject.CompareTag("Player")) {
                if (name.Contains("right")) GameData.position = 0;
                if (name.Contains("bottom")) GameData.position = 1;
                if (name.Contains("left")) GameData.position = 2;
                if (name.Contains("top")) GameData.position = 3;
                
                GameData.level++;
                
                if (GameData.level % 10 == 1 && GameData.level>10 )
                {
                    if (GameData.currentWorld == GameData.max_number_world) GameData.currentWorld = 0;
                    else GameData.currentWorld++;
                    GameData.world = GameData.levelOrder[GameData.currentWorld];
                }
                GameData.predRoomType = GameData.roomType;
                if (name.Contains("Mob")) GameData.roomType = "MobRoom";
                if (name.Contains("merchant")) GameData.roomType = "MerchantRoom";
                if (name.Contains("Boss")) GameData.roomType = "BossRoom";
                SceneSwitcher.Singleton.StartCoroutine("AsyncSwitchScene");
            }
        }
    }


}
