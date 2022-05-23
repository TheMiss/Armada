using Armageddon.Audio;
using Armageddon.Localization;
using Armageddon.UI;
using Armageddon.Worlds;
using Armageddon.Worlds.Actors.Unused;
using Purity.Common;

namespace Armageddon.Games
{
    /// <summary>
    ///     Be careful to always call base.Awake() in derived classes
    /// </summary>
    public abstract class GameContext : Context
    {
        private AudioSystem m_audio;

        private World m_world;

        private Game m_game;

        private LocalizationSystem m_localization;

        private RenderTextureManager m_renderTextureManager;

        private UISystem m_ui;

        public Game Game
        {
            get
            {
                if (m_game == null)
                {
                    m_game = GetService<Game>();
                }

                return m_game;
            }
        }

        public LocalizationSystem Localization
        {
            get
            {
                if (m_localization == null)
                {
                    m_localization = GetService<LocalizationSystem>();
                }

                return m_localization;
            }
        }

        public UISystem UI
        {
            get
            {
                if (m_ui == null)
                {
                    m_ui = GetService<UISystem>();
                }

                return m_ui;
            }
        }

        public AudioSystem Audio
        {
            get
            {
                if (m_audio == null)
                {
                    m_audio = GetService<AudioSystem>();
                }

                return m_audio;
            }
        }

        public RenderTextureManager RenderTextureManager
        {
            get
            {
                if (m_renderTextureManager == null)
                {
                    m_renderTextureManager = GetService<RenderTextureManager>();
                }

                return m_renderTextureManager;
            }
        }

        public World World
        {
            get
            {
                if (m_world == null)
                {
                    m_world = GetService<World>();
                }

                return m_world;
            }
        }

        // protected virtual void Start()
        // {
        //     // It's not safe to assume Start() will be called before these will be used, let users locate when needed
        //
        //     Game = GetService<Game>();
        //     PoolFlusher = GetService<PoolFlusher>();
        //     BulletPool = GetService<BulletPool>();
        //     EnemyPool = GetService<EnemyPool>();
        //     DamageTextPool = GetService<DamageTextPool>();
        //     UIManager = GetService<UIManager>();
        //     Level = GetService<Level>();
        // }
    }
}
