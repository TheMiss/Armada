using NodeCanvas.Framework;

namespace Armageddon.Worlds.Actors.Enemies.Spinner.Actions
{
    // [Category("Spinner")]
    // [Description("For Spinner Only")]
    public abstract class SpinnerActionTask : ActionTask<SpinnerActor>
    {
        private SpinnerActor m_spinnerActor;

        protected SpinnerActor SpinnerActor
        {
            get
            {
                if (m_spinnerActor == null)
                {
                    m_spinnerActor = agent.GetComponent<SpinnerActor>();
                }

                return m_spinnerActor;
            }
        }
    }
}
