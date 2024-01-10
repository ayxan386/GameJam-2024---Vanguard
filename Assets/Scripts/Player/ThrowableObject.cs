using System.Collections;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    [SerializeField] private PowerUp effectComponent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float lifespan;
    [SerializeField] private float force;
    [SerializeField] private float stirringForce;
    [SerializeField] private float effectDistance;
    [SerializeField] private float stirringDelay = 1.5f;
    private bool allowStirring;
    private bool allowDetection;

    public void Throw(Vector3 dir)
    {
        coll.enabled = true;
        transform.parent = null;
        rb.isKinematic = false;
        rb.AddForce(dir * force, ForceMode.Impulse);
        StartCoroutine(DelayAndSet());
        Destroy(gameObject, lifespan);
    }

    private IEnumerator DelayAndSet()
    {
        yield return null;
        yield return null;
        yield return null;
        allowDetection = true;
        yield return new WaitForSeconds(stirringDelay);
        allowStirring = true;
    }

    private void FixedUpdate()
    {
        if (!allowStirring || !allowDetection) return;
        var minDist = effectDistance;
        Vector3 targetPoint = Vector3.zero;
        foreach (var player in MainGameManager.Instance.Players)
        {
            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < minDist)
            {
                minDist = distance;
                targetPoint = player.transform.position;
            }
        }

        if (minDist < effectDistance)
        {
            var dir = targetPoint - transform.position;
            dir.Normalize();
            rb.AddForce(dir * stirringForce);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground")) return;
        Destroy(gameObject, 0.02f);
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.TryGetComponent(out PlayerController playerController))
            {
                Instantiate(effectComponent, playerController.transform).Use(playerController);
            }
        }
    }
}