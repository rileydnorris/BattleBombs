using System.Collections;
using UnityEngine;

public class ArcRenderer : MonoBehaviour
{
    public LineRenderer line;
    private int _obstacleLayerMask;

    void Awake()
    {
        _obstacleLayerMask = LayerMask.GetMask("Obstacles");
    }

    public IEnumerator RenderAim(float angle)
    {
        line.positionCount = 2;
        SetLinePositions(angle);
        yield return null;
    }

    private void SetLinePositions(float angle)
    {
        // Set first point
        line.SetPosition(0, new Vector3(0, 0, 1));

        // Set second point by detecting if any hits occur
        var startPointV2 = new Vector2(transform.position.x, transform.position.y);
        Vector3 rotatedVector = Quaternion.Euler(Vector3.forward * angle) * new Vector3(2, 0, 0);
        Vector3 endPointV3 = transform.position + rotatedVector;
        Vector2 endPointV2 = new Vector2(endPointV3.x, endPointV3.y);

        var hit = Physics2D.Linecast(startPointV2, endPointV2, _obstacleLayerMask);
        if (hit)
        {
            line.transform.eulerAngles = (Vector3.forward);
            var localSpace = line.transform.InverseTransformPoint(hit.point.x, hit.point.y, 1);
            line.SetPosition(1, new Vector3(localSpace.x, localSpace.y, 1));
        }
        else
        {
            line.SetPosition(1, new Vector3(5f, 0, 1));
            line.transform.eulerAngles = (Vector3.forward * angle);
        }
    }

    public void SetAngle(float angle)
    {
        StartCoroutine(RenderAim(angle));
    }
}