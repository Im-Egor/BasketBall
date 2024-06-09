using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject currentHolder; // Текущий игрок, владеющий мячом
    public bool IsInHands => currentHolder != null; // Мяч в руках, если currentHolder не null

    public void ChangeHolder(GameObject newHolder)
    {
        currentHolder = newHolder;
        if (newHolder != null)
        {
            transform.SetParent(newHolder.transform);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
        else
        {
            transform.SetParent(null);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
