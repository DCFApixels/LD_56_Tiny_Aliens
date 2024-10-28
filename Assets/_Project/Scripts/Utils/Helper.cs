using UnityEngine;

public static class Helper
{
    public static Vector3 SetZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }

    public static bool Inside(this Vector3 v, Vector3 center, Vector3 size)
    {
        if (v.x < center.x - size.x / 2f || v.x > center.x + size.x / 2f)
        {
            return false;
        }

        if (v.y < center.y - size.y / 2f || v.y > center.y + size.y / 2f)
        {
            return false;
        }

        return true;
    }


    //private const float C = 100f;
    public static void SetPosition2D(this Transform self, Vector2 position, float height)
    {
        self.position = position.To2D(height);
    }
    public static void SetPosition2D(this Transform self, Vector2 position)
    {
        Vector3 p = self.position;
        float h = (p.y - p.z) * 2f;
        self.position = position.To2D(h);
    }
    public static Vector3 GetPosition2D(this Transform self)
    {
        return (Vector2)self.position;
    }
    public static Vector3 To2D(this Vector3 self, float height)
    {
        self.z = self.y - (height / 2f);

        //self.z = Mathf.Round(self.z * C) / C;
        return self;
    }
    public static Vector3 To2D(this Vector2 self, float height)
    {
        Vector3 result = new Vector3(self.x, self.y, self.y - (height / 2f));

        //result.z = Mathf.Round(result.z * C) / C;
        return result;
    }
}