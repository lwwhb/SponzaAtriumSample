using UnityEngine;

public class DynamicGIRefresher : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        DynamicGI.UpdateEnvironment();
    }
}
