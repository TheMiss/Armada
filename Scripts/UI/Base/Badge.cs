namespace Armageddon.UI.Base
{
    public enum BadgeType
    {
        /// <summary>
        ///     To display the number of events
        /// </summary>
        Numeric,

        /// <summary>
        ///     To tag, name or categorize items
        /// </summary>
        Text,

        /// <summary>
        ///     To show up the status of the underlying object
        /// </summary>
        Icon,

        /// <summary>
        ///     To inform without details
        /// </summary>
        Dot
    }

    public class Badge : Widget
    {
        public void Consume()
        {
            gameObject.SetActive(false);
        }

        public void SetDot()
        {
            gameObject.SetActive(true);
        }
    }
}
