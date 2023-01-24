using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeAnimation : MonoBehaviour
{
    private long ammoCap;
    private float scale = 1f;
    public float flowSpeed = 20f, translateAmount = 0;
    public ParticleSystem explositionParticle;
    public List<GameObject> nodeList;

    public TMPro.TextMeshProUGUI label;
    //public TMPro.TextMeshProUGUI scoreText, gameOverScore;
    public GameObject nodePrefab, snakeHead, snakeHeadPosition; //, gameOverUI, playingUI, debugUI;
    public GameManager gameManager;
    public Transform nextTarget;
    private float horizontalInput, verticalInput, mixedInput;
    private float horizontalMovementSpeed = 50f, verticalMovementSpeed = 30f;
    public float period = 1f;
    public float leftViewEdge = -5.4f;
    public float rightViewEdge = 5f;
    private float powerupSpeedMultiplier = 1.5f;
    private float explosionInterval = 0.1f;
    private Rigidbody rb;
    //public GameObject enemyScript;
    private float movementDistance;

    private bool stillCollidingWithABox = false;
    private int scoreCounter = 0;
    private Color color;
    private Vector3 labelOffset = new Vector3(0, 1, 0);
    private bool isTimePassing = true;
    private bool isPoweredUp = false;
    private float powerUpDuration = 8f;

    private float oldSnakePositionX =0;


    public bool IsTimePassing { get => isTimePassing; set => isTimePassing = value; }
    public Transform NextTarget { get => nextTarget; set => nextTarget = value; }
    public int ScoreCounter { get => scoreCounter; set => scoreCounter = value; }
    public float HorizontalInput { get => HorizontalInput1; set => HorizontalInput1 = value; }
    public float VerticalInput { get => VerticalInput1; set => VerticalInput1 = value; }
    public float MixedInput { get => MixedInput1; set => MixedInput1 = value; }
    public float HorizontalInput1 { get => horizontalInput; set => horizontalInput = value; }
    public float VerticalInput1 { get => verticalInput; set => verticalInput = value; }
    public float MixedInput1 { get => mixedInput; set => mixedInput = value; }
    public float MovementDistance { get => movementDistance; set => movementDistance = value; }


    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        nodePrefab = (GameObject)Resources.Load("Prefabs/Node", typeof(GameObject));
        ammoCap = GameManager.AmmoCap;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine( UpdateNextTargets());
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        //gameManager.snakeHeadPosition.transform.position += new Vector3(0f, 0f, 20f) * Time.deltaTime; 
        
    }

    public void AddStartingNodes()
    {
        AddNode(gameManager.InitialsnakeLength);

        for (int i = 0; i < gameManager.nodeList.Count; i++)
        {
            gameManager.nodeList[i].GetComponent<Node>().nextTarget = gameManager.nodeList[i-1].transform.position;
            
        }
        

    }

    public void AddNode(long count)
    {
        
        for (int i = 0; i < count; i++)
        {
            if (gameManager.nodeList.Count == ammoCap)
            {
                UpdateLabel();
            }
            else
            {
                Vector3 spawnPos = new Vector3(gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.x, gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.y, gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.z -1f);
                GameObject newNode = Instantiate(nodePrefab, spawnPos, nodePrefab.transform.rotation);

                gameManager.nodeList.Add(newNode);
            }
        }
    }

    public void RemoveNode()
    {

        Quaternion headRotation = gameManager.nodeList[0].transform.rotation;

        Destroy(gameManager.nodeList[0]);
        gameManager.nodeList.RemoveAt(0);

        gameManager.nodeList[0].AddComponent<SnakeAnimation>();
        gameManager.nodeList[0].GetComponent<SnakeAnimation>().snakeHead = gameManager.nodeList[0];
        gameManager.GetComponent<PlayerController>().sphere = gameManager.snakeHeadPosition;
        gameManager.snakeHead = gameManager.nodeList[0];
        gameManager.mainCamera.GetComponent<CameraController>().player = gameManager.nodeList[0];
        gameManager.nodeList[0].GetComponentInChildren<RectTransform>().rotation = headRotation;

        gameManager.nodeList[0].AddComponent<Rigidbody>();
        gameManager.nodeList[0].GetComponent<Rigidbody>().useGravity = false;
        gameManager.nodeList[0].GetComponentInChildren<Transform>().rotation = headRotation;
        UpdateLabel();

        if (gameManager.nodeList.Count == 0)
        {
            Debug.Log("Game Over!");
        }

    }
    public void UpdateLabel()
    {
        label.text = gameManager.nodeList.Count.ToString();
        label.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
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
                stillCollidingWithABox = true;
                StartCoroutine(BoxCollision(collisionInfo.gameObject));
        }
        else if (collisionInfo.transform.name == "Divider(Clone)")
        {
            Debug.Log("HIT TO DIVIDER! " );
            gameManager.snakeHeadPosition.TryGetComponent<Rigidbody>(out var rb);
            //rb.velocity = new Vector3(0, 0, 0);
            //gameManager.snakeHeadPosition.transform.position = collisionInfo.gameObject.transform.position + new Vector3(0f,0f,1f);
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
                collider.GetComponent<Enemy>().DecreaseSize();
 
            }
            else
            {

            gameManager.nodeList[0].transform.position += new Vector3(0, 0, -1f) * Time.deltaTime;
            //for (int i = 0; i < gameManager.nodeList.Count; i++)
            //{
            //    gameManager.nodeList[i].transform.position += new Vector3(0, 0, -5f) * Time.deltaTime;
            //}


            collider.GetComponent<Enemy>().DecreaseSize();
            }
            RemoveNode();

            yield return new WaitForSeconds(explosionInterval);
        
    }

    public void Animation()
    {
        Vector3 newVector;

        gameManager.nodeList[0].transform.position = gameManager.snakeHeadPosition.transform.position;

        for (int i = 1; i < gameManager.nodeList.Count; i++)
        {
                gameManager.nodeList[i].transform.position = Vector3.MoveTowards(gameManager.nodeList[i].transform.position, gameManager.nodeList[i].GetComponent<Node>().nextTarget, Time.deltaTime * 5000f);
        }
    }

    public IEnumerator UpdateNextTargets()
    {
        while(true)
        {
            gameManager.nodeList[0].transform.position = gameManager.snakeHeadPosition.transform.position;
            gameManager.nodeList[0].GetComponent<Node>().nextTarget = gameManager.nodeList[1].transform.position - new Vector3(0f, 0f, 0f);

            for (int i = 1; i < gameManager.nodeList.Count - 1; i++)
            {
                gameManager.nodeList[i].GetComponent<Node>().nextTarget = gameManager.nodeList[i + 1].transform.position - new Vector3(0f, 0f, 0f);
                
            }

            yield return new WaitForSeconds(0.00833f);
        }
        
    }
}