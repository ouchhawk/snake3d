using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAnimation : MonoBehaviour
{
    private long ammoCap;
    public float flowSpeed = 40f, translateAmount = 0, horizontalMovementSpeed = 50f, verticalMovementSpeed = 50f, explosionInterval = 0.1f;
    public ParticleSystem explositionParticle;
    public List<GameObject> nodeList;
    public TMPro.TextMeshProUGUI label;
    public GameObject nodePrefab, snakeHead, snakeHeadSphere, gameOverUi; 
    public GameManager gameManager;
    private Rigidbody rb;
    private Vector3 labelOffset = new Vector3(0, 1, 0);
    private Camera mainCamera;
    private bool isGameOver = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        nodePrefab = (GameObject)Resources.Load("Prefabs/Node", typeof(GameObject));
        ammoCap = GameManager.AmmoCap;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        transform.GetComponent<Rigidbody>().MovePosition(transform.position + Vector3.forward * Time.deltaTime * 35f);
        snakeAnimation();
    }

    public void AddNode(long count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = new Vector3(gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.x, gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.y, gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.z - 2.5f);
            GameObject newNode = Instantiate(nodePrefab, spawnPos, nodePrefab.transform.rotation);
            gameManager.nodeList.Add(newNode);
        }
    }

    public void RemoveNode()
    {
        if (gameManager.nodeList.Count <= 1 )
        {
            if(!isGameOver)
            {
                gameManager.GameOver();
                isGameOver = true;
            }
        }
        else
        {
            gameManager.GetComponent<GameManager>().snakeHead.transform.position = gameManager.GetComponent<GameManager>().snakeHead.transform.position;

            Destroy(gameManager.nodeList[0]);
            gameManager.nodeList.RemoveAt(0);

            gameManager.nodeList[0].AddComponent<SnakeAnimation>();
            gameManager.nodeList[0].AddComponent<SnakeAnimation>().gameManager = gameManager;
            gameManager.nodeList[0].GetComponent<SnakeAnimation>().snakeHead = gameManager.nodeList[0];
            gameManager.snakeHead = gameManager.nodeList[0];
            gameManager.GetComponent<PlayerController>().sphere = gameManager.snakeHead;
            gameManager.mainCamera.GetComponent<CameraController>().player = gameManager.nodeList[0];

            for(int i = 0; i < gameManager.nodeList.Count; i++)
            {
                gameManager.nodeList[i].transform.position = gameManager.nodeList[i].transform.position + new Vector3(0, 0, 10f);
            }
            UpdateLabel();
        }
    }
    
    public void UpdateLabel()
    {
        label.text = gameManager.nodeList.Count.ToString();
    }
    public void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.transform.name == "Food(Clone)")
        {
            AddNode(collisionInfo.gameObject.GetComponent<Enemy>().Size);
            Destroy(collisionInfo.gameObject);
            Debug.Log("HIT TO FOOD PREFAB! " + collisionInfo.gameObject.GetComponent<Enemy>().Size.ToString());
        }
        else if (collisionInfo.transform.name == "Box(Clone)")
        {
            StartCoroutine(BoxCollision(collisionInfo.gameObject));
        }
        else if (collisionInfo.transform.name == "Divider(Clone)")
        {
            Debug.Log("HIT TO DIVIDER! " );
            gameManager.snakeHead.TryGetComponent<Rigidbody>(out var rb);
            rb.velocity = new Vector3(0, 0, 0);
            gameManager.snakeHead.transform.position = collisionInfo.gameObject.transform.position + new Vector3(100f,10f,10f);
        }
    }

    public IEnumerator BoxCollision(GameObject collider)
    {
        int colliderInitialSize = collider.GetComponent<Enemy>().Size;

        if (collider.GetComponent<Enemy>().Size < 1)
        {
            Destroy(collider);
        }
        else if (gameManager.nodeList.Count <= 1)
        {
            Transform gameOverUiText = gameManager.GetComponent<GameManager>().gameOverUI.transform.Find("GameOverText");
            StartCoroutine(gameManager.GetComponent<GameManager>().BlinkText(gameOverUiText));
        }
        else
        {
            collider.GetComponent<Enemy>().DecreaseSize();
        }
        RemoveNode();
        //scoreCounter++;
        //scoreText.text = scoreCounter.ToString();

        yield return new WaitForSeconds(explosionInterval);
    }

    private void snakeAnimation()
    {
        if (isGameOver == false)
        {
            Vector3 newVector;

            gameManager.nodeList[0].transform.position = gameManager.GetComponent<GameManager>().snakeHead.transform.position;

            for (int i = 1; i < gameManager.nodeList.Count; i++)
            {
                newVector = (gameManager.nodeList[i].transform.position - gameManager.nodeList[i - 1].transform.position).normalized;
                gameManager.nodeList[i].transform.position = gameManager.nodeList[i - 1].transform.position + newVector * 2.5f;
                //gameManager.nodeList[i].transform.position = Vector3.MoveTowards(gameManager.nodeList[i].transform.position, gameManager.nodeList[i - 1].transform.position, 25f*Time.deltaTime);
            }
        }
    }
}
