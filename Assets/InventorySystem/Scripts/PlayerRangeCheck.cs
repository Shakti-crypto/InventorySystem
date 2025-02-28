using UnityEngine;

namespace InventorySystem
{
    public class PlayerRangeCheck : MonoBehaviour
    {
        [SerializeField] private string interactionTag = "Player";
        public bool playerInRange;

        public delegate void PlayerEnteredRange(GameObject player);
        public PlayerEnteredRange playerEnteredRange;

        public delegate void PlayerLeftRange(GameObject player);
        public PlayerLeftRange playerLeftRange;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag))
            {
                playerInRange = true;
                playerEnteredRange?.Invoke(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(interactionTag))
            {
                playerInRange = false;
                playerLeftRange?.Invoke(other.gameObject);
            }
        }
    }

}