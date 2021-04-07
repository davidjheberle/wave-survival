using UnityEngine;
using System;

public class AABB
{
    public Vector3 position;
    public Vector3 half;

    public Vector3 Min {
        get {
            return this.position - this.half;
        }
    }

    public Vector3 Max {
        get {
            return this.position + this.half;
        }
    }

    public AABB(Vector3 position, Vector3 half)
    {
        this.position = position;
        this.half = half;
    }

    public Hit IntersectSegment(Vector3 position, Vector3 delta, float paddingX = 0, float paddingY = 0)
    {
        float scaleX = 1.0f / delta.x;
        float scaleY = 1.0f / delta.y;
        float signX = Mathf.Sign(scaleX);
        float signY = Mathf.Sign(scaleY);
        float nearTimeX = (this.position.x - signX * (this.half.x + paddingX) - position.x) * scaleX;
        float nearTimeY = (this.position.y - signY * (this.half.y + paddingY) - position.y) * scaleY;
        float farTimeX = (this.position.x + signX * (this.half.x + paddingX) - position.x) * scaleX;
        float farTimeY = (this.position.y + signY * (this.half.y + paddingY) - position.y) * scaleY;

        if (Double.IsNaN(nearTimeX) || 
            Double.IsNaN(nearTimeY) || 
            Double.IsNaN(farTimeX) || 
            Double.IsNaN(farTimeY) ||
            nearTimeX > farTimeY || nearTimeY > farTimeY)
        {
            return null;
        }

        float nearTime = nearTimeX > nearTimeY ? nearTimeX : nearTimeY;
        float farTime = farTimeX < farTimeY ? farTimeX : farTimeY;

        if (nearTime >= 1 || farTime <= 0)
        {
            return null;
        }

        Hit hit = new Hit(this);
        hit.time = Mathf.Clamp01(nearTime);
        if (nearTimeX > nearTimeY)
        {
            hit.normal.x = -signX;
            hit.normal.y = 0;
        }
        else
        {
            hit.normal.x = 0;
            hit.normal.y = -signY;
        }
        hit.delta.x = hit.time * delta.x;
        hit.delta.y = hit.time * delta.y;
        hit.position.x = position.x + hit.delta.x;
        hit.position.y = position.y + hit.delta.y;
        return hit;
    }

    public Hit IntersectAABB(AABB aabb)
    {
        float dx = aabb.position.x - this.position.x;
        float px = (aabb.half.x + this.half.x) - Mathf.Abs(dx);
        if (px <= 0)
        {
            return null;
        }

        float dy = aabb.position.y - this.position.y;
        float py = (aabb.half.y + this.half.y) - Mathf.Abs(dy);
        if (py <= 0)
        {
            return null;
        }

        Hit hit = new Hit(this);
        if (px < py)
        {
            float sx = Mathf.Sign(dx);
            hit.delta.x = px * sx;
            hit.normal.x = sx;
            hit.position.x = this.position.x + (this.half.x * sx);
            hit.position.y = aabb.position.y;
        } else
        {
            float sy = Mathf.Sign(dy);
            hit.delta.y = py * sy;
            hit.normal.y = sy;
            hit.position.x = aabb.position.x;
            hit.position.y = this.position.y + (this.half.y * sy);
        }
        return hit;
    }

    public Sweep SweepAABB(AABB aabb, Vector3 delta)
    {
        Sweep sweep = new Sweep();
        
        if (delta.x == 0 && delta.y == 0)
        {
            sweep.position.x = aabb.position.x;
            sweep.position.y = aabb.position.y;
            sweep.hit = this.IntersectAABB(aabb);
            if (sweep.hit != null)
            {
                sweep.time = sweep.hit.time = 0;
            } else
            {
                sweep.time = 1;
            }
            return sweep;
        }

        sweep.hit = this.IntersectSegment(aabb.position, delta, aabb.half.x, aabb.half.y);
        if (sweep.hit != null)
        {
            sweep.time = Mathf.Clamp01(sweep.hit.time - Mathf.Epsilon);
            sweep.position.x = aabb.position.x + delta.x * sweep.time;
            sweep.position.y = aabb.position.y + delta.y * sweep.time;
            Vector3 direction = delta.normalized;
            sweep.hit.position.x = Mathf.Clamp(
                sweep.hit.position.x + direction.x * aabb.half.x,
                this.position.x - this.half.x,
                this.position.x + this.half.x);
            sweep.hit.position.y = Mathf.Clamp(
                sweep.hit.position.y + direction.y * aabb.half.y,
                this.position.y - this.half.y,
                this.position.y + this.half.y);
        } else
        {
            sweep.position.x = aabb.position.x + delta.x;
            sweep.position.y = aabb.position.y + delta.y;
            sweep.time = 1;
        }
        return sweep;
    }
}