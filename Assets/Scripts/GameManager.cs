using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

public class Cell : MonoBehaviour
{
    private UnitType unitType;
    private CellType cellType; 
    private long value;

    public Cell(CellType _cellType, UnitType _unitType, long _value)
    {
        cellType = _cellType;
        unitType = _unitType;
        value = _value;
    }

    public UnitType UnitType { get => unitType; set => unitType = value; }
    public CellType CellType { get => cellType; set => cellType = value; }
    public long Value { get => value; set => this.value = value; }
}

public class GameManager : MonoBehaviour
{
    private const int rowSize = 100, columnSize = 10;
    private float edgeLength = 10f,radius = 1.25f, leftEdgeCoordinateZ = 200f, leftEdgeCoordinateX = -30f;
    private long initialSnakeLength = SessionInformation.snakeLength;
    public TextMeshProUGUI score;
    public Camera mainCamera;
    public GameObject snakeHead, boxPrefab, foodPrefab, nodePrefab, dividerPrefab, background, gameOverUI, finishLinePrefab, finishLine;
    private Color nodeColor,backgroundColor,boxBaseColor,boxFloorColor, boxCeilColor;
    private List<List<Cell>> cellGrid;
    private bool isGameOver=false, isStageClear=false;
    private AudioManager audioManager;

    public long InitialSnakeLength { get => initialSnakeLength; set => initialSnakeLength = value; }
    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
    public bool IsStageClear { get => isStageClear; set => isStageClear = value; }

    void Start()
    {
        PopulateUnitDistributionTable();
        DistributeUnits();
        SpawnUnits();
    }

    private void Awake()
    {
        audioManager =  GameObject.FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
    }

        private void PopulateUnitDistributionTable()
    {
        cellGrid = new List<List<Cell>>();
        long value = 0;
        UnitType unitType = UnitType.EMPTY;
        CellType cellType = CellType.NONE;

        for (int rowIndex = 0; rowIndex < rowSize; rowIndex++)
        {
            cellGrid.Add(new List<Cell>());
            for (int columnIndex = 0; columnIndex < columnSize; columnIndex++)
            {
                cellGrid[rowIndex].Add(new Cell(cellType, unitType, value));
            }
        }

        for (int rowIndex = 0; rowIndex < rowSize; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columnSize; columnIndex++)
            {
                cellType = CellType.NONE;
                if (rowIndex % 2 == 0) //divider row
                {
                    if (columnIndex % 2 == 0) //divider column
                    {
                        cellType = CellType.DIVIDER;
                    }
                }
                else //box row
                {
                    if (columnIndex % 2 == 1) //box column
                    {
                        cellType = CellType.BOX;
                    }
                }
                cellGrid[rowIndex][columnIndex].CellType=cellType;
            }
        }
    }

    private void DistributeUnits()
    {
        System.Random random = new System.Random(); 
        Double rowRandomDouble, colRandomDouble;
        double dividerChance = 0.33f, boxChance = 0.33f, foodChance = 0.5f;

        // Row: {DIV,BOX,DIV,BOX,DIV,BOX,DIV,BOX,DIV}
        int[] itemCount = { 0,0,0,0,0,0,0,0,0,0 };

        for (int row = 1; row < rowSize; row++)
        {
            Thread.Sleep(1); rowRandomDouble = random.NextDouble();
            for (int column = 1; column < columnSize; column++)
            {
                Thread.Sleep(1); colRandomDouble = random.NextDouble();
                if (cellGrid[row][column].CellType == CellType.DIVIDER)
                {
                    if (colRandomDouble < dividerChance)
                    {
                        cellGrid[row][column].UnitType = UnitType.DIVIDER;
                        itemCount[column]++;
                    }
                    else if (colRandomDouble < foodChance)
                    {
                        cellGrid[row][column].UnitType = UnitType.FOOD;
                    }
                }
                else if (cellGrid[row][column].CellType == CellType.BOX)
                {
                    if (colRandomDouble < boxChance)
                    {
                        // select quintet
                        if (colRandomDouble < boxChance / 5 && column == 1)
                        {
                            cellGrid[row][column].UnitType = UnitType.BOX;
                            cellGrid[row][column+2].UnitType = UnitType.BOX;
                            cellGrid[row][column+4].UnitType = UnitType.BOX;
                            cellGrid[row][column+6].UnitType = UnitType.BOX;
                            cellGrid[row][column+8].UnitType = UnitType.BOX;
                            itemCount[column]++;
                            itemCount[column + 2]++;
                            itemCount[column + 4]++;
                            itemCount[column + 6]++;
                            itemCount[column + 8]++;
                            continue;
                        }
                        else if (colRandomDouble < boxChance)
                        {
                            cellGrid[row][column].UnitType = UnitType.BOX;
                            itemCount[column]++;
                        }
                        else if (colRandomDouble < foodChance)
                        {
                            cellGrid[row][column].UnitType = UnitType.FOOD;
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
        Vector3 lastPosition = new Vector3();
        for (int column = 1; column < columnSize; column++)
        {
            for (int row = 1; row < rowSize; row++)
            {
                if(cellGrid[row][column].UnitType == UnitType.DIVIDER)
                {
                    lastPosition = new Vector3(-(leftEdgeCoordinateX + (column - column / 2) * edgeLength + edgeLength / 2), 0, leftEdgeCoordinateZ + row * edgeLength);
                    GameObject newUnit = Instantiate(dividerPrefab, lastPosition, dividerPrefab.transform.rotation);
                }
                else if (cellGrid[row][column].UnitType == UnitType.FOOD)
                {
                    lastPosition = new Vector3(-(leftEdgeCoordinateX + (column - column / 2) * edgeLength), radius, leftEdgeCoordinateZ + row * edgeLength);
                    GameObject newUnit = Instantiate(foodPrefab, lastPosition, foodPrefab.transform.rotation);
                    newUnit.GetComponent<Enemy>().Size = UnityEngine.Random.Range(1, 15);
                    newUnit.GetComponent<Enemy>().UpdateLabel();

                }
                else if(cellGrid[row][column].UnitType == UnitType.BOX)
                {
                    lastPosition = new Vector3(-(leftEdgeCoordinateX + (column - column / 2) * edgeLength), 0, leftEdgeCoordinateZ + row * edgeLength);
                    GameObject newUnit = Instantiate(boxPrefab, lastPosition, boxPrefab.transform.rotation);
                    newUnit.GetComponent<Enemy>().Size = UnityEngine.Random.Range(1, 50);
                    newUnit.GetComponent<Enemy>().UpdateLabel();
                }
            }
        }
        finishLine = Instantiate(finishLinePrefab,lastPosition + new Vector3(20f, 0f, 100f), finishLinePrefab.transform.rotation);
    }

    public IEnumerator BlinkText(Transform text)
    {
        while(true)
        {
            if (text.gameObject.active)
            {
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    static void AutoSeedRandoms()
    {
        Thread.Sleep(1);
        System.Random autoRand = new System.Random();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        audioManager.PlayGameOver();
        //mainCamera.gameObject.SetActive(false);
        snakeHead.GetComponent<Snake>().label.enabled = false;
        snakeHead.GetComponent<PlayerController>().enabled = false;
        snakeHead.GetComponent<Snake>().gameObject.SetActive(false);
        GetComponent<Snake>().gameObject.SetActive(false);
        snakeHead.GetComponent<Canvas>().GetComponentInChildren<TMPro.TextMeshProUGUI>().enabled = false;
        snakeHead.active = false;
    }
}