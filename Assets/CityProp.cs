using UnityEngine;

public class CityProp : MonoBehaviour
{
    private void Start()
    {
        GameObject gameObject = GetComponentInChildren<MeshRenderer>().gameObject;

        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
    }
}