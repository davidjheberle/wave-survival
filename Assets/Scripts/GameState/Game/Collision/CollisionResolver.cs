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
            Vector3 va = enemy.Velocity * Time.deltaTime;
            AABB a = MostSeparatedPointsOnAABB(new Vector3[] { enemy.AABB.Min, enemy.AABB.Max, enemy.AABB.Min + va, enemy.AABB.Max + va });

            // ... against bullets.
            foreach (Bullet bullet in bullets)
            {
                if (!bullet.isActiveAndEnabled) continue;

                // Calculate broad test bullet AABB and velocity.
                Vector3 vb = bullet.Velocity * Time.deltaTime;
                AABB b = MostSeparatedPointsOnAABB(new Vector3[] { bullet.AABB.Min, bullet.AABB.Max, bullet.AABB.Min + vb, bullet.AABB.Max + vb });

                // Broad test.
                Hit hit = TestAABBAABB(a, b);
                if (hit != null)
                {
                    // Narrow test.
                    hit = IntersectMovingAABBAABB(enemy.AABB, bullet.AABB, va, vb);
                }
                // Final hit result.
                if (hit != null)
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
            Vector3 va = player.Velocity * Time.deltaTime;
            AABB a = MostSeparatedPointsOnAABB(new Vector3[] { player.AABB.Min, player.AABB.Max, player.AABB.Min + va, player.AABB.Max + va });

            // ... against walls.
            foreach (Wall wall in walls)
            {
                // Broad test.
                Hit hit = TestAABBAABB(a, wall.AABB);
                if (hit != null)
                {
                    // Narrow test.
                    hit = IntersectMovingAABBAABB(player.AABB, wall.AABB, va, Vector3.zero);
                }
                // Final hit result.
                if (hit != null)
                {
                    Debug.Log("Player in wall.");

                    // Stop the player in the collision direction.
                    // Find the direction the player came from.
                    // Move the player out of the wall in that direction.
                }
            }
        }
    }

    private Hit TestAABBAABB(AABB a, AABB b)
    {
        //if (Mathf.Abs(a.Center[0] - b.Center[0]) > (a.Extents[0] + b.Extents[0])) return null;
        //if (Mathf.Abs(a.Center[1] - b.Center[1]) > (a.Extents[1] + b.Extents[1])) return null;
        //if (Mathf.Abs(a.Center[2] - b.Center[2]) > (a.Extents[2] + b.Extents[2])) return null;

        Vector3 centerDifference = a.Center - b.Center;
        Vector3 extentsSum = a.Extents + b.Extents;
        float testX = extentsSum.x - Mathf.Abs(centerDifference.x);
        float testY = extentsSum.y - Mathf.Abs(centerDifference.y);
        float testZ = extentsSum.z - Mathf.Abs(centerDifference.z);
        if (testX <= 0) return null;
        if (testY <= 0) return null;
        //if (testZ <= 0) return null;

        if (testX < testY && testX < testZ)
        {
            float signX = Mathf.Sign(centerDifference.x);
            float deltaX = testX * signX;
            float normalX = signX;
            float posX = a.Center.x + (a.Extents.x * signX);
            return new Hit(new Vector3(posX, 0, 0), new Vector3(deltaX, 0, 0), new Vector3(normalX, 0, 0));
        }
        else if (testY < testX && testY < testZ)
        {
            float signY = Mathf.Sign(centerDifference.y);
            float deltaY = testY * signY;
            float normalY = signY;
            float posY = a.Center.y + (a.Extents.y * signY);
            return new Hit(new Vector3(0, posY, 0), new Vector3(0, deltaY, 0), new Vector3(0, normalY, 0));
        }
        else
        {
            float signZ = Mathf.Sign(centerDifference.z);
            float deltaZ = testZ * signZ;
            float normalZ = signZ;
            float posZ = a.Center.z + (a.Extents.z * signZ);
            return new Hit(new Vector3(0, 0, posZ), new Vector3(0, 0, deltaZ), new Vector3(0, 0, normalZ));
        }
    }

    private Hit IntersectMovingAABBAABB(AABB a, AABB b, Vector3 va, Vector3 vb)
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
        for (int i = 0; i < 3; i++)
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
        return new Hit(Vector3.zero, Vector3.zero, Vector3.zero);
    }

    private Hit TestOBBOBB(OBB a, OBB b)
    {
        float ra, rb;
        Matrix4x4 r = new Matrix4x4();
        Matrix4x4 absR = new Matrix4x4();

        // Compute rotation matrix expressing b in a's coordinate frame.
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i == 3 && j == 3)
                {
                    absR[i, j] = 1;
                }
                else if (i == 3 || j == 3)
                {
                    absR[i, j] = 0;
                }
                else
                {
                    r[i, j] = Vector3.Dot(a.u[i], b.u[j]);
                }
            }
        }

        // Compute translation vector t.
        Vector3 t = b.c - a.c;
        // Bring translation into a's coordinate frame.
        t = new Vector3(Vector3.Dot(t, a.u[0]), Vector3.Dot(t, a.u[1]), Vector3.Dot(t, a.u[2]));

        // Compute common subexpressions. Add in an epsilon term to
        // counteract arithmetic errors when two edges are parallel and
        // their cross product is (near) null.
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i == 3 && j == 3)
                {
                    absR[i, j] = 1;
                }
                else if (i == 3 || j == 3)
                {
                    absR[i, j] = 0;
                }
                else
                {
                    absR[i, j] = Mathf.Abs(r[i, j]) + Mathf.Epsilon;
                }
            }
        }

        // Test axes L = A0, L = A1, L = A2.
        for (int i = 0; i < 3; i++)
        {
            ra = a.e[i];
            rb = b.e[0] * absR[i, 0] + b.e[1] * absR[i, 1] + b.e[2] * absR[i, 2];
            if (Mathf.Abs(t[i]) > ra + rb)
            {
                return null;
            }
        }

        // Test axes L = B0, L = B1, L = B2.
        for (int i = 0; i < 3; i++)
        {
            ra = a.e[0] * absR[0, i] + a.e[1] * absR[1, i] + a.e[2] * absR[2, i];
            rb = b.e[i];
            if (Mathf.Abs(t[0] * r[0, i] + t[1] * r[1, i] + t[2] * r[2, i]) > ra + rb)
            {
                return null;
            }
        }

        // Test axis L = A0 x B0.
        ra = a.e[1] * absR[2, 0] + a.e[2] * absR[1, 0];
        rb = b.e[1] * absR[0, 2] + b.e[2] * absR[0, 1];
        if (Mathf.Abs(t[2] * r[1, 0] - t[1] * r[2, 0]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A0 x B1.
        ra = a.e[1] * absR[2, 1] + a.e[2] * absR[1, 1];
        rb = b.e[0] * absR[0, 2] + b.e[2] * absR[0, 0];
        if (Mathf.Abs(t[2] * r[1, 1] - t[1] * r[2, 1]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A0 x B2.
        ra = a.e[1] * absR[2, 2] + a.e[2] * absR[1, 2];
        rb = b.e[0] * absR[0, 2] + b.e[1] * absR[0, 0];
        if (Mathf.Abs(t[2] * r[1, 2] - t[1] * r[2, 2]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A1 x B0.
        ra = a.e[0] * absR[2, 0] + a.e[2] * absR[0, 0];
        rb = b.e[1] * absR[1, 2] + b.e[2] * absR[1, 1];
        if (Mathf.Abs(t[0] * r[2, 0] - t[2] * r[0, 0]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A1 x B1.
        ra = a.e[0] * absR[2, 1] + a.e[2] * absR[0, 1];
        rb = b.e[0] * absR[1, 2] + b.e[2] * absR[1, 0];
        if (Mathf.Abs(t[0] * r[2, 1] - t[2] * r[0, 1]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A1 x B2.
        ra = a.e[0] * absR[2, 2] + a.e[2] * absR[0, 2];
        rb = b.e[0] * absR[1, 1] + b.e[1] * absR[1, 0];
        if (Mathf.Abs(t[0] * r[2, 2] - t[2] * r[0, 2]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A2 x B0.
        ra = a.e[0] * absR[1, 0] + a.e[1] * absR[0, 0];
        rb = b.e[1] * absR[2, 2] + b.e[2] * absR[2, 1];
        if (Mathf.Abs(t[1] * r[0, 0] - t[0] * r[1, 0]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A2 x B1.
        ra = a.e[0] * absR[1, 1] + a.e[1] * absR[0, 1];
        rb = b.e[0] * absR[2, 2] + b.e[2] * absR[2, 0];
        if (Mathf.Abs(t[1] * r[0, 1] - t[0] * r[1, 1]) > ra + rb)
        {
            return null;
        }

        // Test axis L = A2 x B2.
        ra = a.e[0] * absR[1, 2] + a.e[1] * absR[0, 2];
        rb = b.e[0] * absR[2, 1] + b.e[1] * absR[2, 0];
        if (Mathf.Abs(t[1] * r[0, 2] - t[0] * r[1, 2]) > ra + rb)
        {
            return null;
        }

        // Since no separating axis is found, the OBBs must be intersecting.
        return new Hit(Vector3.zero, Vector3.zero, Vector3.zero);
    }

    public AABB MostSeparatedPointsOnAABB(Vector3[] points)
    {
        // First find most extreme points along the principal axes.
        int minx = 0, maxx = 0, miny = 0, maxy = 0, minz = 0, maxz = 0;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i].x < points[minx].x) minx = i;
            if (points[i].x > points[maxx].x) maxx = i;
            if (points[i].y < points[miny].y) miny = i;
            if (points[i].y > points[maxy].y) maxy = i;
            if (points[i].z < points[minz].z) minz = i;
            if (points[i].z > points[maxz].z) maxz = i;
        }

        // Compute the squared distances for the three pairs of points.
        float dist2x = Vector3.Dot(points[maxx] - points[minx], points[maxx] - points[minx]);
        float dist2y = Vector3.Dot(points[maxy] - points[miny], points[maxy] - points[miny]);
        float dist2z = Vector3.Dot(points[maxz] - points[minz], points[maxz] - points[minz]);

        // Pick the pair (min, max) of points most distant.
        int min = minx;
        int max = maxx;
        if (dist2y > dist2x && dist2y > dist2z)
        {
            max = maxy;
            min = miny;
        }
        if (dist2z > dist2x && dist2z > dist2y)
        {
            max = maxz;
            min = minz;
        }

        Vector3 extents = (points[max] - points[min]) / 2f;
        Vector3 center = points[min] + extents;
        return new AABB(center, extents);
    }
}
