using Oculus.Interaction;
using UnityEngine;

public class Knob : MonoBehaviour
{
    public GameObject flame;
    private ToggleParticle toggleParticle;
    public OneGrabRotateTransformer rotation;
    private float currentRotation;
    public GameObject knob;


    void Start()
    {
        currentRotation = knob.transform.localRotation.y;
        toggleParticle = flame.GetComponent<ToggleParticle>();
    }

    void FixedUpdate()
    {
        currentRotation = knob.transform.localRotation.y;
        if (currentRotation >= 0.4f)
        {
            toggleParticle.Play();
        }
        else
        {
            toggleParticle.Stop();
        }
    }
}
