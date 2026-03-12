using UnityEngine;

public class planeHiderAPI : MonoBehaviour
{
    public GameObject frontPlane;
    public GameObject backPlane;
    public GameObject leftPlane;
    public GameObject rightPlane;
    public GameObject topPlane;
    public GameObject bottomPlane;

    void Start()
    {
        SetPlaneInactive(frontPlane);
        SetPlaneInactive(backPlane);
        SetPlaneInactive(leftPlane);
        SetPlaneInactive(rightPlane);
        SetPlaneInactive(topPlane);
        SetPlaneInactive(bottomPlane);
    }

    void SetPlaneInactive(GameObject plane)
    {
        if (plane != null)
        {
            plane.SetActive(false);
        }
    }
}