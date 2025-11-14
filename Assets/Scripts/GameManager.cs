using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public GameState currentState = GameState.Menu;

    [Header ("References")]
    public GameObject player;
    public GameObject startText;
    public GameObject background;
    public GameObject endText;
    public GameObject damageLine;
    public GameObject sunLights;

    private Animator startTextAnimator;
    private Animator endTextAnimator;
    private KBController playerController;
    private Movement playerMovement;
    private Renderer sunLightsRenderer;
    private Vector3 targetPos = Vector3.zero;

    [Header("Clouds Management")]
    public GameObject[] clouds;
    private KBController[] cloudControllers;

    public enum GameState
    {
        Menu,
        Playing,
        GameOver,
        Restarting
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeGame();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void InitializeGame()
    {
        FindGameObjects();

        switch (currentState)
        {
            case GameState.Menu:
                StartMenu();
                break;
            case GameState.Playing:
                StartGame();
                break;
        }
    }

    void FindGameObjects()
    {
        player = GameObject.Find("Player");
        startText = GameObject.Find("PressToStartText");
        background = GameObject.Find("Background");
        endText = GameObject.Find("GameOverText");
        damageLine = GameObject.Find("DamageLine");
        sunLights = GameObject.Find("SunLights");

        if (startText != null)
            startTextAnimator = startText.GetComponent<Animator>();

        if (player != null)
        {
            playerController = player.GetComponent<KBController>();
            playerMovement = player.GetComponent<Movement>();
        }

        if (endText != null)
            endTextAnimator = endText.GetComponent<Animator>();

        if (sunLights != null)
            sunLightsRenderer = sunLights.GetComponent<Renderer>();

        InitializeClouds();
    }

    void Update()
    {
        if (currentState == GameState.Menu && playerController != null && playerController.UpDown() == 1)
        {
            StartGame();
        }

        if (currentState == GameState.Playing)
        {
            if (targetPos.y < player.transform.position.y)
            {
                UpdateCameraAndBackground();
                UpdateDamageLine();
            }
            UpdateCloudsPosition();
        }
    }

    void StartMenu()
    {
        currentState = GameState.Menu;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (startTextAnimator != null)
            startTextAnimator.enabled = true;

        if (startText != null)
        {
            Canvas canvas = startText.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvas.worldCamera = Camera.main;
            }

            startText.SetActive(true);
        }

        if (endText != null)
            endText.SetActive(false);

        if (endTextAnimator != null)
            endTextAnimator.enabled = false;
    }

    void StartGame()
    {
        currentState = GameState.Playing;

        if (startTextAnimator != null)
            startTextAnimator.enabled = false;

        if (startText != null)
            startText.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            playerMovement.isJumping = true;
        }

        RandomizeCloudDirections();
        StartCoroutine(CloudDirectionLoop());
    }

    void UpdateDamageLine()
    {
        damageLine.transform.position = new Vector3(
            0,
            Camera.main.transform.position.y - Camera.main.orthographicSize,
            damageLine.transform.position.z
            );
    }

    void UpdateCameraAndBackground()
    {
        float topBoundCamera;
        float topBoundSunLights;
        if (sunLights != null && sunLightsRenderer != null)
        {
            topBoundSunLights = sunLights.transform.position.y + sunLightsRenderer.bounds.extents.y;
            topBoundCamera = Camera.main.transform.position.y + Camera.main.orthographicSize;
        }
        else
        {
            topBoundSunLights = 1;
            topBoundCamera = -1;
        }
        if (background == null || Camera.main == null || topBoundSunLights == topBoundCamera) return;

        targetPos = new Vector3(
            0,
            player.transform.position.y,
            background.transform.position.z
            );
        background.transform.position = Vector3.Lerp(
            background.transform.position,
            targetPos,
            Time.deltaTime * 2f
            );

        targetPos = new Vector3(
            0,
            player.transform.position.y,
            Camera.main.transform.position.z
            );
        Camera.main.transform.position = Vector3.Lerp(
            Camera.main.transform.position,
            targetPos,
            Time.deltaTime * 3f
            );
    }

    public void TriggerGameOver()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOver;
        StartCoroutine(GameOverRoutine());
    }

    public IEnumerator GameOverRoutine()
    {
        if (endText != null && Camera.main != null)
        {
            endText.transform.position = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                endText.transform.position.z
                );
            endText.SetActive(true);

            if (endTextAnimator != null)
            {
                endTextAnimator.enabled = true;
                endTextAnimator.Play("GameOverAnim");
            }
        }

        while (endTextAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        endTextAnimator.Play("GameOverAnimFullText");

        yield return new WaitForSeconds(3f);

        RestartLevel();
    }

    public void RestartLevel()
    {
        currentState = GameState.Restarting;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        currentState = GameState.Menu;
    }

    void InitializeClouds()
    {
        clouds = new GameObject[18];
        cloudControllers = new KBController[18];

        for (int i = 1; i <= 18; i++)
        {
            string cloudName = "Cloud" + i;
            clouds[i - 1] = GameObject.Find(cloudName);

            if (clouds[i - 1] != null)
            {
                cloudControllers[i - 1] = clouds[i - 1].GetComponent<KBController>();
            }
        }

        SetRandomCloudDirections();
    }

    void SetRandomCloudDirections()
    {
        for (int i = 0; i < clouds.Length; i++)
        {
            if (cloudControllers[i] != null)
            {
                bool moveRight = Random.Range(0, 2) == 0;

                cloudControllers[i].rightAuto = moveRight;
                cloudControllers[i].leftAuto = !moveRight;

                StartCoroutine(StartCloudMovementWithDelay(cloudControllers[i], Random.Range(0f, 2f)));
            }
        }
    }

    void UpdateCloudsPosition()
    {
        for (int i = 0; i < clouds.Length; i++)
        {
            if (clouds[i].transform.position.x > 20f) 
                clouds[i].transform.position = new Vector3(
                    -20f, 
                    clouds[i].transform.position.y, 
                    clouds[i].transform.position.z
                    );
            if (clouds[i].transform.position.x < -20f) 
                clouds[i].transform.position = new Vector3(
                    20f, 
                    clouds[i].transform.position.y, 
                    clouds[i].transform.position.z
                    );
        }
    }

    IEnumerator StartCloudMovementWithDelay(KBController cloudController, float delay)
    {
        bool originalRight = cloudController.rightAuto;
        bool originalLeft = cloudController.leftAuto;

        cloudController.rightAuto = false;
        cloudController.leftAuto = false;

        yield return new WaitForSeconds(delay);

        cloudController.rightAuto = originalRight;
        cloudController.leftAuto = originalLeft;
    }

    public void RandomizeCloudDirections()
    {
        SetRandomCloudDirections();
    }

    private IEnumerator CloudDirectionLoop()
    {
        while (currentState == GameState.Playing)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            for (int i = 0; i < clouds.Length; i++)
            {
                if (cloudControllers[i] != null)
                {
                    bool moveRight = Random.Range(0, 2) == 0;
                    cloudControllers[i].rightAuto = moveRight;
                    cloudControllers[i].leftAuto = !moveRight;
                }
            }
        }
    }
}
