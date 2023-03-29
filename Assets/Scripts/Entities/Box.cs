using UnityEngine;

public class Box : Enemy
{
    public Box(GameManager gameManager) : base(gameManager)
    {
    }

    public override void SetColor(Color color)
    {
        //Color[] colorPalette = {
        //    new Color(0.0f, 0.016f, 0.011f, 1f),
        //    new Color(0.060f, 0.042f, 0.100f, 1f),
        //    new Color(0.150f, 0.106f, 0.900f, 1f),
        //    new Color(0.9f, 0.229f, 0.203f, 1f)
        //};
        //float[] avgNum = {
        //    (colorPalette[0].r + colorPalette[0].b + colorPalette[0].g) / (3f * 255f),
        //    (colorPalette[1].r + colorPalette[1].b + colorPalette[1].g) / (255f * 3f),
        //    (colorPalette[2].r + colorPalette[2].b + colorPalette[2].g) / (255f * 3f),
        //    (colorPalette[3].r + colorPalette[3].b + colorPalette[3].g) / (255f * 3f)
        //};

        //int index = Mathf.Clamp((Size - 1) / 25, 0, 3);
        //Color targetColor = colorPalette[index];
        //Color prevColor = index == 0 ? targetColor : colorPalette[index - 1];
        //float weight = (Size - 1) % 25 / 25f;

        //shapeObject.GetComponent<Renderer>().material.color = Color.Lerp(prevColor, targetColor, weight);
    }

    public override void DecreaseSize()
    {
        if (Size > 1)
        {
            Size--;
            //UpdateBoxColor();
            UpdateLabel();
            //transform.localScale = new Vector3(1f, 0.1f * Size, 1f); 
        }
        else if (Size <= 1)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateColor()
    {

    }
}

