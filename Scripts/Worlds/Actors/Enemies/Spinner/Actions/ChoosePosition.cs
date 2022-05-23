using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Enemies.Spinner.Actions
{
    [Category("Enemies/Spinner")]
    [Description("SDFDF")]
    public class ChoosePosition : SpinnerActionTask
    {
        public BBParameter<Vector2> TargetPosition;
        public BBParameter<float> Speed;

        protected override void OnExecute()
        {
            float x = Random.Range(SpinnerActor.MinBounds.x, SpinnerActor.MaxBounds.x);
            float y = Random.Range(SpinnerActor.MinBounds.y, SpinnerActor.MaxBounds.y);

            TargetPosition.value = new Vector2(x, y);
            Speed.value = 3.0f;

            EndAction(true);

            // var targetPosition = new Vector2(x, y);
            // Spinner.Transform.LookAt2D(targetPosition, 0, out Vector3 direction);
            // Spinner.Velocity = direction * 30;
        }
    }
}
