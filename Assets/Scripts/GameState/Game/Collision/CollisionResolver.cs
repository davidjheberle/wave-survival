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

    private List<Bullet> bullets = new List<Bullet>();
    private List<Enemy> enemies = new List<Enemy>();

    // Self-initialize.
    private void Awake()
    {
        instance = this;
    }

    // Late update to check collisions.
    private void LateUpdate()
    {
        foreach (Enemy enemy in enemies)
        {
            foreach (Bullet bullet in bullets)
            {
                int result = TestOBBOBB(enemy.OBB, bullet.OBB);
                if (result == 1)
                {
                    enemy.Color = Color.red;
                }
                else
                {
                    enemy.Color = Color.black;
                }
            }
        }
    }

    private int TestOBBOBB(OBB a, OBB b)
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
                return 0;
            }
        }

        // Test axes L = B0, L = B1, L = B2.
        for (int i = 0; i < 3; i++)
        {
            ra = a.e[0] * absR[0, i] + a.e[1] * absR[1, i] + a.e[2] * absR[2, i];
            rb = b.e[i];
            if (Mathf.Abs(t[0] * r[0, i] + t[1] * r[1, i] + t[2] * r[2, i]) > ra + rb)
            {
                return 0;
            }
        }

        // Test axis L = A0 x B0.
        ra = a.e[1] * absR[2, 0] + a.e[2] * absR[1, 0];
        rb = b.e[1] * absR[0, 2] + b.e[2] * absR[0, 1];
        if (Mathf.Abs(t[2] * r[1, 0] - t[1] * r[2, 0]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A0 x B1.
        ra = a.e[1] * absR[2, 1] + a.e[2] * absR[1, 1];
        rb = b.e[0] * absR[0, 2] + b.e[2] * absR[0, 0];
        if (Mathf.Abs(t[2] * r[1, 1] - t[1] * r[2, 1]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A0 x B2.
        ra = a.e[1] * absR[2, 2] + a.e[2] * absR[1, 2];
        rb = b.e[0] * absR[0, 2] + b.e[1] * absR[0, 0];
        if (Mathf.Abs(t[2] * r[1, 2] - t[1] * r[2, 2]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A1 x B0.
        ra = a.e[0] * absR[2, 0] + a.e[2] * absR[0, 0];
        rb = b.e[1] * absR[1, 2] + b.e[2] * absR[1, 1];
        if (Mathf.Abs(t[0] * r[2, 0] - t[2] * r[0, 0]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A1 x B1.
        ra = a.e[0] * absR[2, 1] + a.e[2] * absR[0, 1];
        rb = b.e[0] * absR[1, 2] + b.e[2] * absR[1, 0];
        if (Mathf.Abs(t[0] * r[2, 1] - t[2] * r[0, 1]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A1 x B2.
        ra = a.e[0] * absR[2, 2] + a.e[2] * absR[0, 2];
        rb = b.e[0] * absR[1, 1] + b.e[1] * absR[1, 0];
        if (Mathf.Abs(t[0] * r[2, 2] - t[2] * r[0, 2]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A2 x B0.
        ra = a.e[0] * absR[1, 0] + a.e[1] * absR[0, 0];
        rb = b.e[1] * absR[2, 2] + b.e[2] * absR[2, 1];
        if (Mathf.Abs(t[1] * r[0, 0] - t[0] * r[1, 0]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A2 x B1.
        ra = a.e[0] * absR[1, 1] + a.e[1] * absR[0, 1];
        rb = b.e[0] * absR[2, 2] + b.e[2] * absR[2, 0];
        if (Mathf.Abs(t[1] * r[0, 1] - t[0] * r[1, 1]) > ra + rb)
        {
            return 0;
        }

        // Test axis L = A2 x B2.
        ra = a.e[0] * absR[1, 2] + a.e[1] * absR[0, 2];
        rb = b.e[0] * absR[2, 1] + b.e[1] * absR[2, 0];
        if (Mathf.Abs(t[1] * r[0, 2] - t[0] * r[1, 2]) > ra + rb)
        {
            return 0;
        }

        // Since no separating axis is found, the OBBs must be intersecting.
        return 1;
    }
}
