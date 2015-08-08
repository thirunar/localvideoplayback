using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReUsableButton : MonoBehaviour {

    private Button button;
	// Use this for initialization
	void Start () {
        button = gameObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => WaitReuse());
        }
	}

    private void WaitReuse()
    {
        button.interactable = false;
        Invoke("ReEnable", 0.5f);
    }

    private void ReEnable()
    {
        button.interactable = true;
    }
}