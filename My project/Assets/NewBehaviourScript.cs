using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum FormState
{
    RED, GREEN, BLUE, NONE
}

enum Lane
{
    CENTER, LEFT, RIGHT
}


public class NewBehaviourScript : MonoBehaviour
{
    public float moveSpeed = 20.0f;
    public bool GameStarted = false;
    private bool isGameOver = false;
    private bool isPaused = false;
    private float lastGeneratedLine = 0f;

    public Light shieldSpotLight;

    // Modal Panels 
    public GameObject pausedPanel;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    // Panel mute buttons
    public GameObject muteButton;
    public GameObject unMuteButton;

    public GameObject token;
    public GameObject playground;
    public GameObject obstacle;
    public GameObject player;
    public TMP_Text blueScoreText;
    public TMP_Text redScoreText;
    public TMP_Text greenScoreText;
    public TMP_Text scoreText;

    public Transform rightLane;
    public Transform centerLane;
    public Transform leftLane;
    public Transform spawnLocation;
    public Transform playgroundLocation;

    // Audio
    public AudioSource crashSound;
    public AudioSource changeFormSound;
    public AudioSource invalidMovmentSound;
    public AudioSource tokenSound;
    public AudioSource usePowerSound;
    public AudioSource gameSound;
    public AudioSource menuSound;
    private bool isMuted = false;

    private float spawnInterval = 2.0f; // Time between spawns in seconds
    public int numberOfSpawns = 5;
    private int spawnCount = 0;

    // Materials
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material baseMaterial;

    public int blueScore = 0;
    public int greenScore = 0;
    public int redScore = 0;
    public int score = 0;
    private int MAX_SCORE = 5;

    private Lane currentLane = Lane.CENTER;
    private FormState currentPower = FormState.NONE;

    private bool isShielded = false;
    private int scoreMultiplier = 1;
    private int energyMultiplier = 1;


    void Start()
    {
        shieldSpotLight.enabled = false;
        lastGeneratedLine = spawnLocation.position.z;
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    public void handleGreenPowerActivation()
    {
        if (currentPower != FormState.GREEN) return;
        if (scoreMultiplier > 1 || energyMultiplier > 1) return;
        greenScore--;
        greenScoreText.text = "Green Energy Points: " + greenScore;

        //if (redScore == 1) return;
        scoreMultiplier = 5;
        energyMultiplier = 2;

        usePowerSound.Play();

    }

    public bool isMuteStatus(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            if (audioSource.mute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void ToggleMute()
    {
        bool changeFormSoundMuted = isMuteStatus(changeFormSound);

        bool muteBool = true;
        if (!changeFormSoundMuted)
        {
            muteBool = true;
            muteButton.SetActive(false);
            unMuteButton.SetActive(true);
        }
        else
        {
            muteBool = false;
            muteButton.SetActive(true);
            unMuteButton.SetActive(false);
        }

        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.mute = muteBool;
        }
    }

    public void mute()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    public void startGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        GameStarted = true;
        gameSound.Play();
    }


    public void handleRedPowerActivation()
    {
        if (currentPower != FormState.RED) return;

        redScore--;
        redScoreText.text = "Red Energy Points: " + redScore;
        //if (redScore == 1) return;
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        foreach (GameObject obj in obstacles)
        {
            // if bigger than current transform position
            if (transform.position.z < obj.transform.position.z)
            {
                Destroy(obj);
            }
        }

        usePowerSound.Play();
    }

    bool AreMaterialsLike(Material material1, Material material2)
    {
        return material1.shader == material2.shader &&
               material1.mainTexture == material2.mainTexture;
    }

    public string getRandomTag(int index)
    {
        string[] tags = { "redToken", "greenToken", "blueToken" };
        return tags[index];
    }

    public Material getRandomMaterial(int index)
    {

        Material[] materials = { redMaterial, greenMaterial, blueMaterial };

        return materials[index];
    }

    public float getRandomLaneAxis()
    {

        float[] laneAxises = { leftLane.position.x, centerLane.position.x, rightLane.position.x };

        int randomIndex = Random.Range(0, laneAxises.Length);
        return laneAxises[randomIndex];
    }

    

    public void changeForm(GameObject obj, Material material, FormState newForm)
    {
        currentPower = newForm;
        energyMultiplier = 1;
        scoreMultiplier = 1;
        Renderer playerRenderer = obj.GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            playerRenderer.material = material;
        }

        changeFormSound.Play();
    }

