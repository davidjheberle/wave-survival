using UnityEngine;

public class AABB
{
    // Center point.
    public Vector2 Center {
        get;
        private set;
    }

    // Halfwidth extents along each axis.
    public Vector2 Extents {
        get;
        private set;
    }

    // Min point.
    public Vector2 Min {
        get {
            return Center - Extents;
        }
    }

    // Max point.
    public Vector2 Max {
        get {
            return Center + Extents;
        }
    }

    // Size.
    public Vector2 Size {
        get {
            return Extents * 2f;
        }
    }

    // c - center point.
    // e - positive halfwidth extents along each axis.
    public AABB(Vector2 c, Vector2 e)
    {
        this.Center = c;
        this.Extents = e;
    }

    // Test for an intersect against another AABB.
    public Hit IntersectAABB(AABB other)
    {
        Vector2 centerDifference = other.Center - this.Center;
        Vector2 extentsSum = other.Extents + this.Extents;

        float testX = extentsSum.x - Mathf.Abs(centerDifference.x);
        if (testX <= 0) return null;

        float testY = extentsSum.y - Mathf.Abs(centerDifference.y);
        if (testY <= 0) return null;

        if (testX < testY)
        {
            float signX = Mathf.Sign(centerDifference.x);
            float deltaX = testX * signX;
            float normalX = signX;
            float posX = this.Center.x + (this.Extents.x * signX);
            return new Hit(new Vector2(posX, 0), new Vector2(deltaX, 0), new Vector2(normalX, 0));
        }
        else
        {
            float signY = Mathf.Sign(centerDifference.y);
            float deltaY = testY * signY;
            float normalY = signY;
            float posY = this.Center.y + (this.Extents.y * signY);
            return new Hit(new Vector2(0, posY), new Vector2(0, deltaY), new Vector2(0, normalY));
        }
    }

    // Test for an intersect with a segment.
    public Hit IntersectSegment(Vector2 position, Vector2 delta, Vector2 padding)
    {
        Vector2 scale = new Vector2(1f / delta.x, 1f / delta.y);
        Vector2 sign = new Vector2(Mathf.Sign(scale.x), Mathf.Sign(scale.y));
        Vector2 nearTime = new Vector2(
            (this.Center.x - sign.x * (this.Extents.x + padding.x) - position.x) * scale.x,
            (this.Center.y - sign.y * (this.Extents.y + padding.y) - position.y) * scale.y);
        Vector2 farTime = new Vector2(
            (this.Center.x + sign.x * (this.Extents.x + padding.x) - position.x) * scale.x,
            (this.Center.y + sign.y * (this.Extents.y + padding.y) - position.y) * scale.y);

        if (nearTime.x > farTime.x ||
            nearTime.y > farTime.y)
        {
            return null;
        }

        float maxNearTime = Mathf.Max(nearTime.x, nearTime.y);
        float minFarTime = Mathf.Min(farTime.x, farTime.y);

        if (maxNearTime >= 1 || minFarTime <= 0) { return null; }

        float time = Mathf.Clamp01(maxNearTime);
        Vector2 normal;
        if (nearTime.x > nearTime.y)
        {
            normal = new Vector2(-sign.x, 0);
        }
        else
        {
            normal = new Vector2(0, -sign.y);
        }
        delta = time * delta;
        position = position + delta;
        return new Hit(position, delta, normal, time);
    }

    // Test with a sweep against another AABB.
    public Sweep SweepAABB(AABB other, Vector2 delta)
    {
        Hit hit;

        // If there is no delta do a static test.
        if (delta.Equals(Vector2.zero))
        {
            hit = this.IntersectAABB(other);
            float time = 1;
            if (hit != null)
            {
                time = hit.Time;
            }
            return new Sweep(hit, hit.Position, time);
        }
        else
        {
            hit = this.IntersectSegment(other.Center, delta, other.Extents);
            if (hit != null)
            {
                float time = Mathf.Clamp01(hit.Time - Mathf.Epsilon);
                Vector2 position = other.Center + delta * time;
                Vector2 direction = delta.normalized;
                position.x = Mathf.Clamp(
                    position.x + direction.x * other.Extents.x,
                    this.Center.x - this.Extents.x,
                    this.Center.x + this.Extents.x);
                position.y = Mathf.Clamp(
                    position.y + direction.y * other.Extents.y,
                    this.Center.y - this.Extents.y,
                    this.Center.y + this.Extents.y);
                return new Sweep(hit, position, time);
            }
            else
            {
                Vector2 position = other.Center + delta;
                return new Sweep(hit, position);
            }
        }
    }

    // Debug draw function.
    public void Draw(Color color)
    {
        Vector3 topRight = new Vector3(Center.x + Extents.x, Center.y - Extents.y);
        Vector3 topLeft = Center - Extents;
        Vector3 bottomRight = Center + Extents;
        Vector3 bottomLeft = new Vector3(Center.x - Extents.x, Center.y + Extents.y);
        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);
    }
}
