using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private int size;
    private float maxBoxSize = 50f;
    private Color color;
    public GameObject shapeObject;
    public TMPro.TextMeshProUGUI label;

    public int Size { get => size; set => size = value; }
    public Color Color { get => color; set => color = value; }

    private void Start()
    {
    }

    public void UpdateFoodColor()
    {
        shapeObject.transform.Find("Sphere").GetComponent<Material>().color =
            GameObject.Find("GameManager").GetComponent<GameManager>().NodeColor;
    }

    public void UpdateBoxColor()
    {
        Color[] colorPalette = { new Color(0.0f, 0.016f, 0.011f, 1f), new Color(0.060f, 0.042f, 0.100f, 1f), new Color(0.150f, 0.106f, 0.900f, 1f), new Color(0.9f, 0.229f, 0.203f, 1f) };
        float[] avgNum = { (colorPalette[0].r + colorPalette[0].b + colorPalette[0].g)/(3f * 255f) ,
                (colorPalette[1].r + colorPalette[1].b + colorPalette[1].g)/ ( 255f*3f) ,
                (colorPalette[2].r + colorPalette[2].b + colorPalette[2].g)/ ( 255f*3f) ,
                (colorPalette[3].r + colorPalette[3].b + colorPalette[3].g)/ ( 255f*3f) };

        float boxNormalized = Convert.ToSingle(Size) / 10000f;
        if (Size > 75)
        {
            shapeObject.GetComponent<Renderer>().material.color = new Color(colorPalette[1].r - (Size-75) *((colorPalette[1].r - colorPalette[0].r)/25f),
                                                                            colorPalette[1].g - (Size - 75) * ((colorPalette[1].g - colorPalette[0].g) / 25f),
                                                                            colorPalette[1].b - (Size - 75) * ((colorPalette[1].b - colorPalette[0].b) / 25f));
        }
        else if (Size > 50)
        {
            shapeObject.GetComponent<Renderer>().material.color = new Color(colorPalette[2].r - (Size - 50) * ((colorPalette[2].r - colorPalette[1].r) / 25f),
                                                                           colorPalette[2].g - (Size - 50) * ((colorPalette[2].g - colorPalette[1].g) / 25f),
                                                                           colorPalette[2].b - (Size -50) * ((colorPalette[2].b - colorPalette[1].b) / 25f));
        }
        else if (Size > 0)
        {
            shapeObject.GetComponent<Renderer>().material.color = new Color(colorPalette[3].r - (Size) * ((colorPalette[3].r - colorPalette[2].r) / 50f),
                                                                           colorPalette[3].g - (Size) * ((colorPalette[3].g - colorPalette[2].g) / 50f),
                                                                           colorPalette[3].b - (Size) * ((colorPalette[3].b - colorPalette[2].b) / 50f));
        }
        else if (Size > 0)
        {
            shapeObject.GetComponent<Renderer>().material.color = new Color(colorPalette[3].r + Size * (1f  / 25f),
                                                                           colorPalette[3].g + Size * (1f  / 25f),
                                                                           colorPalette[3].b + Size * (1f  / 25f));
        }
        Debug.Log(Size / 50f + "vs" + avgNum[3] + " vs " + avgNum[2] + " vs " + avgNum[1] + " vs " + avgNum[0] );

    }
    public void UpdateLabel()
    {
        label.text = size.ToString();
    }
    public void DecreaseSize()
    {
        if (Size > 1)
        {
            Size--;
            UpdateBoxColor();
            UpdateLabel();
            //transform.localScale = new Vector3(1f, 0.1f * Size, 1f); 
        }
        else if (Size <= 1 )
        {
            Destroy(gameObject);
        }
    }
}