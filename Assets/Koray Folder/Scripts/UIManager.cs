using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text zombieCoolDownText;
    [SerializeField] private TMP_Text comboCoolDownText;
    [SerializeField] private TMP_Text comboLevelText;
    [SerializeField] private TMP_Text pointText;
    internal float point;
    internal float zombieCoolDown;
    internal float comboCoolDown;
    internal int comboLevel;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        zombieCoolDownText.text = zombieCoolDown.ToString();
        comboCoolDownText.text = comboCoolDown.ToString();
        comboLevelText.text = comboLevel.ToString();
        pointText.text = point.ToString();
    }
}
