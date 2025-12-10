using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeedX;
    public float scrollSpeedY;

    public MeshRenderer meshRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        meshRenderer.material.mainTextureOffset = new Vector2(Time.realtimeSinceStartup * scrollSpeedX,
            Time.realtimeSinceStartup * scrollSpeedY);
    }
}
