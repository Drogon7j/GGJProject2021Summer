// File create date:2020/4/20
using UnityEngine;
// Created By Yu.Liu
namespace RoachGame.Data {
    /// <summary>
    /// 简单二元向量，用于特殊场景替代Vector2
    /// </summary>
    public struct SimpleVector2 {

        public float x;
        public float y;

        public SimpleVector2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2(SimpleVector2 data) {
            return new Vector2(data.x, data.y);
        }

        public override string ToString() {
            return $"({x},{y})";
        }
    }
    /// <summary>
    /// 简单三元向量，用于特殊场景替代Vector3
    /// </summary>
    public struct SimpleVector3 {

        public float x;
        public float y;
        public float z;

        public SimpleVector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(SimpleVector3 data) {
            return new Vector3(data.x, data.y, data.z);
        }

        public override string ToString() {
            return $"({x},{y},{z})";
        }
    }
}
