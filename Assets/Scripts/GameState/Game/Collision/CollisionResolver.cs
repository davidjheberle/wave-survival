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
            AABB a = MostSeparatedPointsOnAABB(new Vector2[] { enemy.AABB.Min, enemy.AABB.Max, enemy.AABB.Min + va, enemy.AABB.Max + va });

            // ... against bullets.
            foreach (Bullet bullet in bullets)
            {
                if (!bullet.isActiveAndEnabled) continue;

                // Calculate broad test bullet AABB and velocity.
                Vector2 vb = bullet.Velocity * Time.deltaTime;
                AABB b = MostSeparatedPointsOnAABB(new Vector2[] { bullet.AABB.Min, bullet.AABB.Max, bullet.AABB.Min + vb, bullet.AABB.Max + vb });

                // Broad test.
                Hit hit = TestAABBAABB(a, b);
                if (hit != null)
                {
                    // Narrow test.
                    hit = IntersectMovingAABBAABB(enemy.AABB, bullet.AABB, va, vb);
                    if (hit != null)
                    {
                        enemy.TakeDamage();
                        bullet.Reset();
                    }
                }
            }
        }

        // Test players...
        foreach (Player player in players)
        {
            if (!player.isActiveAndEnabled) continue;

            // Calculate broad test player AABB and velocity.
            Vector2 va = player.Velocity * Time.deltaTime;
            AABB a = MostSeparatedPointsOnAABB(new Vector2[] { player.AABB.Min, player.AABB.Max, player.AABB.Min + va, player.AABB.Max + va });

            // ... against walls.
            foreach (Wall wall in walls)
            {
                // Broad test.
                //Hit hit = TestAABBAABB(a, wall.AABB);
                Hit hit = a.IntersectAABB(wall.AABB);
                if (hit != null)
                {
                    // Narrow test.
                    //hit = IntersectMovingAABBAABB(player.AABB, wall.AABB, va, Vector3.zero);
                    Sweep sweep = player.AABB.SweepAABB(wall.AABB, va);
                    if (sweep != null)
                    {
                        Debug.Log("Player in wall.");
                        Debug.LogFormat("Sweep Position: {0}", sweep.Position);
                        Debug.LogFormat("Sweep time: {0}", sweep.Time);

                        player.transform.position = player.transform.position + sweep.Position;
                        player.Idle();

                        // Stop the player in the collision direction.
                        // Find the direction the player came from.
                        // Move the player out of the wall in that direction.
                    }
                }
            }
        }
    }

    private Hit TestAABBAABB(AABB a, AABB b)
    {
        Vector2 centerDifference = a.Center - b.Center;
        Vector2 extentsSum = a.Extents + b.Extents;
        float testX = extentsSum.x - Mathf.Abs(centerDifference.x);
        float testY = extentsSum.y - Mathf.Abs(centerDifference.y);
        if (testX <= 0) return null;
        if (testY <= 0) return null;

        if (testX < testY)
        {
            float signX = Mathf.Sign(centerDifference.x);
            float deltaX = testX * signX;
            float normalX = signX;
            float posX = a.Center.x + (a.Extents.x * signX);
            return new Hit(new Vector2(posX, 0), new Vector2(deltaX, 0), new Vector2(normalX, 0));
        }
        else
        {
            float signY = Mathf.Sign(centerDifference.y);
            float deltaY = testY * signY;
            float normalY = signY;
            float posY = a.Center.y + (a.Extents.y * signY);
            return new Hit(new Vector2(0, posY), new Vector2(0, deltaY), new Vector2(0, normalY));
        }
    }

    private Hit IntersectMovingAABBAABB(AABB a, AABB b, Vector2 va, Vector2 vb)
    {
        // Exit early if 'a' and 'b' are initially overlapping.
        Hit hit = TestAABBAABB(a, b);
        if (hit != null)
        {
            return hit;
        }

        // Use relative velocity.
        // Effectively treating 'a' as stationary.
        Vector3 v = vb - va;

        // Initialize times of first and last contact.
        float tfirst = 0f;
        float tlast = 1f;

        // For each axis, determine times of first and last contact, if any.
        for (int i = 0; i < 2; i++)
        {
            if (v[i] < 0f)
            {
                if (b.Max[i] < a.Min[i]) return null;      // Nonintersecting and moving apart.
                if (a.Max[i] < b.Min[i]) tfirst = Mathf.Max((a.Max[i] - b.Min[i]) / v[i], tfirst);
                if (b.Max[i] > a.Min[i]) tlast = Mathf.Min((a.Min[i] - b.Max[i]) / v[i], tlast);
            }
            if (v[i] > 0f)
            {
                if (b.Min[i] > a.Max[i]) return null;      // Nonintersecting and moving apart.
                if (b.Max[i] < a.Min[i]) tfirst = Mathf.Max((a.Min[i] - b.Max[i]) / v[i], tfirst);
                if (a.Max[i] > b.Min[i]) tlast = Mathf.Min((a.Max[i] - b.Min[i]) / v[i], tlast);
            }

            // No overlap possible if time of first contact occurs after time of last contact.
            if (tfirst > tlast)
            {
                return null;
            }
        }
        return new Hit(Vector2.zero, Vector2.zero, Vector2.zero);
    }

    public AABB MostSeparatedPointsOnAABB(Vector2[] points)
    {
        // First find most extreme points along the principal axes.
        int minx = 0, maxx = 0, miny = 0, maxy = 0;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i].x < points[minx].x) minx = i;
            if (points[i].x > points[maxx].x) maxx = i;
            if (points[i].y < points[miny].y) miny = i;
            if (points[i].y > points[maxy].y) maxy = i;
        }

        // Compute the squared distances for the three pairs of points.
        float dist2x = Vector3.Dot(points[maxx] - points[minx], points[maxx] - points[minx]);
        float dist2y = Vector3.Dot(points[maxy] - points[miny], points[maxy] - points[miny]);

        // Pick the pair (min, max) of points most distant.
        int min = minx;
        int max = maxx;
        if (dist2y > dist2x)
        {
            max = maxy;
            min = miny;
        }

        Vector2 extents = (points[max] - points[min]) / 2f;
        Vector2 center = points[min] + extents;
        return new AABB(center, extents);
    }
}
