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
    private float horizontalMovementSpeed = 50f, verticalMovementSpeed = 50f;
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

    // Start is called before the first frame update

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        nodePrefab = (GameObject)Resources.Load("Prefabs/Node", typeof(GameObject));
        ammoCap = GameManager.AmmoCap;
        //AddStartingNodes();
    }
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        //AddStartingNodes();
        //oldSnakePositionX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void FixedUpdate()
    {
        transform.GetComponent<Rigidbody>().MovePosition(transform.position + Vector3.forward * Time.deltaTime * 35f);
        //transform.Translate(Vector3.forward * Time.deltaTime * 35f);
        //gameManager.snakeHead.GetComponent<Rigidbody>().velocity = (Vector3.forward * 35f * Time.deltaTime);
        snakeAnimation();

        //if (oldSnakePositionX != transform.position.x)
        //{

        //oldSnakePositionX = transform.position.x;
        //}
    }

    public void AddStartingNodes()
    {
        AddNode(gameManager.InitialsnakeLength);

        for (int i = 0; i < gameManager.nodeList.Count; i++)
        {
            gameManager.nodeList[i].GetComponent<Node>().previousPosition = gameManager.nodeList[i].transform.position;
            
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
                Vector3 spawnPos = new Vector3(gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.x, gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.y, gameManager.nodeList[gameManager.nodeList.Count - 1].transform.position.z - 2.5f);
                GameObject newNode = Instantiate(nodePrefab, spawnPos, nodePrefab.transform.rotation);

                //newNode.GetComponent<SpriteRenderer>().color = gameManager.GetComponent<GameManager>().NodeColor;
                gameManager.nodeList.Add(newNode);
                //UpdateLabel();
            }

        }
    }

    public void RemoveNode()
    {
        for (int i = gameManager.nodeList.Count - 1; i > 0; i--)
        {
            gameManager.nodeList[i].transform.position = Vector3.MoveTowards(gameManager.nodeList[i].transform.position, gameManager.nodeList[i - 1].transform.position, 10 * Time.deltaTime);
            //gameManager.nodeList[i].transform.rotation = gameManager.nodeList[i - 1].transform.rotation;
        }

        //for (int i = gameManager.nodeList.Count - 1; i > 0; i--)
        //{
        //    gameManager.nodeList[i].transform.position -= new Vector3(0,0, 1f);
            
        //}

        Quaternion headRotation = gameManager.nodeList[0].transform.rotation;

        if (gameManager.nodeList.Count > 1)
        {
            gameManager.GetComponent<GameManager>().snakeHeadPosition.transform.position = Vector3.MoveTowards(gameManager.GetComponent<GameManager>().snakeHeadPosition.transform.position,
                gameManager.nodeList[1].transform.position, 1000 * Time.deltaTime);
        }

        Destroy(gameManager.nodeList[0]);
        gameManager.nodeList.RemoveAt(0);

        gameManager.nodeList[0].AddComponent<SnakeAnimation>();
        gameManager.nodeList[0].GetComponent<SnakeAnimation>().snakeHead = gameManager.nodeList[0];
        gameManager.GetComponent<PlayerController>().sphere = gameManager.snakeHeadPosition;
        gameManager.snakeHead = gameManager.nodeList[0];
        gameManager.mainCamera.GetComponent<CameraController>().player = gameManager.nodeList[0];
        gameManager.nodeList[0].GetComponentInChildren<RectTransform>().rotation = headRotation;

        gameManager.nodeList[0].transform.position = gameManager.nodeList[0].transform.position + new Vector3(0, 0, -5f);


        //Instantiate(explositionParticle, transform.position, explositionParticle.transform.rotation);
        UpdateLabel();

        //if(gameManager.nodeList.Count <= 1)
        //{
        //    player.SetActive(false) ;
        //    Debug.Log("GAME OVER !!!!!!!!!!!!!!");
        //}

        if (gameManager.nodeList.Count == 0)
        {
            Debug.Log("Game Over!");
        }

    }
    public void UpdateLabel()
    {
        if (gameManager.nodeList.Count == ammoCap)
        {
            gameManager.snakeHeadPosition.GetComponentInChildren<TextMeshProUGUI>().text = "max";
        }
        else
        {
            label.text = gameManager.nodeList.Count.ToString();
        }
        //Debug.Log(gameManager.nodeList[1].transform.GetChild(1).transform.GetChild(0).transform.GetComponent<TMPro.TextMeshProUGUI>().text);

        for (int i = gameManager.nodeList.Count - 1; i > 0; i--)
        {
            gameManager.nodeList[i].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "helo";
        }

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
            //gameManager.GetComponent<GameManager>().IsTimePassing = false;
            //ParticleSystem particle = Instantiate(explositionParticle, transform.position, explositionParticle.transform.rotation);
            //if (!stillCollidingWithABox)
            //{
                stillCollidingWithABox = true;
                StartCoroutine(BoxCollision(collisionInfo.gameObject));
            //}
            //else
            //{
            //}
        }
        else if (collisionInfo.transform.name == "Divider(Clone)")
        {
            Debug.Log("HIT TO DIVIDER! " );
            gameManager.snakeHeadPosition.TryGetComponent<Rigidbody>(out var rb);
            rb.velocity = new Vector3(0, 0, 0);
            gameManager.snakeHeadPosition.transform.position = collisionInfo.gameObject.transform.position + new Vector3(100f,10f,10f);
            //if (transform.position.x < collisionInfo.gameObject.transform.position.x)
            //{
            //    transform.position = (new Vector3(collisionInfo.gameObject.transform.position.x - 1f, transform.position.y, transform.position.z));
            //}
            //else
            //{
            //    transform.position = (new Vector3(collisionInfo.gameObject.transform.position.x + 1f, transform.position.y, transform.position.z));
            //}
        }
    }

    public void OnCollisionEnter(Collision collisionInfo)
    {

    }

    public void OnTriggerExit()
    {
        if (gameManager.GetComponent<GameManager>().IsTimePassing == false)
        {
            gameManager.GetComponent<GameManager>().IsTimePassing = true;
            //StartCoroutine(gameManager.SpawnCoroutine());
            stillCollidingWithABox = false;
        }
    }

    //public IEnumerator MakeStraight()
    //{
    //    Vector2 movementDirection;
    //    while (IsTimePassing && controlMode == 1)
    //    {
    //        //if (Input.GetAxis("Horizontal") == 0)
    //        if (horizontalInput == 0)
    //        {
    //            for (int j = 0; j < 20; j++)
    //            {
    //                for (int i = 1; i < gameManager.nodeList.Count; i++)
    //                {
    //                    movementDirection = new Vector2((gameManager.nodeList[i - 1].GetComponent<Node>().transform.position.x - gameManager.nodeList[i].transform.position.x), (gameManager.nodeList[i - 1].GetComponent<Node>().transform.position.y - gameManager.nodeList[i].transform.position.y - 1f));
    //                    gameManager.nodeList[i].transform.Translate(movementDirection * translateAmount, Space.World);
    //                }
    //            }
    //        }
    //        yield return new WaitForSeconds(0.02f);
    //    }
    //}
    public IEnumerator BoxCollision(GameObject collider)
    {
        int colliderInitialSize = collider.GetComponent<Enemy>().Size;
        //Debug.Log(colliderInitialSize);
        //ParticleSystem particle=Instantiate(explositionParticle, transform.position, explositionParticle.transform.rotation);

        //if (isPoweredUp)
        //{
        //    Destroy(collider);
        //    scoreCounter += collider.GetComponent<Enemy>().Size;
        //    scoreText.text = scoreCounter.ToString();
        //    yield return "end";
        //}

        //while (stillCollidingWithABox)
        //{
            //particle = Instantiate(explositionParticle, transform.position, explositionParticle.transform.rotation);

            if (collider.GetComponent<Enemy>().Size < 1)
            {
                Destroy(collider);
                //if (colliderInitialSize == 25)
                //{
                //    StartCoroutine(flashingLights(powerUpDuration));
                //    isPoweredUp = true;

                //    nodePrefab.GetComponent<Enemy>().Speed *= powerupSpeedMultiplier;
                //}
            }
            else if (gameManager.nodeList.Count <= 1)
            {
                isTimePassing = false;
                snakeHead.SetActive(false);
                //playingUI.SetActive(false);
                //gameOverUI.SetActive(true);
                //gameOverScore.text = scoreCounter.ToString();
                //gameManager.enabled = false;
            }
            else
            {
                collider.GetComponent<Enemy>().DecreaseSize();
            }
            RemoveNode();
            //scoreCounter++;
            //scoreText.text = scoreCounter.ToString();

            yield return new WaitForSeconds(explosionInterval);
        //}
    }

    private void snakeAnimation()
    {
        Vector3 newVector;

        gameManager.nodeList[0].transform.position = gameManager.GetComponent<GameManager>().snakeHeadPosition.transform.position;

        for (int i = 1; i < gameManager.nodeList.Count; i++)
        {
            newVector = (gameManager.nodeList[i].transform.position - gameManager.nodeList[i - 1].transform.position - new Vector3(0f, 0f, 0f)).normalized;
            gameManager.nodeList[i].transform.position = gameManager.nodeList[i - 1].transform.position + newVector * 2.5f;
            //gameManager.nodeList[i].transform.position = Vector3.MoveTowards(gameManager.nodeList[i].transform.position, gameManager.nodeList[i - 1].transform.position, 25f*Time.deltaTime);
        }

        //for (int i = 0; i < gameManager.nodeList.Count; i++)
        //{
        //    gameManager.nodeList[i].GetComponent<Node>().previousPosition = gameManager.nodeList[i].transform.position;
        //}
    }

}
