using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FacilitySensor : MonoBehaviour
{
    // 현재 감지된 구조물
    public GameObject CurrentFacility { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Facility"))
            CurrentFacility = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Facility") && other.gameObject == CurrentFacility)
            CurrentFacility = null;
    }
}
