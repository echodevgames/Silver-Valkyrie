using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float bounceForce = 800f;
    public AudioClip bounceSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector3 direction = (other.transform.position - transform.position).normalized;
                rb.AddForce(direction * bounceForce, ForceMode.Impulse);

                if (audioSource && bounceSound)
                    audioSource.PlayOneShot(bounceSound);
            }
        }
    }
}
