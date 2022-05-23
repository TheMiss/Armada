using DG.Tweening;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Actions
{
    public class Spawn : CharacterActionTask
    {
        private Vector2 m_spawnTargetPosition;

        protected override void OnExecute()
        {
            float x = Random.Range(Character.MinBounds.x, Character.MaxBounds.x);
            Bounds bounds = Character.Collider2D.bounds;
            float y = Character.MaxBounds.y + bounds.size.y * 2;

            Character.Transform.position = new Vector2(x, y);
            Character.Velocity = Vector2.zero;
            Character.AllowPassCeiling = true;
            Character.AutoHandleMovement = false;

            m_spawnTargetPosition = new Vector2(x, y - bounds.size.y * 3);

            Character.Transform.DOMove(m_spawnTargetPosition, 0.5f).OnComplete(OnMoveCompete);
        }

        private void OnMoveCompete()
        {
            EndAction(true);
        }
    }
}
