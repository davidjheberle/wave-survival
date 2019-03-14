using UnityEngine;

public class AABB
{
    // Center point.
    public Vector3 Center {
        get;
        private set;
    }

    // Halfwidth extents along each axis.
    public Vector3 Extents {
        get;
        private set;
    }

    // Min point.
    public Vector3 Min {
        get {
            return Center - Extents;
        }
    }

    // Max point.
    public Vector3 Max {
        get {
            return Center + Extents;
        }
    }

    // Size.
    public Vector3 Size {
        get {
            return Extents * 2f;
        }
    }

    // c - center point.
    // e - positive halfwidth extents along each axis.
    public AABB(Vector3 c, Vector3 e)
    {
        this.Center = c;
        this.Extents = e;
    }

    // Test for an intersect against another AABB.
    public Hit IntersectAABB(AABB other)
    {
        Vector3 centerDifference = other.Center - this.Center;
        Vector3 extentsSum = other.Extents + this.Extents;

        float testX = extentsSum.x - Mathf.Abs(centerDifference.x);
        if (testX <= 0) return null;

        float testY = extentsSum.y - Mathf.Abs(centerDifference.y);
        if (testY <= 0) return null;

        float testZ = extentsSum.z - Mathf.Abs(centerDifference.z);
        if (testZ <= 0) return null;

        if (testX < testY)
        {
            float signX = Mathf.Sign(centerDifference.x);
            float deltaX = testX * signX;
            float normalX = signX;
            float posX = this.Center.x + (this.Extents.x * signX);
            return new Hit(new Vector3(posX, 0, 0), new Vector3(deltaX, 0, 0), new Vector3(normalX, 0, 0));
        }
        else if (testY < testZ)
        {
            float signY = Mathf.Sign(centerDifference.y);
            float deltaY = testY * signY;
            float normalY = signY;
            float posY = this.Center.y + (this.Extents.y * signY);
            return new Hit(new Vector3(0, posY, 0), new Vector3(0, deltaY, 0), new Vector3(0, normalY, 0));
        }
        else
        {
            float signZ = Mathf.Sign(centerDifference.z);
            float deltaZ = testZ * signZ;
            float normalZ = signZ;
            float posZ = this.Center.z + (this.Extents.z * signZ);
            return new Hit(new Vector3(0, 0, posZ), new Vector3(0, 0, deltaZ), new Vector3(0, 0, normalZ));
        }
    }

    // Test for an intersect with a segment.
    public Hit IntersectSegment(Vector3 position, Vector3 delta, Vector3 padding)
    {
        Vector3 scale = new Vector3(1f / delta.x, 1f / delta.y, 1f / delta.z);
        Vector3 sign = new Vector3(Mathf.Sign(scale.x), Mathf.Sign(scale.y), Mathf.Sign(scale.z));
        Vector3 nearTime = new Vector3(
            (this.Center.x - sign.x * (this.Extents.x + padding.x) - position.x) * scale.x,
            (this.Center.y - sign.y * (this.Extents.y + padding.y) - position.y) * scale.y,
            (this.Center.z - sign.z * (this.Extents.z + padding.z) - position.z) * scale.z);
        Vector3 farTime = new Vector3(
            (this.Center.x + sign.x * (this.Extents.x + padding.x) - position.x) * scale.x,
            (this.Center.y + sign.y * (this.Extents.y + padding.y) - position.y) * scale.y,
            (this.Center.z + sign.z * (this.Extents.z + padding.z) - position.z) * scale.z);

        if (nearTime.x > farTime.x ||
            nearTime.y > farTime.y ||
            nearTime.z > farTime.z)
        {
            return null;
        }

        float maxNearTime = Mathf.Max(nearTime.x, nearTime.y, nearTime.z);
        float minFarTime = Mathf.Min(farTime.x, farTime.y, farTime.z);

        if (maxNearTime >= 1 || minFarTime <= 0) { return null; }

        float time = Mathf.Clamp01(maxNearTime);
        Vector3 normal;
        if (nearTime.x > nearTime.y)
        {
            normal = new Vector3(-sign.x, 0, 0);
        }
        else if (nearTime.y > nearTime.z)
        {
            normal = new Vector3(0, -sign.y, 0);
        }
        else
        {
            normal = new Vector3(0, 0, -sign.z);
        }
        delta = time * delta;
        position = position + delta;
        return new Hit(position, delta, normal, time);
    }

    // Test with a sweep against another AABB.
    public Sweep SweepAABB(AABB other, Vector3 delta)
    {
        Hit hit;

        // If there is no delta do a static test.
        if (delta.Equals(Vector3.zero))
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
                Vector3 position = other.Center + delta * time;
                Vector3 direction = delta.normalized;
                position.x = Mathf.Clamp(
                    position.x + direction.x * other.Extents.x,
                    this.Center.x - this.Extents.x,
                    this.Center.x + this.Extents.x);
                position.y = Mathf.Clamp(
                    position.y + direction.y * other.Extents.y,
                    this.Center.y - this.Extents.y,
                    this.Center.y + this.Extents.y);
                position.z = Mathf.Clamp(
                    position.z + direction.z * other.Extents.z,
                    this.Center.z - this.Extents.z,
                    this.Center.z + this.Extents.z);
                return new Sweep(hit, position, time);
            }
            else
            {
                Vector3 position = other.Center + delta;
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
