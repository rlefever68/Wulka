namespace Wulka.Crypto
{
    public partial class CryptoEngine
    {
        // we'd like symmetric encryption, so we setup our own encryption key and IV
        private static byte[] _key = new byte[8] { 42, 182, 33, 19, 97, 11, 101, 209 };
        private static byte[] _IV = new byte[8] { 36, 64, 32, 151, 18, 13, 73, 41 };

    }
}
