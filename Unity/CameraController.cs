using UnityEngine;

public class CameraController : MonoBehaviour
{
    public new bool enabled;

    public float moveSpeed = 5f;
    public float zoomSensitivity= 10f;

    public float minZoom = 1f;
    public float maxZoom = 100f;

    void Update()
    {
        if (!enabled)
        {
            return;
        }

        float zoom = transform.localPosition.z;

        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += Vector3.up * moveSpeed * UnityEngine.Time.deltaTime * -zoom;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += Vector3.down * moveSpeed * UnityEngine.Time.deltaTime * -zoom;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += Vector3.left * moveSpeed * UnityEngine.Time.deltaTime * -zoom;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += Vector3.right * moveSpeed * UnityEngine.Time.deltaTime * -zoom;
        }

        zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, -maxZoom, -minZoom);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zoom);

    }

    public void CenterTo(Vector2 point)
    {
        transform.localPosition = new Vector3(point.x, point.y, transform.localPosition.z);
    }
}
