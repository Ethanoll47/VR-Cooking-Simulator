using UnityEngine;

/// <summary>
/// Toggles particle system
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ToggleParticle : MonoBehaviour
{
    public LayerMask m_LayerMask;
    private ParticleSystem currentParticleSystem = null;
    private MonoBehaviour currentOwner = null;
    private Collider flameCollider = null;
    public GameObject flame;
    private Collider[] colliders = null;

    private void Awake()
    {
        currentParticleSystem = GetComponent<ParticleSystem>();
        flameCollider = GetComponent<Collider>();
        Stop();
    }

    void FixedUpdate()
    {
        FlameCollisions();
    }

    void FlameCollisions()
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        colliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);

        // if (colliders.Length != 0)
        // {
        //     if (currentParticleSystem.isPlaying == true)
        //     {
        //         foreach (Collider collider in colliders)
        //         {
        //             collider.gameObject.GetComponent<CookingController>().StartCooking();
        //         }
        //     }
        //     else 
        //     {
        //         foreach (Collider collider in colliders)
        //     {
        //         collider.gameObject.GetComponent<CookingController>().StopCooking();
        //     }
        //     }
        // }

        if (colliders.Length != 0 && currentParticleSystem.isPlaying == false)
        {   
            foreach (Collider collider in colliders)
            {
                if(collider.gameObject.GetComponent<CookingController>().isBurning == true)
                {
                    collider.gameObject.GetComponent<CookingController>().StopCooking();
                }
            }
        }
    }

    public void Play()
    {
        currentParticleSystem.Play();
        flameCollider.enabled = true;
    }

    public void Stop()
    {
        currentParticleSystem.Stop();
        flameCollider.enabled = false;
    }

    public void PlayWithExclusivity(MonoBehaviour owner)
    {
        if (currentOwner == null)
        {
            currentOwner = this;
            Play();
        }
    }

    public void StopWithExclusivity(MonoBehaviour owner)
    {
        if (currentOwner == this)
        {
            currentOwner = null;
            Stop();
        }
    }

    private void OnValidate()
    {
        if (currentParticleSystem)
        {
            ParticleSystem.MainModule main = currentParticleSystem.main;
            main.playOnAwake = false;
        }
    }
}
