using UnityEngine;

public class DestroyOnFloor : MonoBehaviour
{
    // quand une bouteille touche quelque chose
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Floor" ||
           (collision.transform.parent != null && collision.transform.parent.name == "Floor"))
        {
            Destroy(gameObject);
        }
    }
}