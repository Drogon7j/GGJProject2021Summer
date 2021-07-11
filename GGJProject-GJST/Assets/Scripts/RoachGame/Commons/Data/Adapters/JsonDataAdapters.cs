// File create date:2019/7/31
using UnityEngine;
/// <summary>
/// Vector2Int的Json适配类
/// </summary>
public class JsonVector2Int {

    public int x;
    public int y;

    public JsonVector2Int(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2Int(JsonVector2Int v) {
        return new Vector2Int(v.x, v.y);
    }

    public static JsonVector2Int operator +(JsonVector2Int left, Vector2Int right) {
        JsonVector2Int result = new JsonVector2Int(left.x, left.y);
        result.x += right.x;
        result.y += right.y;
        return result;
    }

    public static JsonVector2Int operator -(JsonVector2Int left, Vector2Int right) {
        JsonVector2Int result = new JsonVector2Int(left.x, left.y);
        result.x -= right.x;
        result.y -= right.y;
        return result;
    }

    public static JsonVector2Int operator *(JsonVector2Int left, int right) {
        JsonVector2Int result = new JsonVector2Int(left.x, left.y);
        result.x *= right;
        result.y *= right;
        return result;
    }

    public static JsonVector2Int operator /(JsonVector2Int left, int right) {
        JsonVector2Int result = new JsonVector2Int(left.x, left.y);
        result.x /= right;
        result.y /= right;
        return result;
    }
}

/// <summary>
/// Vector2的Json适配类
/// </summary>
public class JsonVector2 {

    public float x;
    public float y;

    public JsonVector2(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2(JsonVector2 v) {
        return new Vector2(v.x, v.y);
    }

    public static JsonVector2 operator +(JsonVector2 left, Vector2 right) {
        JsonVector2 result = new JsonVector2(left.x, left.y);
        result.x += right.x;
        result.y += right.y;
        return result;
    }

    public static JsonVector2 operator -(JsonVector2 left, Vector2 right) {
        JsonVector2 result = new JsonVector2(left.x, left.y);
        result.x -= right.x;
        result.y -= right.y;
        return result;
    }

    public static JsonVector2 operator *(JsonVector2 left, float right) {
        JsonVector2 result = new JsonVector2(left.x, left.y);
        result.x *= right;
        result.y *= right;
        return result;
    }

    public static JsonVector2 operator /(JsonVector2 left, float right) {
        JsonVector2 result = new JsonVector2(left.x, left.y);
        result.x /= right;
        result.y /= right;
        return result;
    }
}

/// <summary>
/// Vector3Int的Json适配类
/// </summary>
public class JsonVector3Int {

    public int x;
    public int y;
    public int z;

    public JsonVector3Int(int x, int y, int z) {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector3Int(JsonVector3Int v) {
        return new Vector3Int(v.x, v.y, v.z);
    }

    public static JsonVector3Int operator +(JsonVector3Int left, Vector3Int right) {
        JsonVector3Int result = new JsonVector3Int(left.x, left.y, left.z);
        result.x += right.x;
        result.y += right.y;
        result.z += right.z;
        return result;
    }

    public static JsonVector3Int operator -(JsonVector3Int left, Vector3Int right) {
        JsonVector3Int result = new JsonVector3Int(left.x, left.y, left.z);
        result.x -= right.x;
        result.y -= right.y;
        result.z -= right.z;
        return result;
    }

    public static JsonVector3Int operator *(JsonVector3Int left, int right) {
        JsonVector3Int result = new JsonVector3Int(left.x, left.y, left.z);
        result.x *= right;
        result.y *= right;
        result.z *= right;
        return result;
    }

    public static JsonVector3Int operator /(JsonVector3Int left, int right) {
        JsonVector3Int result = new JsonVector3Int(left.x, left.y, left.z);
        result.x /= right;
        result.y /= right;
        result.z /= right;
        return result;
    }
}

/// <summary>
/// Vector3的Json适配类
/// </summary>
public class JsonVector3 {

    public float x;
    public float y;
    public float z;

    public JsonVector3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator Vector3(JsonVector3 v) {
        return new Vector3(v.x, v.y, v.z);
    }

    public static JsonVector3 operator +(JsonVector3 left, Vector3 right) {
        JsonVector3 result = new JsonVector3(left.x, left.y, left.z);
        result.x += right.x;
        result.y += right.y;
        result.z += right.z;
        return result;
    }

    public static JsonVector3 operator -(JsonVector3 left, Vector3 right) {
        JsonVector3 result = new JsonVector3(left.x, left.y, left.z);
        result.x -= right.x;
        result.y -= right.y;
        result.z -= right.z;
        return result;
    }

    public static JsonVector3 operator *(JsonVector3 left, float right) {
        JsonVector3 result = new JsonVector3(left.x, left.y, left.z);
        result.x *= right;
        result.y *= right;
        result.z *= right;
        return result;
    }

    public static JsonVector3 operator /(JsonVector3 left, float right) {
        JsonVector3 result = new JsonVector3(left.x, left.y, left.z);
        result.x /= right;
        result.y /= right;
        result.z /= right;
        return result;
    }
}