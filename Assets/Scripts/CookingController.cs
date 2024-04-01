using System.Collections;
using UnityEngine;

public class CookingController : MonoBehaviour
{
    private Color burntColor = new(0.1886792f, 0.09004446f, 0.02046992f, 0.0f);
    private Material meatMaterial;
    private Material[] materials;

    public bool isBurning = false;
    private IEnumerator colorCoroutine;
    private AudioSource audioSource;
    private readonly int cookingLayerMask = 6;
    
    

    // Start is called before the first frame update
    void Start()
    {
        meatMaterial = GetComponent<Renderer>().material;
        materials = GetComponent<Renderer>().materials;
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate() 
    {
        //Check if object is currently cooking
        if (isBurning)
        {
            colorCoroutine = LerpColor(meatMaterial.color, burntColor, 15.0f);
            StartCoroutine(colorCoroutine);
        }
        else
        {
            if (colorCoroutine != null)
            {
                //Stop cooking
                StopAllCoroutines();
            }
        }
    }

    IEnumerator LerpColor(Color startColor, Color endColor, float duration)
    {
        float time = 0.0f;
        while (time < duration)
        {
            // Calculate the lerp value.
            float lerpValue = time / duration;

            // Lerp the base map color of all the materials in the material list
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = Color.Lerp(startColor, endColor, lerpValue);
            }

            // Increment the time.
            time += Time.deltaTime;
            yield return null;
        }

        // Set the base map color of all the materials in the material list to the end color
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = endColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if object touches fire 
        if (other.gameObject.layer == cookingLayerMask)
        {
            StartCooking();
        }
    }

    public void StopCooking()
    {
        isBurning = false;
        audioSource.Stop();
    }

    public void StartCooking()
    {
        isBurning = true;
        audioSource.Play();
    }

    void OnTriggerExit(Collider other)
    {
        // Check if object leaves the fire
        if (other.gameObject.layer == cookingLayerMask)
        {
            StopCooking();
        }
    }

}
