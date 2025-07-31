using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public int speed;

    public float time = 2;

    private void Update()
    {

    }

    private void OnMove()
    {
        World.instance.GenerateWorld(Vector3Int.RoundToInt(cam.transform.position));
    }
}