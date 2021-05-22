using UnityEngine;
using TMPro;

public class ControlsUiUpdater : MonoBehaviour {

    private TextMeshProUGUI p1Up;
    private TextMeshProUGUI p1Down;
    private TextMeshProUGUI p1Left;
    private TextMeshProUGUI p1Right;

    private TextMeshProUGUI p1FireUp;
    private TextMeshProUGUI p1FireDown;
    private TextMeshProUGUI p1FireLeft;
    private TextMeshProUGUI p1FireRight;

    private TextMeshProUGUI p2Up;
    private TextMeshProUGUI p2Down;
    private TextMeshProUGUI p2Left;
    private TextMeshProUGUI p2Right;

    private TextMeshProUGUI p2FireUp;
    private TextMeshProUGUI p2FireDown;
    private TextMeshProUGUI p2FireLeft;
    private TextMeshProUGUI p2FireRight;


    private void Start() {
        Bind("P1 Controls", "Move", p1Up, p1Down, p1Left, p1Right);
        Bind("P1 Controls", "Fire", p1FireUp, p1FireDown, p1FireLeft, p1FireRight);
        Bind("P2 Controls", "Move", p2Up, p2Down, p2Left, p2Right);
        Bind("P2 Controls", "Fire", p2FireUp, p2FireDown, p2FireLeft, p2FireRight);

        if (!GameData.isMultiplayer) {
            transform.Find("P2 Controls").gameObject.SetActive(false);
        }
    }

    private void Bind(string parentName, string subName, TextMeshProUGUI childUp, TextMeshProUGUI childDown, TextMeshProUGUI childLeft, TextMeshProUGUI childRight) {
        Transform t = transform.Find(parentName).Find(subName);
        string textChild = "Text (TMP)";

        childUp = t.Find("Up").Find(textChild).GetComponent<TextMeshProUGUI>();
        childDown = t.Find("Down").Find(textChild).GetComponent<TextMeshProUGUI>();
        childLeft = t.Find("Left").Find(textChild).GetComponent<TextMeshProUGUI>();
        childRight = t.Find("Right").Find(textChild).GetComponent<TextMeshProUGUI>();
    }
}
