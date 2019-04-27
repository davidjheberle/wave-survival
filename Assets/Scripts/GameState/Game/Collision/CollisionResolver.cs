using UnityEngine;
using System.Collections.Generic;

public class CollisionResolver : MonoBehaviour
{
    private static CollisionResolver instance;

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

    // Late update to check collisions.
    private void LateUpdate()
    {
        // Test enemies...
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.isActiveAndEnabled) continue;

            // Calculate broad test enemy AABB and velocity.
            Vector2 va = enemy.Velocity * Time.deltaTime;

            // ... against bullets.
            foreach (Bullet bullet in bullets)
            {
                if (!bullet.isActiveAndEnabled) continue;

                // Calculate broad test bullet AABB and velocity.
                Vector2 vb = bullet.Velocity * Time.deltaTime;

                // Sweep test.
                Sweep sweep = enemy.AABB.SweepAABB(bullet.AABB, vb - va);
                if (sweep.hit != null)
                {
                    enemy.TakeDamage();
                    bullet.Reset();
                }
            }
        }

        // Test players...
        foreach (Player player in players)
        {
            if (!player.isActiveAndEnabled) continue;

            // Calculate broad test player AABB and velocity.
            Vector2 va = player.Velocity * Time.deltaTime;

            // ... against walls.
            foreach (Wall wall in walls)
            {
                // Sweep test.
                Sweep sweep = wall.AABB.SweepAABB(player.AABB, va);
                if (sweep.hit != null)
                {
                    Debug.DrawLine(sweep.position, player.transform.position);

                    Vector3 sweepPosition = sweep.position;
                    player.transform.position -= sweepPosition - player.transform.position;
                    player.Idle();

                    // Stop the player in the collision direction.
                    // Find the direction the player came from.
                    // Move the player out of the wall in that direction.
                }
            }
        }
    }
}
