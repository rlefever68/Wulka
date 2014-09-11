namespace Wulka.Crypto
{
    public class Md5
    {
        public static string Encode(string str)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(str);
            bs = x.ComputeHash(bs);

            var sb = new System.Text.StringBuilder();
            foreach (var b in bs)
            {
                sb.Append(b.ToString("x2").ToLower());
            }

            return (sb.ToString());
        }

    }
}