using UnityEngine;

public class Particles : MonoBehaviour
{
    void Awake()
    {
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
            TutorialManager.registerParticle(ps);
    }
}
