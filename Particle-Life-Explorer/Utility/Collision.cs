using System.Numerics;

namespace Particle_Life_Explorer.Utility
{
    static class Collision
    {
        static bool PointInLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 startDir = Vector2.Normalize(lineStart - point);
            Vector2 endDir = Vector2.Normalize(point - lineEnd);
            return (startDir == endDir);
        }


        static bool PointInRect(int x, int y, int rect_x, int rect_y, int rect_width, int rect_height)
        {
            if (x < rect_x)
                return false;
            else if (x > rect_x + rect_width)
                return false;
            else if (y < rect_y)
                return false;
            else if (y > rect_y + rect_height)
                return false;
            else
                return true;
        }


        static bool PointInCircle(Vector2 point, Vector2 center, float r)
        {
            return ((point - center).LengthSquared() <= r * r);
        }


        static bool LineInCircle(Vector2 lineStart, Vector2 lineEnd, Vector2 center, float r)
        {
            // Method based on example here - http://doswa.com/2009/07/13/circle-segment-intersectioncollision.html
            Vector2 seg_v = lineEnd - lineStart;
            Vector2 pt_v = center - lineStart;

            float scalarProjection = Vector2.Dot(pt_v, seg_v) * seg_v.Length();
            Vector2 projection = scalarProjection * Vector2.Normalize(seg_v);

            Vector2 closest;
            if (scalarProjection < 0)
                closest = lineStart;
            else if (scalarProjection * scalarProjection > seg_v.LengthSquared())
                closest = lineEnd;
            else
                closest = lineStart + projection;

            Vector2 dist_v = center - closest;

            return (dist_v.LengthSquared() <= r * r);
        }


        static bool RectInRect(float rect1_x, float rect1_y, float rect1_w, float rect1_h,
                             float rect2_x, float rect2_y, float rect2_w, float rect2_h)
        {
            return(rect1_x + rect1_w >= rect2_x && rect1_x <= rect2_x + rect2_w &&
                    rect1_y + rect1_h >= rect2_y && rect1_y <= rect2_y + rect2_h);
        }
}
}
