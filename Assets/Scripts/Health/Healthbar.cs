using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image currenthealthBar;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Color Low;
    [SerializeField] private Color High;
    private float health;
    [SerializeField] public Text healthText;

    private void Start()
    {

        
        currenthealthBar.fillAmount = playerHealth.currentHealth/200f;
       //totalhealthBar.fillAmount = playerHealth.currentHealth;
        //playerHealth.currentHealth = health;
       
    }

    private void Update()
    {
        
        
        currenthealthBar.fillAmount = playerHealth.currentHealth/200f;
   }

    public void Setup (float health)
    {
        healthText.text = health.ToString() + " / 200";
    }
}
