using UnityEngine;

namespace Battle
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private Team _team;
        [SerializeField, Range(0, 3)] private int _index;

        public Team Team => _team;
        public int Index => _index;

        private void OnDrawGizmos()
        {
            Gizmos.color = _team == Team.Ally ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.5f,
                $"{(_team == Team.Ally ? "A" : "E")}{_index}"
            );
#endif
        }
    }
}
