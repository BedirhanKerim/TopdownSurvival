using TopdownSurvival.Enemies;
using TopdownSurvival.Level;
using TopdownSurvival.Player;
using TopdownSurvival.Save;
using TopdownSurvival.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TopdownSurvival.Core
{
    public sealed class GameBootstrap : LifetimeScope
    {
        [SerializeField] private LevelData[] m_Levels;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameEventBus>(Lifetime.Singleton);
            builder.Register<SaveSystem>(Lifetime.Singleton);
            builder.Register<LevelManager>(Lifetime.Singleton);
            builder.RegisterInstance(m_Levels);

            builder.RegisterComponentInHierarchy<EnemySpawner>();
            builder.RegisterComponentInHierarchy<PlayerHealth>();
            builder.RegisterComponentInHierarchy<UIManager>();

            builder.RegisterEntryPoint<GameFlow>();
        }
    }
}
