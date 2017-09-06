using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace crypt
{
    public partial class MainFrame : Form
    {
        private readonly string saltKey = "puvZU9eT4puvZUal0HJs8t3";
        private readonly string viKey = "mSbuxJ42lK8URHKh";


        private string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(txtPassKey.Text, Encoding.ASCII.GetBytes(saltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(viKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        private string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(txtPassKey.Text, Encoding.ASCII.GetBytes(saltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(viKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        public MainFrame()
        {
            InitializeComponent();
        }

        private void txtPassKey_Click(object sender, EventArgs e)
        {
            txtPassKey.SelectAll();
        }

        private void txtInput_Click(object sender, EventArgs e)
        {
            txtInput.SelectAll();
        }

        private void txtOutput_Click(object sender, EventArgs e)
        {
            txtOutput.SelectAll();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            txtOutput.Text = Encrypt(txtInput.Text);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            txtOutput.Text = Decrypt(txtInput.Text);
        }
    }
}