using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

enum FormState
{
    RED, GREEN, BLUE, NONE
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

    private int currentLane = 0; // -1L, 0C, 1R
    private FormState currentPower = FormState.NONE;

    private bool isShielded = false;

    void Start()
    {
        isGameOn = true;
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    public void handleGreenPowerActivation(int scoreAdd, int energyAdd, FormState tokenType)
    {
        if (currentPower != FormState.GREEN) return;
       
        
        if(tokenType == FormState.RED)
        {
            score = score + (scoreAdd * 5);
            redScore = redScore + (energyAdd * 2)
        } else if (tokenType == FormState.BLUE)
        {
            score = score + (scoreAdd * 5);
            blueScore = blueScore + (energyAdd * 2)
        } else if (tokenType == FormState.GREEN)
        {
            score = score + (scoreAdd * 10);
            greenScore = greenScore + (energyAdd * 2)
        } else
        {
            return;
        }

    }

    bool AreMaterialsLike(Material material1, Material material2)
    {
        // Compare shader and main texture
        return material1.shader == material2.shader &&
               material1.mainTexture == material2.mainTexture;
    }

    public Material getRandomMaterial()
    {

        Material[] materials = { redMaterial, greenMaterial, blueMaterial };

        int randomIndex = Random.Range(0, materials.Length);
        return materials[randomIndex];
    }

    public float getRandomLaneAxis()
    {

        float[] laneAxises = { leftLane.position.x, centerLane.position.x, rightLane.position.x };

        int randomIndex = Random.Range(0, laneAxises.Length);
        return laneAxises[randomIndex];
    }

    public void changeForm(GameObject obj, Material material)
    {
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
                changeForm(player, formMaterial);
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
                changeForm(player, formMaterial);
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
                changeForm(player, formMaterial);
            }
                
        }
        else
        {
            return;
        }

        
    }

    public void handleFormUse()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentPower == FormState.NONE) return;
            if (currentPower == FormState.RED)
            {
                redScore--;
                redScoreText.text = "Red Score: " + redScore;
            }
            if (currentPower == FormState.GREEN)
            {
                greenScore--;
                greenScoreText.text = "Green Score: " + greenScore;
            }
            if (currentPower == FormState.BLUE)
            {
                if (isShielded) return;
                blueScore--;
                blueScoreText.text = "Blue Score: " + blueScore;
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
        changeForm(player, baseMaterial);
    }

    public void handleKeyMovments()
    {
        Vector3 xMovmentsVector = new Vector3(0.0f, 0.0f, 0.0f); ;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (currentLane == -1) return;
            else if (currentLane == 0)
            {
                xMovmentsVector = new Vector3(leftLane.position.x, transform.position.y, transform.position.z);

            }
            else if (currentLane == 1)
            {
                xMovmentsVector = new Vector3(centerLane.position.x, transform.position.y, transform.position.z);
            }

            currentLane = currentLane - 1;
            transform.position = xMovmentsVector;
            //Vector3 moveDirection = new Vector3(xMovmentsVector, 0.0f, 0.0f);
            //Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            //transform.Translate(movement);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (currentLane == 1) return;
            else if (currentLane == 0)
            {
                xMovmentsVector = new Vector3(rightLane.position.x, transform.position.y, transform.position.z);

            }
            else if (currentLane == -1)
            {
                xMovmentsVector = new Vector3(centerLane.position.x, transform.position.y, transform.position.z);
            }

            currentLane = currentLane + 1;
            transform.position = xMovmentsVector;
            //Vector3 moveDirection = new Vector3(xMovmentsVector, 0.0f, 0.0f);
            //Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            //transform.Translate(movement);
        }
    }

    public void updatePowers(Material m)
    {
        if(AreMaterialsLike(m, redMaterial))
        { 
            if (redScore < MAX_SCORE) redScore++;
            redScoreText.text = "Red Score: " + redScore;
            
        } else if(AreMaterialsLike(m, blueMaterial))
        { 
            if (blueScore < MAX_SCORE) blueScore++;
            blueScoreText.text = "Blue Score: " + blueScore;    

        } else if(AreMaterialsLike(m, greenMaterial))
        {
            if (greenScore < MAX_SCORE) greenScore++;
            greenScoreText.text = "Green Score: " + greenScore;
        }

        if (score == MAX_SCORE) return;
        score++;
        if((AreMaterialsLike(m, redMaterial) && currentPower == FormState.RED) || (AreMaterialsLike(m, blueMaterial) && currentPower == FormState.BLUE) || (AreMaterialsLike(m, greenMaterial) && currentPower == FormState.GREEN))
        {
            score++;
        }
        scoreText.text = "Score: " + score;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("obstacle"))
        {
            if(isShielded)
            {
                isShielded = false;
            } else
            {
                if(currentPower == FormState.NONE)
                {
                    isGameOn = false;
                }
            }
            changeForm(player, baseMaterial);
            
        }
        else if (other.CompareTag("token"))
        {
            Renderer otherRenderer = other.GetComponent<Renderer>();
            if (otherRenderer != null)
            {
                updatePowers(otherRenderer.material); 
            }
        }

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
                objectRenderer.material = getRandomMaterial();
            }

            // render obstacles 
            Vector3 randomPositionObstacle = new Vector3(getRandomLaneAxis(), -8.7f, spawnLocation.position.z + 15.0f);
            Instantiate(obstacle, randomPositionObstacle, Quaternion.identity);
            spawnCount++;

            //if (spawnCount >= numberOfSpawns)
            //{
            //    CancelInvoke("SpawnObject");
            //}
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
            GameObject tmpPlayground = playground;
            playground = Instantiate(playground, new Vector3(playgroundLocation.position.x, playgroundLocation.position.y, (float) playerZPosition), Quaternion.identity);
            Destroy(tmpPlayground);
        }
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        handleFormEnding();
        handleFormUse();
        handleFormChange();
        handleKillingObject();
        handleKeyMovments();

    }
}