    public void changeToGreenForm()
    {
        if (currentPower != FormState.GREEN)
        {
            if (greenScore != MAX_SCORE) return;
            greenScore = greenScore - 1;
            currentPower = FormState.GREEN;
            changeForm(player, greenMaterial, FormState.GREEN);
            isShielded = false;
        }
    }

    public void changeToRedForm()
    {
        if (currentPower != FormState.RED)
        {
            if (redScore != MAX_SCORE) return;
            redScore = redScore - 1;
            currentPower = FormState.RED;
            changeForm(player, redMaterial, FormState.RED);
            isShielded = false;
        }
    }

    public void changeToBlueForm()
    {
        if (currentPower != FormState.BLUE)
        {
            if (blueScore != MAX_SCORE) return;
            blueScore = blueScore - 1;
            currentPower = FormState.BLUE;
            changeForm(player, blueMaterial, FormState.BLUE);
        }
    }

    public void handleFormChange()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            changeToRedForm();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            changeToGreenForm();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            changeToBlueForm();
        }
        else
        {
            return;
        }


    }

    public void handleBluePowerActivation()
    {
        if (currentPower != FormState.BLUE) return;
        if (isShielded) return;
        blueScore--;
        blueScoreText.text = "Blue Energy Points: " + blueScore;
        if (blueScore == 1) return;
        isShielded = true;
        usePowerSound.Play();
        shieldSpotLight.enabled = true;
    }

    public void handleFormUseActivation()
    {
        if (currentPower == FormState.NONE) return;
        if (currentPower == FormState.RED)
        {
            handleRedPowerActivation();
        }
        else if (currentPower == FormState.GREEN)
        {
            handleGreenPowerActivation();
        }
        else if (currentPower == FormState.BLUE)
        {
            handleBluePowerActivation();
        }
    }



    public void handleFormUse()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            handleFormUseActivation();
        }
    }



    public void handleFormEnding()
    {
        if (currentPower == FormState.NONE) return;
        if (currentPower == FormState.RED)
        {
            if (redScore > 0) return;
        }
        else if (currentPower == FormState.GREEN)
        {
            if (greenScore > 0) return;
        }
        else if (currentPower == FormState.BLUE)
        {
            if (blueScore > 0) return;

        }
        else
        {
            return;
        }

        currentPower = FormState.NONE;
        changeForm(player, baseMaterial, FormState.NONE);
    }

    public void handleGamePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        gameSound.Pause();
        menuSound.Play();
        pausedPanel.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        gameSound.Play();
        menuSound.Stop();

        pausedPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void moveRight()
    {
        if (currentLane == Lane.RIGHT)
        {
            invalidMovmentSound.Play();
            return;
        }
        else if (currentLane == Lane.CENTER)
        {
            currentLane = Lane.RIGHT;
        }
        else if (currentLane == Lane.LEFT)
        {
            currentLane = Lane.CENTER;
        }
    }

    public void moveLeft()
    {
        if (currentLane == Lane.LEFT)
        {
            invalidMovmentSound.Play();
            return;
        }
        else if (currentLane == Lane.CENTER)
        {
            currentLane = Lane.LEFT;
        }
        else if (currentLane == Lane.RIGHT)
        {
            currentLane = Lane.CENTER;
        }
    }

    public void handleKeyMovments()
    {

        Vector3 targetPosition = transform.position;
        float speed = 20.0f;
        if (currentLane == Lane.RIGHT)
        {
            targetPosition.x = rightLane.position.x;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else if (currentLane == Lane.CENTER)
        {
            targetPosition.x = centerLane.position.x;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else if (currentLane == Lane.LEFT)
        {
            targetPosition.x = leftLane.position.x;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            moveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            moveRight();
        }
    }

    public void updatePowers(Collider other)
    {
        int scoreCounter = 0;
        int redEnergyCounter = 0;
        int blueEnergyCounter = 0;
        int greenEnergyCounter = 0;

        tokenSound.Play();
        if (other.CompareTag("redToken"))
        {
            redEnergyCounter++;
        }
        else if (other.CompareTag("blueToken"))
        {
            blueEnergyCounter++;
        }
        else if (other.CompareTag("greenToken"))
        {
            greenEnergyCounter++;
        }



        if ((other.CompareTag("redToken") && currentPower == FormState.RED) ||
        (other.CompareTag("blueToken") && currentPower == FormState.BLUE) ||
        (other.CompareTag("greenToken") && currentPower == FormState.GREEN))
        {
            if (currentPower == FormState.GREEN)
            {
                scoreCounter += 10;
                greenEnergyCounter--;
            }
            else
            {
                scoreCounter += 2;
            }
        }
        else
        {
            if (currentPower == FormState.GREEN)
            {
                scoreCounter += 5;
                greenEnergyCounter++;
            }
            else
            {
                scoreCounter++;
            }

        }

        redScore += (redEnergyCounter * energyMultiplier);
        if (redScore > MAX_SCORE) redScore = MAX_SCORE;
        blueScore += (blueEnergyCounter * energyMultiplier);
        if (blueScore > MAX_SCORE) blueScore = MAX_SCORE;
        greenScore += (greenEnergyCounter * energyMultiplier);
        if (greenScore > MAX_SCORE) greenScore = MAX_SCORE;
        score += (scoreCounter * scoreMultiplier);


        if (energyMultiplier > 1)
        {
            energyMultiplier = 1;
        }

        if (scoreMultiplier > 1)
        {
            scoreMultiplier = 1;
        }

        redScoreText.text = "Red Energy Points: " + redScore;
        blueScoreText.text = "Blue Energy Points: " + blueScore;
        greenScoreText.text = "Green Energy Points: " + greenScore;
        scoreText.text = "Score: " + score;

    }

    public void deactivatePowers()
    {
        bool wasShielded = isShielded;
        if (isShielded)
        {
            isShielded = false;
            shieldSpotLight.enabled = false;
        }
        energyMultiplier = 1;
        scoreMultiplier = 1;
        if (currentPower == FormState.BLUE && wasShielded) return;
        changeForm(player, baseMaterial, FormState.NONE);
    }

    public void endGameOver()
    {
        finalScoreText.text = "Final Score : " + score;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        gameSound.Stop();
        menuSound.Play();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("obstacle"))
        {
            if (!isShielded && currentPower == FormState.NONE)
            {
                endGameOver();

            }
            deactivatePowers();
        }

        // handle token taking 
        updatePowers(other);

        Destroy(other.gameObject);
    }

    public void createObject(int objectType, float positionX, float sparePosZ)
    {

        Renderer playgroundRenderer = playground.GetComponent<Renderer>();
        //if (playgroundRenderer == null) return; 
        Bounds playgroundBounds = playgroundRenderer.bounds;
        float playgroundMaxZ = playgroundBounds.max.z;

        if (lastGeneratedLine - 5 >= playgroundMaxZ) return;

        string[] objectTypes = { "obstacle", "token", null };
        
        lastGeneratedLine = sparePosZ;
        //Vector3 position = new Vector3(positionX, -8.7f, spawnLocation.position.z + sparePosZ);
        Vector3 position = new Vector3(positionX, -8.7f, sparePosZ);

        string objectTypeInstance = objectTypes[objectType];
        if (objectTypeInstance == null) return;
        else if (objectTypeInstance == "obstacle")
        {
            Instantiate(obstacle, position, Quaternion.identity);
        }
        else if (objectTypeInstance == "token")
        {
            GameObject spawnedToken = Instantiate(token, position, Quaternion.identity);

            Renderer objectRenderer = spawnedToken.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                int index = Random.Range(0, 3);
                objectRenderer.tag = getRandomTag(index);
                objectRenderer.material = getRandomMaterial(index);
            }
        }
    }

    public void handleSpawnPermutation(float sparePosZ)
    {

        int numbObstacles = 0;
        int nullObjects = 0;

        int rightLaneIndex = Random.Range(0, 3);
        if (rightLaneIndex == 0) numbObstacles++;
        if (rightLaneIndex == 2) nullObjects++;

        int centerLaneIndex = Random.Range(0, 3);
        if (centerLaneIndex == 0) numbObstacles++;
        if (centerLaneIndex == 2) nullObjects++;

        int leftLaneIndex = Random.Range(0, 3);
        if (leftLaneIndex == 0) numbObstacles++;
        if (leftLaneIndex == 2) nullObjects++;


        if (numbObstacles == 3)
        {
            rightLaneIndex = Random.Range(1, 3);
        }
        else if (nullObjects == 3)
        {
            rightLaneIndex = Random.Range(0, 2);
            centerLaneIndex = Random.Range(0, 2);
        }
        float[] laneAxises = { leftLane.position.x, centerLane.position.x, rightLane.position.x };
        createObject(rightLaneIndex, laneAxises[2], sparePosZ);
        createObject(centerLaneIndex, laneAxises[1], sparePosZ);
        createObject(leftLaneIndex, laneAxises[0], sparePosZ);
    }



    public void SpawnObject()
    {

        if (!isGameOver && GameStarted)
        {
            if(lastGeneratedLine <= spawnLocation.position.z)
            {
                lastGeneratedLine = spawnLocation.position.z + 15.0f;
            }
            handleSpawnPermutation(lastGeneratedLine + 10.0f);
            handleSpawnPermutation(lastGeneratedLine + 10.0f);  
        }
    }



    public void handleKillingObject()
    {
        // This kills objects that has been passed by the player
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        GameObject[] rTokens = GameObject.FindGameObjectsWithTag("redToken");
        GameObject[] gTokens = GameObject.FindGameObjectsWithTag("greenToken");
        GameObject[] bTokens = GameObject.FindGameObjectsWithTag("blueToken");
        GameObject[] trees = GameObject.FindGameObjectsWithTag("trees");

        List<GameObject> allObjectsList = new List<GameObject>();
        allObjectsList.AddRange(obstacles);
        allObjectsList.AddRange(rTokens);
        allObjectsList.AddRange(gTokens);
        allObjectsList.AddRange(bTokens);
        allObjectsList.AddRange(trees);

        // Loop through the combined array and do something with each object
        foreach (GameObject obj in allObjectsList)
        {
            if (transform.position.z - 10 > obj.transform.position.z)
            {
                Destroy(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShielded)
        {
            shieldSpotLight.enabled = false;
        }
        if (!GameStarted || isGameOver) return;


        Renderer playgroundRenderer = playground.GetComponent<Renderer>();
        //if (playgroundRenderer == null) return; 
        Bounds playgroundBounds = playgroundRenderer.bounds;
        float playgroundMaxZ = playgroundBounds.max.z;

        double playerZPosition = transform.position.z;
        if (playerZPosition < playgroundMaxZ - 50)
        {

        }
        else
        {
            if (!isPaused)
            {
                GameObject tmpPlayground = playground;
                playground = Instantiate(playground, new Vector3(playgroundLocation.position.x, playgroundLocation.position.y, (float)playerZPosition), Quaternion.identity);
                playgroundLocation = playground.transform;
                //Destroy(tmpPlayground);
            }
        }
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        handleGamePause();
        mute();
        handleFormEnding();
        handleFormUse();
        handleFormChange();
        handleKillingObject();
        handleKeyMovments();
    }
}
