using NodeCanvas.Framework;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Actions
{
    public class MoveToPosition : CharacterActionTask
    {
        public BBParameter<Vector2> TargetPosition;
        public BBParameter<float> Speed;

        private Vector2 m_direction;

        // Use for initialization. This is called only once in the lifetime of the task.
        // Return null if init was successful. Return an error string otherwise
        protected override string OnInit()
        {
            return null;
        }

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {
            m_direction = (TargetPosition.value - Character.Position).normalized;
        }

        //Called once per frame while the action is active.
        protected override void OnUpdate()
        {
            m_direction = (TargetPosition.value - Character.Position).normalized;

            Vector2 displacement = m_direction * (Speed.value * Time.deltaTime);
            float distanceFromTarget = Vector2.Distance(TargetPosition.value, Character.Position);

            if (distanceFromTarget > displacement.magnitude)
            {
                Character.Position += displacement;
            }
            else
            {
                Character.Position = TargetPosition.value;
                EndAction(true);
            }
        }

        public override void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(Character.Position, TargetPosition.value);
        }

        //Called when the task is disabled.
        protected override void OnStop()
        {
        }

        //Called when the task is paused.
        protected override void OnPause()
        {
        }
    }
}
