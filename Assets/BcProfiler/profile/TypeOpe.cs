using UnityEngine;
using System.Collections;
using System;

public class TypeOpe
{
    public static string TypeToString(string type, object val)
    {
        string valStr = "";
        try
        {
            if ("UnityEngine.Color" == type)
            {
                Color color = (Color)val;
                valStr = string.Format("{0}, {1}, {2}, {3}", color.r, color.g, color.b, color.a);
            }
            else if ("UnityEngine.Vector2" == type)
            {
                Vector2 vec = (Vector2)val;
                valStr = string.Format("{0}, {1}", vec.x, vec.y);
            }
            else if ("UnityEngine.Vector3" == type)
            {
                Vector3 vec = (Vector3)val;
                valStr = string.Format("{0}, {1}, {2}", vec.x, vec.y, vec.z);
            }
            else if ("UnityEngine.Vector4" == type)
            {
                Vector4 vec = (Vector4)val;
                valStr = string.Format("{0}, {1}, {2}, {3}", vec.x, vec.y, vec.z, vec.w);
            }
            else if ("UnityEngine.Rect" == type)
            {
                Rect rec = (Rect)val;
                valStr = string.Format("{0}, {1}, {2}, {3}", rec.x, rec.y, rec.width, rec.height);
            }
            else
                valStr = val.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError("TypeToString error: "+e.ToString());
        }

        return valStr;
    }

    public static object StringToType(Type type, string val)
    {
        if (typeof(UnityEngine.Color) == type)
        {
            Color tmp;
            if (ColorTryParse(val, out tmp))
                return (object)tmp;
        }
        else if (typeof(UnityEngine.Vector2) == type)
        {
            Vector2 tmp;
            if (Vector2TryParse(val, out tmp))
                return (object)tmp;
        }
        if (typeof(UnityEngine.Vector3) == type)
        {
            Vector3 tmp;
            if (Vector3TryParse(val, out tmp))
                return (object)tmp;
        }
        if (typeof(UnityEngine.Vector4) == type)
        {
            Vector4 tmp;
            if (Vector4TryParse(val, out tmp))
                return (object)tmp;
        }
        if (typeof(UnityEngine.Rect) == type)
        {
            Rect tmp;
            if (RectTryParse(val, out tmp))
                return (object)tmp;
        }
        else if (typeof(System.Single) == type)
        {
            System.Single tmp;
            if (System.Single.TryParse(val, out tmp))
                return (object)tmp;
        }
        else if (typeof(System.Boolean) == type)
        {
            System.Boolean tmp;
            if (System.Boolean.TryParse(val, out tmp))
                return (object)tmp;
        }
        else if (typeof(System.Int32) == type)
        {
            System.Int32 tmp;
            if (System.Int32.TryParse(val, out tmp))
                return (object)tmp;
        }
        else if (typeof(System.UInt32) == type)
        {
            System.UInt32 tmp;
            if (System.UInt32.TryParse(val, out tmp))
                return (object)tmp;
        }
        else if (typeof(System.String) == type)
        {
                return (object)val;
        }

        return null;
    }

    public static bool ColorTryParse(string val, out Color color)
    {
        color = Color.clear;
        try
        {
            string[] paras = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            color.r = float.Parse(paras[0]);
            color.g = float.Parse(paras[1]);
            color.b = float.Parse(paras[2]);
            color.a = float.Parse(paras[3]);
            return true;
        }
        catch (System.Exception)
        {
        }

        return false;
    }

    public static bool Vector2TryParse(string val, out Vector2 vec3)
    {
        vec3 = Vector2.zero;
        try
        {
            string[] paras = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            vec3.Set(float.Parse(paras[0]), float.Parse(paras[1]));
            return true;
        }
        catch (System.Exception)
        {
        }

        return false;
    }

    public static bool Vector3TryParse(string val, out Vector3 vec3)
    {
        vec3 = Vector3.zero;
        try
        {
            string[] paras = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            vec3.Set(float.Parse(paras[0]), float.Parse(paras[1]), float.Parse(paras[2]));
            return true;
        }
        catch (System.Exception)
        {
        }

        return false;
    }

    public static bool Vector4TryParse(string val, out Vector4 vec3)
    {
        vec3 = Vector4.zero;
        try
        {
            string[] paras = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            vec3.Set(float.Parse(paras[0]), float.Parse(paras[1]), float.Parse(paras[2]), float.Parse(paras[3]));
            return true;
        }
        catch (System.Exception)
        {
        }

        return false;
    }

    public static bool RectTryParse(string val, out Rect vec3)
    {
        vec3 = new Rect(0f,0f,0f,0f);
        try
        {
            string[] paras = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            vec3.Set(float.Parse(paras[0]), float.Parse(paras[1]), float.Parse(paras[2]), float.Parse(paras[3]));
            return true;
        }
        catch (System.Exception)
        {
        }

        return false;
    }
}
