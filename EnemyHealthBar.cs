using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Canvas canvas;
    private Slider slider;
    private Gradient gradient;
    private Image fill;

    void Awake(){
        slider = canvas.GetComponentInChildren<Slider>();
        gradient = canvas.GetComponentInChildren<Gradient>();
        fill = canvas.GetComponentInChildren<Image>();
    }
    
    public void SetMaxHealth(int health){
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health){
        slider.value = health;
        fill.color = gradient.Evaluate(health/slider.maxValue);
    }
}