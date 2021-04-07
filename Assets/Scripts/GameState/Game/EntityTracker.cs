using UnityEngine;
using System.Collections.Generic;

public class EntityTracker : MonoBehaviour
{
    private static EntityTracker instance;

    public static void AddEnemy(Enemy enemy)
    {
        instance.enemies.Add(enemy);
    }

    public static void AddBullet(Bullet bullet)
    {
        instance.bullets.Add(bullet);
    }

    public static void AddPlatform(Platform platform)
    {
        instance.platforms.Add(platform);
    }

    public static void AddWall(Wall wall)
    {
        instance.walls.Add(wall);
    }

    public static void AddPlayer(Player player)
    {
        instance.players.Add(player);
    }

    public static List<Bullet> GetBullets()
    {
        return instance.bullets;
    }

    public static List<Enemy> GetEnemies()
    {
        return instance.enemies;
    }

    public static List<Platform> GetPlatforms()
    {
        return instance.platforms;
    }

    public static List<Wall> GetWalls()
    {
        return instance.walls;
    }

    public static List<Player> GetPlayers()
    {
        return instance.players;
    }

    private List<Bullet> bullets = new List<Bullet>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Platform> platforms = new List<Platform>();
    private List<Wall> walls = new List<Wall>();
    private List<Player> players = new List<Player>();

    // Self-initialize.
    private void Awake()
    {
        instance = this;
    }
}
