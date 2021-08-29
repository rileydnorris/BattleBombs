using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 cameraOffset = new Vector3(0, 0, -1);

    void Update()
    {
        transform.position = player.transform.position + cameraOffset;
    }
}
