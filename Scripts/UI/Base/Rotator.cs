using UnityEngine;

namespace Armageddon.UI.Base
{
    public class Rotator : Widget
    {
        public float Speed;

        public override void Tick()
        {
            Transform.Rotate(Speed * Vector3.forward * Time.deltaTime);
        }
    }
}
