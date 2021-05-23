using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoPrinter : MonoBehaviour {
    public static InfoPrinter Singleton = null;
    public Queue<string> titles = new Queue<string>();
    public Queue<string> descriptions = new Queue<string>();

    private TextMeshProUGUI titleGUI;
    private TextMeshProUGUI descriptionGUI;

    // Start is called before the first frame update
    void Start () {
        if (Singleton == null) {
            Singleton = this;
            titleGUI = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            descriptionGUI = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            StartCoroutine("PrintCoroutine");
        }
    }

    public void Print (string title, string description) {
        if (title.Equals("")) return;

        titles.Enqueue(title);
        descriptions.Enqueue(description);
    }

    IEnumerator PrintCoroutine () {
        while (true) {
            if (titles.Count != 0) {
                titleGUI.text = titles.Dequeue();
                descriptionGUI.text = descriptions.Dequeue();

                yield return new WaitForSeconds(3);
                titleGUI.text = "";
                descriptionGUI.text = "";
                yield return new WaitForSeconds(0.5f);
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
