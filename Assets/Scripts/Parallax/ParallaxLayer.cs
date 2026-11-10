using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;
    [SerializeField] private float imageWidthOffset = 15f;

    private float imageFullWidth;
    private float imageHalfWidth;

    public void CalculateImageWidth()
    {
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        imageHalfWidth = imageFullWidth / 2;
    }

    public void Move(float moveDistance)
    {
        background.position += Vector3.right * moveDistance * parallaxMultiplier;
    }

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        float imageRightEdge = background.position.x + imageHalfWidth - imageWidthOffset;
        float imageLeftEdge = background.position.x - imageHalfWidth + imageWidthOffset;
        if (cameraLeftEdge > imageRightEdge)
            background.position += Vector3.right * imageFullWidth;
        else if (cameraRightEdge < imageLeftEdge)
            background.position -= Vector3.right * imageFullWidth;
    }
}
