using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class Snake : MonoBehaviour
{
    private long scoreCounter= SessionInformation.totalScore;
    private float flowSpeed, verticalMovementSpeed = 50f, explosionInterval = 0.1f, nodeRadius;
    public ParticleSystem explositionParticle;
    public List<GameObject> nodeList;
    public TMPro.TextMeshProUGUI label, scoreText, levelText;
    public GameObject nodePrefab, snakeHead, snakeHeadSphere, gameOverUi, continueText;
    private GameManager gameManagerScript;
    private Rigidbody snakeHeadRB;
    private Transform cont;
    private AudioManager audioManager;
    private double randomNumber;
    System.Random random;

    private void Awake()
    {
        flowSpeed = verticalMovementSpeed;
        gameManagerScript = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        Vector3 spawnPos = new Vector3(0, nodeRadius / 2, 0);
        GameObject createdNode = Instantiate(nodePrefab, spawnPos, nodePrefab.transform.rotation);
        nodeList.Add(createdNode);
        AddNode(SessionInformation.snakeLength);

        nodePrefab = (GameObject) Resources.Load("Prefabs/Node", typeof(GameObject));
        nodeRadius = nodePrefab.GetComponentInChildren<Transform>().localScale.x;
        cont = gameManagerScript.gameOverUI.transform.Find("TapToContinue");
        audioManager = GameObject.FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
        random = new System.Random();
    }
    void Start()
    {
        snakeHeadRB = GetComponent<Rigidbody>();
        UpdateLevelText();
        UpdateScoreText(scoreCounter);
    }

    private void Update()
    {
        UpdateSizeText();
    }

    private void FixedUpdate()
    {
        snakeHeadRB.MovePosition(transform.position + Vector3.forward * Time.deltaTime * flowSpeed);
        AnimateSnake();
    }

    public void AddNode(long count)
    {
        int lastNode = nodeList.Count - 1;
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = new Vector3(nodeList[lastNode].transform.position.x, nodeList[lastNode].transform.position.y, nodeList[lastNode].transform.position.z - nodeRadius);
            GameObject createdNode = Instantiate(nodePrefab, spawnPosition, nodePrefab.transform.rotation);
            nodeList.Add(createdNode);
        }
    }

    public void RemoveNode()
    {
        if (nodeList.Count < 1 )
        {
            nodeList.RemoveAt(0);
            Destroy(nodeList[0]);
            Destroy(nodeList[0]);
            snakeHead.transform.position += new Vector3(0, 0, -nodeRadius * 2);
            UpdateLabel();
            gameManagerScript.GameOver();
            snakeHead.GetComponent<Snake>().enabled = false;
        }
        else
        {
            Destroy(nodeList[0]);
            nodeList.RemoveAt(0);
            snakeHead.transform.position += new Vector3(0, 0, -nodeRadius*2);
            UpdateLabel();
        }
    }
    
    public void UpdateLabel()
    {
        label.text = nodeList.Count.ToString();
    }

    private void OnTriggerExit(Collider other)
    {
        flowSpeed = verticalMovementSpeed;
    }

    public void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.transform.name == "Food(Clone)")
        {
            AddNode(collisionInfo.gameObject.GetComponent<Enemy>().Size);
            Destroy(collisionInfo.gameObject);
            audioManager.PlayFoodAte();
        }
        else if (collisionInfo.transform.name == "Box(Clone)")
        {
            flowSpeed = 0f;
            StartCoroutine(BoxCollision(collisionInfo.gameObject));
        }
        else if (collisionInfo.transform.name == "Divider(Clone)")
        {
            gameManagerScript.snakeHead.TryGetComponent<Rigidbody>(out var rb);
            snakeHeadRB.velocity = new Vector3(0, 0, 0);
        }
        else if (collisionInfo.transform.name == "FinishLine(Clone)")
        {
            gameManagerScript.gameOverUI.transform.Find("StageClear").gameObject.SetActive(true);
            gameManagerScript.mainCamera.GetComponent<CameraController>().player=gameManagerScript.finishLine;
            Console.WriteLine("FINISH!!");
            SaveState();
            gameManagerScript.IsStageClear = true;
            audioManager.PlayStageClear();
        }
    }


    public void SaveState()
    {
        SessionInformation.snakeLength = nodeList.Count;
        SessionInformation.totalScore = scoreCounter;
        SessionInformation.level++;
    }

    public IEnumerator BoxCollision(GameObject collider)
    {
        int colliderInitialSize = collider.GetComponent<Enemy>().Size;

        if (collider.GetComponent<Enemy>().Size <= 1)
        {
            Destroy(collider);
            audioManager.PlayBoxDestroyed();
            flowSpeed = verticalMovementSpeed;
        }
        else if (nodeList.Count < 1)
        {
            //if (gameManagerScript.IsGameOver == false)
            if (true)
            {
                verticalMovementSpeed = 0;
                gameManagerScript.IsGameOver = true;
                Transform gameOverUiText = gameManagerScript.gameOverUI.transform.Find("GameOver");
                StartCoroutine(gameManagerScript.BlinkText(gameOverUiText));
                //scoreText.enabled = false;
                cont.gameObject.SetActive(true);
                gameManagerScript.GameOver();
            }
        }
        else
        {
            
            if (nodeList.Count%3 == 0)
            {
                audioManager.PlayHit2();
            }
            else 
            {
                collider.GetComponent<Enemy>().DecreaseSize();
                Thread.Sleep(1);
                randomNumber = random.NextDouble();
                if (randomNumber < 0.15)
                {
                    audioManager.PlayHit1();
                }
            }
        }
        RemoveNode();
        scoreCounter++;
        UpdateScoreText(scoreCounter);

        yield return new WaitForSeconds(explosionInterval);
    }

    private double CalculateDistanceBetweenTwoPoints(Vector3 point1, Vector3 point2)
    {
        return Math.Sqrt(Math.Pow(point1.x - point2.x, 2) + Math.Pow(point1.y - point2.y, 2) + Math.Pow(point1.z - point2.z, 2));
    }
    private void AnimateSnake()
    {
        if(!gameManagerScript.IsGameOver)
        {
            Vector3 newVector;
            nodeList[0].transform.position = Vector3.MoveTowards(nodeList[0].transform.position, snakeHead.transform.position, (float)(CalculateDistanceBetweenTwoPoints(nodeList[0].transform.position, snakeHead.transform.position) * flowSpeed * Time.deltaTime));

            for (int i = 1; i < nodeList.Count; i++)
            {
                newVector = (nodeList[i].transform.position - nodeList[i - 1].transform.position).normalized;
                nodeList[i].transform.position = nodeList[i - 1].transform.position + newVector * nodeRadius * 2;
            }
        }
    }

    private void UpdateSizeText()
    {
        snakeHead.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = nodeList.Count.ToString();
    }
    private void UpdateScoreText(long score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
    private void UpdateLevelText()
    {
        levelText.text = "Level " + SessionInformation.level.ToString();
    }
}
