// File create date:5/13/2019
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
// Created By Yu.Liu
namespace RoachGame.Utils {
    /// <summary>
    /// 文件操作工具集
    /// </summary>
    public static class FileUtils {
        /// <summary>
        /// 检查指定路径是否为目录且存在
        /// </summary>
        /// <param name="path">检查路径</param>
        /// <returns>是否为目录且存在</returns>
        public static bool CheckDirectory(string path) {
            return Directory.Exists(path);
        }
        /// <summary>
        /// 检查指定路径是否为文件且存在
        /// </summary>
        /// <param name="path">检查路径</param>
        /// <returns>是否为文件且存在</returns>
        public static bool CheckFile(string path) {
            return File.Exists(path);
        }
        /// <summary>
        /// 创建指定目录
        /// </summary>
        /// <param name="dirPath">指定目录路径</param>
        /// <returns>目录信息</returns>
        public static DirectoryInfo CreateDirectory(string dirPath) {
            Directory.CreateDirectory(dirPath);
            return new DirectoryInfo(dirPath);
        }
        /// <summary>
        /// 删除指定目录
        /// </summary>
        /// <param name="dirPath">指定目录路径</param>
        /// <param name="isRecursive">是否递归删除，即是否删除子文件夹</param>
        /// <returns>是否成功删除</returns>
        public static bool DeleteDirectory(string dirPath, bool isRecursive = false) {
            try {
                Directory.Delete(dirPath, isRecursive);
                return true;
            } catch (Exception e) {
                LogUtils.LogError($"Cannot Delete Directory at {dirPath}!\n{e.Message}\n{e.StackTrace}");
            }
            return false;
        }
        /// <summary>
        /// 读取或者创建文件为流
        /// </summary>
        /// <param name="filePath">指定文件路径</param>
        /// <param name="mode">读取模式，默认为打开或创建</param>
        /// <returns>文件流对象，为空表示没有读取到文件</returns>
        public static FileStream StreamFile(string filePath, FileMode mode = FileMode.OpenOrCreate) {
            return new FileStream(filePath, mode);
        }
        /// <summary>
        /// 读取文件并将内容转化为字符串列表，请预先检查文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>字符串列表</returns>
        public static List<string> FileToList(string filePath) {
            var reader = new StreamReader(filePath);
            var result = new List<string>();
            string row;
            while ((row = reader.ReadLine()) != null) {
                if (TextUtils.HasData(row)) {
                    result.Add(row);
                }
            }
            reader.Close();
            return result;
        }
        /// <summary>
        /// 读取文件并将内容转化为字符串列表，请预先检查文件内容
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>字符串列表</returns>
        public static List<string> FileToList(FileStream fs) {
            var reader = new StreamReader(fs);
            var result = new List<string>();
            string row;
            while ((row = reader.ReadLine()) != null) {
                if (TextUtils.HasData(row)) {
                    result.Add(row);
                }
            }
            reader.Close();
            return result;
        }
        /// <summary>
        /// 读取文件并将内容转化为字符串，可以指定每一行的分隔符，默认为逗号
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="seperator">分隔符</param>
        /// <returns>字符串</returns>
        public static string FileToString(string filePath, char seperator = ',') {
            var builder = new StringBuilder();
            var reader = new StreamReader(filePath);
            var row = reader.ReadLine();
            if (row != null) {
                builder.Append(row);
                while ((row = reader.ReadLine()) != null) {
                    builder.Append(seperator);
                    builder.Append(row);
                }
            }
            reader.Close();
            return builder.ToString();
        }
        /// <summary>
        /// 读取文件并将内容转化为字符串，可以指定每一行的分隔符，默认为逗号
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <param name="seperator">分隔符</param>
        /// <returns>字符串</returns>
        public static string FileToString(FileStream fs, char seperator = ',') {
            var builder = new StringBuilder();
            var reader = new StreamReader(fs);
            var row = reader.ReadLine();
            if (row != null) {
                builder.Append(row);
                while ((row = reader.ReadLine()) != null) {
                    builder.Append(seperator);
                    builder.Append(row);
                }
            }
            reader.Close();
            return builder.ToString();
        }
        /// <summary>
        /// 读取文件并将内容转化为byte数组
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>byte数组</returns>
        public static byte[] FileToBytes(FileStream fs) {
            var size = (int)fs.Length;
            var array = new byte[size];
            fs.Read(array, 0, size);
            fs.Close();
            return array;
        }
        /// <summary>
        /// 向文件流写入文本数据，
        /// </summary>
        /// <param name="fs">目标文件流</param>
        /// <param name="strList">待写入字符串列表</param>
        /// <param name="isReplace">是否替换，如果为真会从头开始写入数据</param>
        public static void WriteText(FileStream fs, List<string> strList, bool isReplace = false) {
            if (isReplace) {
                // 替换写入，调整流指针到开头
                fs.Seek(0, SeekOrigin.Begin);
            } else {
                // 后续写入，调整流指针到结尾
                fs.Seek(0, SeekOrigin.End);
            }
            var writer = new StreamWriter(fs);
            foreach (var str in strList) {
                writer.WriteLine(str);
            }
            writer.Flush();
            writer.Close();
        }
        /// <summary>
        /// 向文件流写入文本数据
        /// </summary>
        /// <param name="fs">目标文件流</param>
        /// <param name="str">待写入字符串</param>
        /// <param name="isReplace">是否替换，如果为真会从头开始写入数据</param>
        public static void WriteText(FileStream fs, string str, bool isReplace = false) {
            if (isReplace) {
                // 替换写入，调整流指针到开头
                fs.Seek(0, SeekOrigin.Begin);
            } else {
                // 后续写入，调整流指针到结尾
                fs.Seek(0, SeekOrigin.End);
            }
            var writer = new StreamWriter(fs);
            writer.WriteLine(str);
            writer.Flush();
            writer.Close();
        }
        /// <summary>
        /// 向文件流写入byte数组，固定为从头覆盖写入
        /// </summary>
        /// <param name="fs">目标文件流</param>
        /// <param name="array">byte数组</param>
        public static void WriteBytes(FileStream fs, byte[] array) {
            fs.Seek(0, SeekOrigin.Begin);
            if (array != null) {
                fs.Write(array, 0, array.Length);
            }
            fs.Flush();
            fs.Close();
        }
    }
}
