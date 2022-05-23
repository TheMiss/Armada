using NodeCanvas.Framework;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Actions
{
    public class ChoosePosition : CharacterActionTask
    {
        public BBParameter<Vector2> TargetPosition;
        public BBParameter<float> Speed;

        protected override void OnExecute()
        {
            float x = Random.Range(Character.MinBounds.x, Character.MaxBounds.x);
            float y = Random.Range(Character.MinBounds.y, Character.MaxBounds.y);

            TargetPosition.value = new Vector2(x, y);
            Speed.value = 3.0f;

            EndAction(true);

            // var targetPosition = new Vector2(x, y);
            // Spinner.Transform.LookAt2D(targetPosition, 0, out Vector3 direction);
            // Spinner.Velocity = direction * 30;
        }
    }
}
