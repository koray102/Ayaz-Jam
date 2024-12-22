using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Cooldown : MonoBehaviour
{
    public List<Image> cooldownOverlays; // List of radial fill images for each skill
    public List<float> cooldownDurations; // List of cooldown durations for each skill
    private List<bool> isOnCooldown; // List of cooldown states for each skill
    private List<float> cooldownTimers; // List of cooldown timers for each skill

    void Start()
    {
        // Initialize the lists
        isOnCooldown = new List<bool>();
        cooldownTimers = new List<float>();

        foreach (var overlay in cooldownOverlays)
        {
            isOnCooldown.Add(false);
            cooldownTimers.Add(0);
        }
    }

    void Update()
    {
        for (int i = 0; i < cooldownOverlays.Count; i++)
        {
            if (isOnCooldown[i])
            {
                // Update the cooldown timer
                cooldownTimers[i] -= Time.deltaTime;

                // Calculate the fill amount (1 = full shade, 0 = no shade)
                cooldownOverlays[i].fillAmount = cooldownTimers[i] / cooldownDurations[i];

                // End cooldown when timer reaches zero
                if (cooldownTimers[i] <= 0)
                {
                    cooldownTimers[i] = 0;
                    isOnCooldown[i] = false;
                    cooldownOverlays[i].fillAmount = 0; // Ensure fully clear
                }
            }
        }

        // Example: Trigger cooldown for testing (replace with actual skill logic)
        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown[0]) // For Skill 1
        {
            StartCooldown(0);
        }
        if (Input.GetMouseButtonDown(0)&& !isOnCooldown[1]) // For Skill 2
        {
            StartCooldown(1);
        }
        // Add more keys for additional skills if needed
    }

    public void StartCooldown(int skillIndex)
    {
        if (!isOnCooldown[skillIndex])
        {
            isOnCooldown[skillIndex] = true;
            cooldownTimers[skillIndex] = cooldownDurations[skillIndex];
            cooldownOverlays[skillIndex].fillAmount = 1; // Fully shaded
        }
    }
}
