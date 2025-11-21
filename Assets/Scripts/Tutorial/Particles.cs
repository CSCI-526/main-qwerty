using UnityEngine;

public class Particles : MonoBehaviour
{
    GameManager gameManager => FindFirstObjectByType<GameManager>();

    void Awake()
    {
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
            TutorialManager.registerParticle(ps);
    }

    private void Update()
    {
        if (transform.parent != null)
        {
            transform.position = gameManager.ScreenToWorldSpace(transform.parent.position);
        }
    }
}
