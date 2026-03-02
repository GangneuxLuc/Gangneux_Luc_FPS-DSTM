using UnityEngine;

public class SphereCastTest : MonoBehaviour
{
    [Range(0.1f, 1f)] public float sphereCastRadius;
    [Range(1f, 100f)] public float range;
    public LayerMask layerMask;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward * range, out hit, range, layerMask))
        {
            Gizmos.color = Color.green;
            Vector3 sphereCastMidpoint = transform.position + (transform.forward * hit.distance);
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            Gizmos.DrawSphere(hit.point, 0.1f);
            Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 sphereCastMidpoint = transform.position + (transform.forward * (range - sphereCastRadius));
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
        }


     
    }
}
