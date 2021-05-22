using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public Animator animator;
    private Dictionary<int, string> gridOpen = new Dictionary<int, string>();
    
    private bool opened = false;
    private static string closed;


    private void Start() {
        gridOpen.Add(0, "left");
        gridOpen.Add(1, "top");
        gridOpen.Add(2, "right");
        gridOpen.Add(3, "bot");
        
        closed = gridOpen[GameData.position];
    }

    private void Update() {
        if (opened || gameObject.name.Contains(closed))
            return;

        if (GameData.enemies.Count == 0 && !gameObject.name.Contains(gridOpen[GameData.position]) && !GameData.roomType.Equals("MerchantRoom")) {
            animator.SetTrigger("Door");
            opened = true;

            PlayGateSound();
        }
    }

    private void PlayGateSound() {
        AudioManager.instance.Play(SFX.OpenGate);
    }
}
