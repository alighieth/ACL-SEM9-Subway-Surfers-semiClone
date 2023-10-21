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
    public bool isGameOn = false;

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

    public float spawnInterval = 2.0f; // Time between spawns in seconds
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

    private Lane currentLane = Lane.CENTER; // -1L, 0C, 1R
    private FormState currentPower = FormState.NONE;

    private bool isShielded = false;
    private int scoreMultiplier = 1;
    private int energyMultiplier = 1;


    void Start()
    {
        isGameOn = true;
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    public void handleGreenPowerActivation()
    {
        if (currentPower != FormState.GREEN) return;
        greenScore--;
        greenScoreText.text = "Green Score: " + greenScore;

        scoreMultiplier = 5;
        energyMultiplier = 2;

    }

    public void handleRedPowerActivation()
    {
        if (currentPower != FormState.RED) return;
        redScore--;
        redScoreText.text = "Red Score: " + redScore;

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        foreach (GameObject obj in obstacles)
        {
            // if bigger than current transform position
            if (transform.position.z < obj.transform.position.z)
            {
                Destroy(obj);
            }
        }
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
    }

    public void handleFormChange()
    {
        Material formMaterial = baseMaterial;
        if (Input.GetKeyDown(KeyCode.J))
        {
            if(currentPower != FormState.RED)
            {
                formMaterial = redMaterial;
                if (redScore != MAX_SCORE) return;
                redScore = redScore - 1;
                currentPower = FormState.RED;
                changeForm(player, formMaterial, FormState.RED);
                isShielded = false;
            }

        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentPower != FormState.GREEN)
            {
                formMaterial = greenMaterial;
                if (greenScore != MAX_SCORE) return;
                greenScore = greenScore - 1;
                currentPower = FormState.GREEN;
                changeForm(player, formMaterial, FormState.GREEN);
                isShielded = false;
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentPower != FormState.BLUE)
            {
                formMaterial = blueMaterial;
                if (blueScore != MAX_SCORE) return;
                blueScore = blueScore - 1;
                currentPower = FormState.BLUE;
                changeForm(player, formMaterial, FormState.BLUE);
            }
                
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
        isShielded=true;
        blueScore--;
        blueScoreText.text = "Blue Score: " + blueScore;
    }

   

    public void handleFormUse()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentPower == FormState.NONE) return;
            if (currentPower == FormState.RED)
            {
                handleRedPowerActivation();
            }
            if (currentPower == FormState.GREEN)
            {
                handleGreenPowerActivation();
            }
            if (currentPower == FormState.BLUE)
            {
                handleBluePowerActivation();
            }
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
            
        } else
        {
            return;
        }

        currentPower = FormState.NONE;
        changeForm(player, baseMaterial, FormState.NONE);
    }

    public void handleKeyMovments()
    {
        Debug.Log(currentLane);

        Vector3 targetPosition = transform.position;
        float speed = 20.0f;
        if (currentLane == Lane.RIGHT)
        {
            targetPosition.x = rightLane.position.x;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else if (currentLane == Lane.CENTER)
        {
            targetPosition.x = 0;//centerLane.position.x;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else if (currentLane == Lane.LEFT)
        {
            targetPosition.x = leftLane.position.x;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }




        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (currentLane == Lane.LEFT) return;
            else if (currentLane == Lane.CENTER)
            {
                currentLane = Lane.LEFT;
            }
            else if (currentLane == Lane.RIGHT)
            {
                currentLane = Lane.CENTER;
            }
     
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (currentLane == Lane.RIGHT) return;
            else if (currentLane == Lane.CENTER)
            {
                currentLane = Lane.RIGHT;
            }
            else if (currentLane == Lane.LEFT)
            {
                currentLane = Lane.CENTER;
            }

        }
    }

    public void updatePowers(Collider other)
    {
        int scoreCounter = 0;
        int redEnergyCounter = 0;
        int blueEnergyCounter = 0;
        int greenEnergyCounter = 0;
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
            if(currentPower == FormState.GREEN)
            {
                scoreCounter += 10;
                greenEnergyCounter--;
            } else
            {
                scoreCounter+=2;
            }
        } else
        {
            if(currentPower == FormState.GREEN)
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
        if(redScore > MAX_SCORE) redScore = MAX_SCORE;
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

        redScoreText.text = "Red Score: " + redScore;
        blueScoreText.text = "Blue Score: " + blueScore;
        greenScoreText.text = "Green Score: " + greenScore;
        scoreText.text = "Score: " + score;

    }

    public void deactivatePowers()
    {
        if(isShielded)
        {
           isShielded = false;
        }
        energyMultiplier = 1;
        scoreMultiplier = 1;
        changeForm(player, baseMaterial, FormState.NONE);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("obstacle"))
        {
            if(!isShielded && currentPower == FormState.NONE)
            {
                isGameOn = false;
            }
            deactivatePowers();
        }

        // handle token taking 
        updatePowers(other);

        Destroy(other.gameObject);
    }

    public void SpawnObject()
    {

        if (isGameOn)
        {
            // render token
            Vector3 randomPositionToken = new Vector3(getRandomLaneAxis(), -8.7f, spawnLocation.position.z + 10.0f);
            GameObject spawnedToken = Instantiate(token, randomPositionToken, Quaternion.identity);

            Renderer objectRenderer = spawnedToken.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                int index = 1;//Random.Range(0, 3);
                objectRenderer.tag = getRandomTag(index);
                objectRenderer.material = getRandomMaterial(index);
            }

            // render obstacles 
            Vector3 randomPositionObstacle = new Vector3(getRandomLaneAxis(), -8.7f, spawnLocation.position.z + 15.0f);
            Instantiate(obstacle, randomPositionObstacle, Quaternion.identity);
            spawnCount++;
        }
    }

   

    public void handleKillingObject()
    {
        // This kills objects that has been passed by the player
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("token");

        GameObject[] obstaclesAndTokens = new GameObject[obstacles.Length + tokens.Length];
        obstacles.CopyTo(obstaclesAndTokens, 0);
        tokens.CopyTo(obstaclesAndTokens, obstacles.Length);

        // Loop through the combined array and do something with each object
        foreach (GameObject obj in obstaclesAndTokens)
        {
            if(transform.position.z - 10 > obj.transform.position.z)
            {
                Destroy(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOn) return;
        Renderer playgroundRenderer = playground.GetComponent<Renderer>();
        if (playgroundRenderer == null) return; 
        Bounds playgroundBounds = playgroundRenderer.bounds;
        float playgroundMaxZ = playgroundBounds.max.z;
        
        double playerZPosition = transform.position.z;
        if (playerZPosition < playgroundMaxZ - 2)
        {
            
        } else
        {
            //GameObject tmpPlayground = playground;
            playground = Instantiate(playground, new Vector3(playgroundLocation.position.x, playgroundLocation.position.y, (float) playerZPosition), Quaternion.identity);
            //Destroy(tmpPlayground);
        }
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        handleFormEnding();
        handleFormUse();
        handleFormChange();
        handleKillingObject();
        handleKeyMovments();

    }
}
