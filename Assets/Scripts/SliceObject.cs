using UnityEngine;
using EzySlice;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class SliceObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;
    public GameObject foodPrefab;
    public AudioSource cuttingSound;
    public PhysicMaterial sticky;
    public GameObject gloveOnText;

    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if (hasHit)
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
    }

    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);
        Renderer renderer = target.GetComponent<Renderer>();
        Material crossSectionMaterial = renderer.material;
        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull, target);
            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull, target);
            //Play slicing audio
            cuttingSound.Play();
            Destroy(target);
        }
    }

    public void SetupSlicedComponent(GameObject slicedObject, GameObject target)
    {
        AudioSource audioSource = slicedObject.AddComponent<AudioSource>();
        audioSource.clip = target.GetComponent<AudioSource>().clip;
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.mass = 50;
        rb.drag = 10;
        rb.position = target.GetComponent<Rigidbody>().position;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.material = sticky;
        slicedObject.AddComponent<CookingController>();
        Grabbable grabbable = slicedObject.AddComponent<Grabbable>();
        HandGrabInteractable hgb = slicedObject.AddComponent<HandGrabInteractable>();
        hgb.PointableElement = grabbable;
        hgb.Rigidbody = rb;
        ForceManager fm = slicedObject.AddComponent<ForceManager>();
        fm.weight = 200;
        slicedObject.layer = target.layer;
        slicedObject.tag = "GB";
        if (gloveOnText.activeInHierarchy == true)
        {
            grabbable.includeInExperiment = true;
        }
        else
        {
            grabbable.includeInExperiment = false;
        }
    }
}
