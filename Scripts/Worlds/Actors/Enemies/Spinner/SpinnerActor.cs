using Armageddon.Sheets.Actors;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Enemies.Spinner
{
    public enum SpinnerPartType
    {
        InnerBody,
        OuterBody
    }

    public class SpinnerActor : EnemyActor
    {
        public new SpinnerSheet Sheet => (SpinnerSheet)base.Sheet;

        private Transform OuterBodyTransform => PartResolvers[(int)SpinnerPartType.OuterBody].Transform;

        public override void Tick()
        {
            base.Tick();

            UpdateSpin();
        }

        private void UpdateSpin()
        {
            Vector3 rotation = OuterBodyTransform.eulerAngles;
            rotation.z += Sheet.RotateSpeed * Time.deltaTime;

            OuterBodyTransform.eulerAngles = rotation;
        }
    }
}
