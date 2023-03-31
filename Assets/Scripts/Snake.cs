using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private long scoreCounter=0;
    private float flowSpeed, verticalMovementSpeed = 50f, explosionInterval = 0.1f, nodeRadius;
    public ParticleSystem explositionParticle;
    public List<GameObject> nodeList;
    public TMPro.TextMeshProUGUI label, scoreText;
    public GameObject nodePrefab, snakeHead, snakeHeadSphere, gameOverUi;
    private GameManager gameManagerScript;
    private Rigidbody snakeHeadRB;

    private void Awake()
    {
        flowSpeed = verticalMovementSpeed;
        gameManagerScript = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        Vector3 spawnPos = new Vector3(0, nodeRadius / 2, 0);
        GameObject createdNode = Instantiate(nodePrefab, spawnPos, nodePrefab.transform.rotation);
        nodeList.Add(createdNode);
        AddNode(gameManagerScript.InitialSnakeLength);

        nodePrefab = (GameObject) Resources.Load("Prefabs/Node", typeof(GameObject));
        nodeRadius = nodePrefab.GetComponentInChildren<Transform>().localScale.x;
    }
    void Start()
    {
        snakeHeadRB = GetComponent<Rigidbody>();
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
        if (nodeList.Count <= 1 )
        {
            Destroy(nodeList[0]);
            nodeList.RemoveAt(0);
            snakeHead.transform.position += new Vector3(0, 0, -nodeRadius * 2);
            UpdateLabel();

            gameManagerScript.GameOver();
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
        }
    }

    public IEnumerator BoxCollision(GameObject collider)
    {
        int colliderInitialSize = collider.GetComponent<Enemy>().Size;

        if (collider.GetComponent<Enemy>().Size <= 1)
        {
            Destroy(collider);
            flowSpeed = verticalMovementSpeed;
        }
        else if (nodeList.Count <= 1)
        {
            //if (gameManagerScript.IsGameOver == false)
            if (true)
            {
                gameManagerScript.IsGameOver = true;
                Transform gameOverUiText = gameManagerScript.gameOverUI.transform.Find("GameOver");
                StartCoroutine(gameManagerScript.BlinkText(gameOverUiText));
                gameManagerScript.GameOver();
            }
        }
        else
        {
            collider.GetComponent<Enemy>().DecreaseSize();
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
        Vector3 newVector;

        nodeList[0].transform.position = Vector3.MoveTowards(nodeList[0].transform.position, snakeHead.transform.position, (float)(CalculateDistanceBetweenTwoPoints(nodeList[0].transform.position, snakeHead.transform.position) * flowSpeed * Time.deltaTime));

        for (int i = 1; i < nodeList.Count; i++)
        {
            newVector = (nodeList[i].transform.position - nodeList[i - 1].transform.position).normalized;
            nodeList[i].transform.position = nodeList[i - 1].transform.position + newVector * nodeRadius *2 ;
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
}