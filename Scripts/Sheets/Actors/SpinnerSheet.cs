using UnityEngine;

namespace Armageddon.Sheets.Actors
{
    public class SpinnerSheet : EnemySheet
    {
        [SerializeField]
        private float m_rotateSpeed = 30.0f;

        public float RotateSpeed => m_rotateSpeed;
    }
}
