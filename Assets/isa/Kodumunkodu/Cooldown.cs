using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Cooldown : MonoBehaviour
{
    public List<Image> cooldownOverlays; // Skill cooldown UI'ları
    public List<bool> isOnCooldown; // Skill cooldown durumları
    private List<float> cooldownTimers; // Mevcut cooldown süreleri
    private List<float> cooldownDurations; // Başlangıç cooldown süreleri

    void Start()
    {
        // Tüm listeleri başlat
        isOnCooldown = new List<bool>();
        cooldownTimers = new List<float>();
        cooldownDurations = new List<float>();

        foreach (var overlay in cooldownOverlays)
        {
            isOnCooldown.Add(false);
            cooldownTimers.Add(0);
            cooldownDurations.Add(0); // Varsayılan süre 0 olarak başlar
        }
    }

    void Update()
    {
        for (int i = 0; i < cooldownOverlays.Count; i++)
        {
            if (isOnCooldown[i])
            {
                // Cooldown süresini düşür
                cooldownTimers[i] -= Time.deltaTime;

                // UI için doluluk oranını ayarla
                cooldownOverlays[i].fillAmount = cooldownTimers[i] / cooldownDurations[i];

                // Cooldown bittiğinde işlemi sonlandır
                if (cooldownTimers[i] <= 0)
                {
                    cooldownTimers[i] = 0;
                    isOnCooldown[i] = false;
                    cooldownOverlays[i].fillAmount = 0;
                }
            }
        }
    }

    public void StartCooldown(int skillIndex, float duration)
    {
        if (!isOnCooldown[skillIndex])
        {
            isOnCooldown[skillIndex] = true;
            cooldownDurations[skillIndex] = duration; // Yeni cooldown süresi
            cooldownTimers[skillIndex] = duration;
            cooldownOverlays[skillIndex].fillAmount = 1; // UI'yi tamamen doldur
        }
    }

    public void UpdateCooldownExternally(int skillIndex, float remainingTime, float totalDuration)
    {
        if (skillIndex >= 0 && skillIndex < cooldownOverlays.Count)
        {
            isOnCooldown[skillIndex] = true;
            cooldownDurations[skillIndex] = totalDuration;
            cooldownTimers[skillIndex] = remainingTime;
            cooldownOverlays[skillIndex].fillAmount = remainingTime / totalDuration;
        }
    }
}
