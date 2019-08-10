using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]float speed = 5f;
    [SerializeField]float deadzoneRadius = .1f;

    GameObject target;

    void Update()
    {
        if (target == null)
            return;

        float d = Vector3.Distance(target.transform.position, this.transform.position);

        if (d > deadzoneRadius)
            this.transform.position += (target.transform.position - this.transform.position).normalized * (speed + d) * Time.deltaTime;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
        this.transform.position = target.transform.position;
    }
}
