using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI score;
    private long initialsnakeLength = 99;
    private long currentSnakeLength = 10;

    private static long ammoCap = 100;
    private bool isTimePassing = true;
    private const int rowSize = 1000;
    private const int columnSize = 10;
    private const long currentRow = 0;
    private float edgeLength = 10f;
    private float radius = 1.25f;

    public List<GameObject> nodeList;
    public Camera mainCamera;
    private float leftEdgeCoordinateZ = 200f;
    private float leftEdgeCoordinateX = -30f;

    private Color nodeColor;
    private Color backgroundColor;
    private Color boxBaseColor;
    private Color boxFloorColor, boxCeilColor;

    private List<List<UnitType>> typeDistributionTable = new List<List<UnitType>>();
    private List<List<long>> valueDistributionTable = new List<List<long>>();


    public GameObject snakeHead;
    public GameObject boxPrefab;
    public GameObject foodPrefab;
    public GameObject nodePrefab;
    public GameObject dividerPrefab;
    public GameObject background;
    public GameObject snakeHeadPosition;

    public bool IsTimePassing { get => isTimePassing; set => isTimePassing = value; }
    public Color NodeColor { get => nodeColor; set => nodeColor = value; }
    public Color BackgroundColor { get => backgroundColor; set => backgroundColor = value; }
    public Color BoxFloorColor { get => boxFloorColor; set => boxFloorColor = value; }
    public Color BoxCeilColor { get => boxCeilColor; set => boxCeilColor = value; }
    public long InitialsnakeLength { get => initialsnakeLength; set => initialsnakeLength = value; }
    public long CurrentSnakeLength { get => currentSnakeLength; set => currentSnakeLength = value; }
    public static long AmmoCap { get => ammoCap; set => ammoCap = value; }

    public enum UnitType
    {
        BOX,
        DIVIDER,
        EMPTY,
        FOOD
    }

    private void Awake()
    {
        


    }
    private void Update()
    {
        UpdateScore();
    }

    void Start()
    {
        PopulateUnitDistributionTable();
        DistributeUnits();
        SpawnUnits();
        GenerateGameColors();

        Vector3 spawnPos = new Vector3(-1, 1.25f, 0);
        GameObject newNode = Instantiate(nodePrefab, spawnPos, nodePrefab.transform.rotation);
        nodeList.Add(newNode);

        mainCamera.GetComponent<CameraController>().player = newNode;
        transform.GetComponent<PlayerController>().sphere = snakeHeadPosition;

        newNode.AddComponent<SnakeAnimation>();
        newNode.GetComponent<SnakeAnimation>().snakeHead = newNode;
        newNode.GetComponent<SnakeAnimation>().AddStartingNodes();

    }

    private void UpdateScore()
    {
        if (nodeList.Count == AmmoCap)
        {
            snakeHeadPosition.GetComponentInChildren<TextMeshProUGUI>().text = "max";
        }
        else
        {
            snakeHeadPosition.GetComponentInChildren<TextMeshProUGUI>().text = nodeList.Count.ToString();
        }
    }
    private void PopulateUnitDistributionTable()
    {
        for (int i = 0; i < columnSize; i++)
        {
            typeDistributionTable.Add(new List<UnitType>());
            valueDistributionTable.Add(new List<long>());
            for (int j = 0; j < rowSize; j++)
            {
                typeDistributionTable[i].Add(UnitType.EMPTY);
                valueDistributionTable[i].Add(0);
            }
        }
    }
    private void DistributeUnits()
    {
        int randomNumber;
        // higher number imply less probability (0-100)
        int dividerChance = 85, boxChance = 80, foodChance = 80;
        int[] itemCount = { 0,0,0,0,0,0,0,0,0,0 };


        for (int row = 1; row < rowSize; row++) 
        {
            for (int column = 1; column < columnSize; column++)
            {
                if (column % 2 == 0) // divider column
                {
                    if (typeDistributionTable[column-1][row] != UnitType.BOX)
                    {
                        randomNumber = Random.Range(0, 100);
                        if(typeDistributionTable[column][row - 1] == UnitType.DIVIDER) // increase chance if there is divider previously spawned 
                            randomNumber *= 2;

                        if (randomNumber > dividerChance)
                        {
                            typeDistributionTable[column][row] = UnitType.DIVIDER;
                            itemCount[column]++;
                        }
                    }
                }
                else // box column
                {
                    if(row % 2 == 0)
                    {
                        if (typeDistributionTable[column - 1][row] != UnitType.DIVIDER)
                        {
                            if (typeDistributionTable[column][row - 1] != UnitType.BOX)
                            {
                                randomNumber = Random.Range(0, 110);

                                if (randomNumber > boxChance)
                                {
                                    // select quintet
                                    if (randomNumber > 100 && column == 1 &&
                                        typeDistributionTable[column + 2][row - 1] != UnitType.BOX &&
                                        typeDistributionTable[column + 4][row - 1] != UnitType.BOX &&
                                        typeDistributionTable[column + 6][row - 1] != UnitType.BOX &&
                                        typeDistributionTable[column + 8][row - 1] != UnitType.BOX
                                        )
                                    {
                                        typeDistributionTable[column][row] = UnitType.BOX;
                                        typeDistributionTable[column + 2][row] = UnitType.BOX;
                                        typeDistributionTable[column + 4][row] = UnitType.BOX;
                                        typeDistributionTable[column + 6][row] = UnitType.BOX;
                                        typeDistributionTable[column + 8][row] = UnitType.BOX;
                                        itemCount[column]++;
                                        itemCount[column+2]++;
                                        itemCount[column+4]++;
                                        itemCount[column+6]++;
                                        itemCount[column+8]++;
                                        continue;
                                    }
                                    else
                                    {
                                        if(randomNumber > 90)
                                        {
                                            typeDistributionTable[column][row] = UnitType.BOX;
                                            itemCount[column]++;
                                        }
                                        else
                                        {
                                            typeDistributionTable[column][row] = UnitType.FOOD;
                                        }
                                    }
                                }
                                else if (randomNumber > foodChance)
                                {
                                    typeDistributionTable[column][row] = UnitType.FOOD;
                                }
                            }
                        }
                    }
                }
            }
        }
                Debug.Log("Dividers:" + itemCount[2] + ", " + itemCount[4] + ", " + itemCount[6] + ", " + itemCount[8] + ", ");
                Debug.Log("Boxes:" + itemCount[1] + ", " + itemCount[3] + ", " + itemCount[5] + ", " + itemCount[7] + ", " + itemCount[9] + ", ");
        
    }

    private void SpawnUnits()
    {
        for (int column = 1; column < columnSize; column++)
        {
            for (int row = 1; row < rowSize; row++)
            {
                if(typeDistributionTable[column][row] == UnitType.DIVIDER)
                {
                    GameObject newUnit = Instantiate(dividerPrefab, 
                        new Vector3(-(leftEdgeCoordinateX + (column-column/2) * edgeLength + edgeLength/2), 0, leftEdgeCoordinateZ + row * edgeLength), 
                        dividerPrefab.transform.rotation);
                }
                else if (typeDistributionTable[column][row] == UnitType.FOOD)
                {
                    GameObject newUnit = Instantiate(foodPrefab, 
                        new Vector3(-(leftEdgeCoordinateX + (column - column / 2) * edgeLength), radius, leftEdgeCoordinateZ + row * edgeLength),
                        foodPrefab.transform.rotation);
                    newUnit.GetComponent<Enemy>().Size = Random.Range(1, 10);
                    newUnit.GetComponent<Enemy>().UpdateLabel();
                }
                else if(typeDistributionTable[column][row] == UnitType.BOX)
                {
                    GameObject newUnit = Instantiate(boxPrefab, 
                        new Vector3(-(leftEdgeCoordinateX + (column - column / 2) * edgeLength), 0, leftEdgeCoordinateZ + row * edgeLength),
                        boxPrefab.transform.rotation);
                    newUnit.GetComponent<Enemy>().Size = Random.Range(1, 100);
                    newUnit.GetComponent<Enemy>().UpdateLabel();
                    //newUnit.GetComponent<Transform>().localScale = new Vector3(1f, 0.05f * newUnit.GetComponent<Enemy>().Size, 1f);
                    //newUnit.transform.Find("Cube").GetComponent<Renderer>().material.color = new Color(BoxFloorColor.r + newUnit.GetComponent<Enemy>().Size * 0.01f,
                    //                                                         BoxFloorColor.g + newUnit.GetComponent<Enemy>().Size * 0.01f,
                    //                                                         BoxFloorColor.b + newUnit.GetComponent<Enemy>().Size * 0.01f);
                }
            }
        }
    }
    
    private void GenerateGameColors()
    {
        //float[] colorCoefficients = new float[3];
        //for (int i = 0; i < 3; i++)
        //{
        //    colorCoefficients[i] = Random.Range(0f, 1f);
        //}
        //NodeColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //BackgroundColor = new Color(1 - NodeColor.r, 1 - NodeColor.g, 1 - NodeColor.b);
        //float redFloor = Random.Range(1f, 1f);
        //float greenFloor = Random.Range(0f, 0.5f);
        //float blueFloor = Random.Range(0f, 0.5f);
        //BoxFloorColor = new Color(redFloor, greenFloor, blueFloor);;
        //BoxCeilColor = new Color(redFloor+0.5f, greenFloor + 0.5f, blueFloor + 0.5f);

        //background.GetComponent<SpriteRenderer>().color = BackgroundColor;
        //nodePrefab.GetComponent<SpriteRenderer>().color = nodeColor;

        //snakeHead.GetComponent<Snake>().AddStartingNodes();
        //foodPrefab.GetComponent<Enemy>().prefabShapeObject.transform.GetComponent<SpriteRenderer>().color = nodeColor;

        //GameObject.Find("Circle").transform.GetComponent<SpriteRenderer>().color = nodeColor;
    }
}
