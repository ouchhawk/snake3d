using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;
    private const int rowSize = 30, columnSize = 10;
    private float edgeLength = 10f,radius = 1.25f, leftEdgeCoordinateZ = 200f, leftEdgeCoordinateX = -30f;
    private long initialsnakeLength = 15;
    private static long ammoCap = 100;
    public TextMeshProUGUI score;
    public Camera mainCamera;
    public GameObject snakeHead, boxPrefab, foodPrefab, nodePrefab, dividerPrefab, background, gameOverUI;
    private Color nodeColor,backgroundColor,boxBaseColor,boxFloorColor, boxCeilColor;
    private List<List<UnitType>> typeDistributionTable = new List<List<UnitType>>();
    private List<List<long>> valueDistributionTable = new List<List<long>>();
    public List<GameObject> nodeList;

    public static long AmmoCap { get => ammoCap; set => ammoCap = value; }

    private void Awake()
    {
        //mainCamera = Camera.main;
    }
    private void Update()
    {
        UpdateSizeText();
    }

    void Start()
    {
        PopulateUnitDistributionTable();
        DistributeUnits();
        SpawnUnits();
        Vector3 spawnPos = new Vector3(0, 1.25f, 0);
        GameObject newNode = Instantiate(nodePrefab, spawnPos, nodePrefab.transform.rotation);
        nodeList.Add(newNode);
        SetSnakeHead(newNode);
    }

    private void SetSnakeHead(GameObject newNode)
    {
        newNode.AddComponent<SnakeAnimation>();
        newNode.GetComponent<SnakeAnimation>().snakeHead = newNode;
        newNode.GetComponent<SnakeAnimation>().AddNode(initialsnakeLength); 
        mainCamera.GetComponent<CameraController>().player = newNode;
        transform.GetComponent<PlayerController>().sphere = snakeHead;
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
        System.Random random = new System.Random();
        Double randomRowDouble, randomColDouble;
        double dividerChance = 0.33f, boxChance = 0.33f, foodChance = 0.5f;

        // Row: {DIV,BOX,DIV,BOX,DIV,BOX,DIV,BOX,DIV}
        int[] itemCount = { 0,0,0,0,0,0,0,0,0,0 };

        for (int row = 1; row < rowSize; row++)
        {
            Thread.Sleep(1); randomRowDouble = random.NextDouble();
            for (int column = 1; column < columnSize; column++)
            {
                Thread.Sleep(1); randomColDouble = random.NextDouble();
                if (row % 2 == 0 || randomRowDouble > 0.75f) //divider row
                {
                    if (column % 2 == 0) // divider column
                    {
                        if (randomColDouble < dividerChance)
                        {
                            typeDistributionTable[column][row] = UnitType.DIVIDER;
                            itemCount[column]++;
                        }
                        else if (randomColDouble < foodChance)
                        {
                            typeDistributionTable[column][row] = UnitType.FOOD;
                        }
                    }
                }
                else // box row
                {
                    if (column % 2 == 1) //box column
                    {
                        if (randomColDouble < boxChance)
                        {
                            // select quintet
                            if (randomColDouble < boxChance / 5 && column == 1 &&
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
                                itemCount[column + 2]++;
                                itemCount[column + 4]++;
                                itemCount[column + 6]++;
                                itemCount[column + 8]++;
                                continue;
                            }
                            else if (randomColDouble < boxChance)
                            {
                                typeDistributionTable[column][row] = UnitType.BOX;
                                itemCount[column]++;
                            }
                            else if (randomColDouble < foodChance)
                            {
                                typeDistributionTable[column][row] = UnitType.FOOD;
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
                    newUnit.GetComponent<Enemy>().Size = UnityEngine.Random.Range(1, 100);
                    newUnit.GetComponent<Enemy>().UpdateLabel();

                }
                else if(typeDistributionTable[column][row] == UnitType.BOX)
                {
                    GameObject newUnit = Instantiate(boxPrefab, 
                        new Vector3(-(leftEdgeCoordinateX + (column - column / 2) * edgeLength), 0, leftEdgeCoordinateZ + row * edgeLength),
                        boxPrefab.transform.rotation);
                    newUnit.GetComponent<Enemy>().Size = UnityEngine.Random.Range(1, 100);
                    newUnit.GetComponent<Enemy>().UpdateLabel();
                }
            }
        }
    }

    private void UpdateSizeText()
    {
        snakeHead.GetComponentInChildren<TextMeshProUGUI>().text = nodeList.Count.ToString();
    }

    public IEnumerator BlinkText(Transform text)
    {
        yield return new WaitForSeconds(1f);
    }

    static void AutoSeedRandoms()
    {
        Thread.Sleep(1);
        System.Random autoRand = new System.Random();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        //mainCamera.gameObject.SetActive(false);
        snakeHead.GetComponent<SnakeAnimation>().gameObject.SetActive(false);
        snakeHead.GetComponent<SnakeAnimation>().gameObject.SetActive(false);
    }
}