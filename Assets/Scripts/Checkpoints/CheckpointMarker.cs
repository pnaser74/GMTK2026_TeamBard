using UnityEngine;

// This is proximity-based. Add this script to an object in a scene to trigger a save.
[DisallowMultipleComponent]
public class CheckpointMarker : MonoBehaviour
{
    [Tooltip("Global Checkpoint Id.")]
    [SerializeField] private int _globalCheckpointId = 1;

    [Tooltip("How close the player has to get for this checkpoint to be recorded.")]
    [SerializeField] private float _triggerRadius = 1.5f;

    private Transform _player;
    private bool _recorded;

    public int GlobalCheckpointId => _globalCheckpointId;

    private void Update()
    {
        if (_recorded)
            return;
        
        if (_player == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
                return;

            _player = player.transform;
        }

        if ((_player.position - transform.position).sqrMagnitude > _triggerRadius * _triggerRadius)
            return;

        _recorded = true;

        // walking back past an earlier checkpoint should not undo progress.
        if (_globalCheckpointId > SaveService.Checkpoint || !SaveService.HasSave)
            SaveService.SetCheckpoint(_globalCheckpointId);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.85f, 0.3f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, _triggerRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
}
