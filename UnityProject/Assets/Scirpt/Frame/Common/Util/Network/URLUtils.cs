namespace GameFrameWork.Utils
{
    public class UrlUtils
    {
        public static readonly string[] UrlHeadDefine = {"http://", "https://"};

        public static void SplitUrl(string url, out string head, out string host, out string port, out string path)
        {
            int a = url.IndexOf("://");

            if (a >= 0)
            {
                head = url.Substring(0, a + 3);
                url = url.Substring(a + 3);
            }
            else
            {
                head = "";
            }

            a = url.IndexOf("/");
            if (a >= 0)
            {
                host = url.Substring(0, a);
                path = url.Substring(a);
            }
            else
            {
                host = url;
                path = "";
            }

            a = host.LastIndexOf(":");
            if (a >= 0)
            {
                port = host.Substring(a + 1);
                host = host.Substring(0, a);
            }
            else
            {
                port = "";
            }

            Debuger.Log("URLUtils", "SplitUrl() head=" + head +
                                    ", host=" + host + ", port=" + port + ", path=" + path);
        }
    }
}