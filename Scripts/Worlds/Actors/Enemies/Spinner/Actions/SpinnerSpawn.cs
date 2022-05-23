using DG.Tweening;
using ParadoxNotion.Design;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Enemies.Spinner.Actions
{
    [Category("Enemies/Spinner")]
    public class SpinnerSpawn : SpinnerActionTask
    {
        private Vector2 m_spawnTargetPosition;

        protected override void OnExecute()
        {
            float x = Random.Range(SpinnerActor.MinBounds.x, SpinnerActor.MaxBounds.x);
            float y = SpinnerActor.MaxBounds.y + SpinnerActor.Collider2D.bounds.size.y * 2;

            SpinnerActor.Transform.position = new Vector2(x, y);
            SpinnerActor.Velocity = Vector2.zero;
            SpinnerActor.AllowPassCeiling = true;
            SpinnerActor.AutoHandleMovement = false;

            m_spawnTargetPosition = new Vector2(x, y - SpinnerActor.Collider2D.bounds.size.y * 3);

            SpinnerActor.Transform.DOMove(m_spawnTargetPosition, 0.5f).OnComplete(OnMoveCompete);
        }

        private void OnMoveCompete()
        {
            EndAction(true);
        }
    }
}
