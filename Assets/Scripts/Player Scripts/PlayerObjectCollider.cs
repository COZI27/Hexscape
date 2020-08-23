using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectCollider : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        foreach (ITriggerable component in other.gameObject.GetComponents<ITriggerable>())
        {
            component.Trigger();
        }
    }
}
