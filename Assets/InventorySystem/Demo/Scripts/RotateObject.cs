using UnityEngine;

namespace InventorySystem.Demo
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Vector3 rotationAxis;

        private void Update()
        {
            transform.rotation = transform.rotation * Quaternion.Euler(rotationAxis*rotationSpeed*Time.deltaTime);
        }
    }
}
