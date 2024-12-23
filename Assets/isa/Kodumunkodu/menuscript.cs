using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class menuscript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public bool GameisPaused = false;
    public GameObject pauseMenuUI;
    private GameObject player;
    private PlayerMovementPhysics playerMovementPhysicsSc;


    private void Start()
    {
        //LockCursor();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerMovementPhysicsSc = player.GetComponent<PlayerMovementPhysics>();
        }
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && playerMovementPhysicsSc != null && !playerMovementPhysicsSc.isDead)
        {
            if (GameisPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    public void Resume()
    {
        LockCursor();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameisPaused = false;
    }
    public void Pause()
    {
        UnlockCursor();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameisPaused = true;
    }
    public void test()
    {
        Debug.Log("Pressed");
    }
    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("mastervolume", volume);
    }

    public void SetFullScreen(bool isFullscreen)
    {
            Screen.fullScreen = isFullscreen;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
