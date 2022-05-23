using Armageddon.Worlds.Actors.Characters;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Armageddon.Worlds.Actors.Actions
{
    [Category(Categories.Character)]
    public abstract class CharacterActionTask : ActionTask<CharacterActor>
    {
        private CharacterActor m_character;

        protected CharacterActor Character
        {
            get
            {
                if (m_character == null)
                {
                    m_character = agent.GetComponent<CharacterActor>();
                }

                return m_character;
            }
        }
    }
}
