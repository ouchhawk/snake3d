using UnityEngine;

public class Box : Enemy
{
    Color[] colorPalette = {
            new Color(0.01f, 0.15f, 0.79f, 0.5f),
            new Color(0.060f, 0.042f, 0.100f, 0.5f),
            new Color(0.150f, 0.106f, 0.900f, 0.5f),
            new Color(0.9f, 0.229f, 0f, 0.15f),
            new Color(0.150f, 0f, 0.900f, 0.8f),
            new Color(0.9f, 0.229f, 0f, 0.5f)
        };

    private void Awake()
    {
        UpdateColor();
    }

    public override void UpdateColor()
    {
        int index = Size % 5;

        Color targetColor = colorPalette[index];
        Color prevColor = index == 0 ? targetColor : colorPalette[index];
        float weight = (Size - 1) % 25 / 25f;

        shapeObject.GetComponent<Renderer>().material.color = Color.Lerp(prevColor, targetColor, weight);
    }

    public override void DecreaseSize()
    {
        if (Size > 1)
        {
            Size--;
            UpdateColor();
            UpdateLabel();
            //transform.localScale = new Vector3(1f, 0.1f * Size, 1f); 
        }
        else if (Size <= 1)
        {
            Destroy(gameObject);
        }
    }
}

