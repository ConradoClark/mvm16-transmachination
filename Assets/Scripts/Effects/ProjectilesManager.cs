using System;
using System.Linq;
using Licht.Unity.Objects;

public class ProjectilesManager : SceneObject<ProjectilesManager>
{
    [Serializable]
    public struct ProjectileDefinition
    {
        public ProjectilePool Pool;
        public string ProjectileName;
    }

    public ProjectileDefinition[] Projectiles;

    public ProjectilePool GetProjectile(string projectileName)
    {
        return Projectiles.FirstOrDefault(eff => eff.ProjectileName == projectileName).Pool;
    }
}
