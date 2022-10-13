using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public GameManager gameInfo;
    private int level;
    private TMPro.TextMeshProUGUI text;

    //This is a bit ridiculous to update this every frame but it shouldn't imapact performance for this game
    void Update()
    {
        level = gameInfo.numLevel;
        text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        text.SetText("Level: " + level.ToString());
    }
}
