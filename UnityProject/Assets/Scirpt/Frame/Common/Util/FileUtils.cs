using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameFrameWork.Utils
{
    public class FileUtils
    {
        public static byte[] ReadFile(string fullPath, int readSize = 0)
        {
            byte[] buffer = null;
            if (File.Exists(fullPath))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    if (readSize == 0 || readSize >= fs.Length)
                    {
                        buffer = new byte[fs.Length];
                    }
                    else
                    {
                        buffer = new byte[readSize];
                    }

                    fs.Read(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    Debuger.LogError("ReadFile() Path:{0}, Error:{1}", fullPath, e.Message);
                }
                finally
                {
                    fs?.Close();
                }
            }
            else
            {
                Debuger.LogError("ReadFile() File is Not Exist: {0}", fullPath);
            }

            return buffer;
        }


        public static string[] ReadFileLines(string fullPath)
        {
            List<string> listLines = new List<string>();
            if (File.Exists(fullPath))
            {
                StreamReader fs = null;

                try
                {
                    fs = new StreamReader(fullPath);
                    while (fs.Peek() > 0)
                    {
                        listLines.Add(fs.ReadLine());
                    }
                }
                catch (Exception e)
                {
                    Debuger.LogError("ReadFileLines() Path:{0}, Error:{1}", fullPath, e.Message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            else
            {
                Debuger.LogError("ReadFileLines() File is Not Exist: {0}", fullPath);
            }

            return listLines.ToArray();
        }


        public static string ReadString(string fullPath)
        {
            byte[] buffer = ReadFile(fullPath);
            if (buffer != null)
            {
                return Encoding.UTF8.GetString(buffer);
            }

            return "";
        }


        public static string ReadStringASCII(string fullPath)
        {
            byte[] buffer = ReadFile(fullPath);
            if (buffer != null)
            {
                return Encoding.ASCII.GetString(buffer);
            }

            return "";
        }

        public static int SaveFile(string fullpath, byte[] content)
        {
            if (content == null)
            {
                content = new byte[0];
            }

            string dir = PathUtils.GetParentDir(fullpath);

            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception e)
                {
                    Debuger.LogError("SaveFile() CreateDirectory Error! Dir:{0}, Error:{1}", dir, e.Message);
                    return -1;
                }
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                fs.Write(content, 0, content.Length);
            }
            catch (Exception e)
            {
                Debuger.LogError("SaveFile() Path:{0}, Error:{1}", fullpath, e.Message);
                fs.Close();
                return -1;
            }

            fs.Close();
            return content.Length;
        }


        public static int SaveFile(string fullpath, string content)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            return SaveFile(fullpath, buffer);
        }

        public static List<string> GetFileFullNames(string dir, SearchOption option, string searchPattern,
            string[] listIgnoredSuffix = null)
        {
            List<string> result = new List<string>();
            var list = GetFiles(dir, option, searchPattern, listIgnoredSuffix);
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(list[i].FullName);
            }

            return result;
        }

        public static List<string> GetFileNames(string dir, SearchOption option, string searchPattern,
            string[] listIgnoredSuffix = null)
        {
            List<string> result = new List<string>();
            var list = GetFiles(dir, option, searchPattern, listIgnoredSuffix);
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(list[i].Name);
            }

            return result;
        }

        public static List<FileInfo> GetFiles(string dir, SearchOption option, string searchPattern,
            string[] listIgnoredSuffix = null)
        {
            List<FileInfo> result = new List<FileInfo>();

            if (Directory.Exists(dir))
            {
                DirectoryInfo directory = new DirectoryInfo(dir);
                FileInfo[] files = directory.GetFiles(searchPattern, option);

                int len = files.Length;
                for (int i = 0; i < len; i++)
                {
                    FileInfo f = files[i];
                    bool ignored = false;
                    if (listIgnoredSuffix != null)
                    {
                        int lenIgnored = listIgnoredSuffix.Length;
                        for (int j = 0; j < lenIgnored; j++)
                        {
                            if (f.FullName.EndsWith(listIgnoredSuffix[j]))
                            {
                                ignored = true;
                                break;
                            }
                        }
                    }

                    if (!ignored)
                    {
                        result.Add(f);
                    }
                }
            }

            return result;
        }


        public static void EnsureDirectory(string dirPath)
        {
            dirPath = dirPath.Replace('\\', '/');
            var tokens = dirPath.Split('/');

            string dstTempPath = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                dstTempPath += tokens[i] + "/";
                if (!Directory.Exists(dstTempPath))
                {
                    Directory.CreateDirectory(dstTempPath);
                }
            }
        }

        public static void EnsureFileDirectory(string filePath)
        {
            filePath = filePath.Replace('\\', '/');
            var tokens = filePath.Split('/');

            string dstTempPath = "";
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                dstTempPath += tokens[i] + "/";
                if (!Directory.Exists(dstTempPath))
                {
                    Directory.CreateDirectory(dstTempPath);
                }
            }
        }


        public static bool Copy(string srcPath, string dstPath)
        {
            if (!File.Exists(srcPath))
            {
                return false;
            }

            EnsureFileDirectory(dstPath);

            try
            {
                File.Copy(srcPath, dstPath);
                return true;
            }
            catch (Exception e)
            {
                Debuger.LogError(e.Message);
            }

            return false;
        }
    }
}