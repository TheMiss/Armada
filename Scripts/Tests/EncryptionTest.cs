using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    public class EncryptionTest : MonoBehaviour
    {
        public TextAsset JsonAsset;

        private Encryption m_encryption;
        
        [Button]
        private void RunTest1()
        {
            string originalText = JsonAsset.text;
            WriteFile("Original.txt", originalText);
            
            string compressedText = Zip.CompressToBase64(JsonAsset.text);
            WriteFile("Compressed.txt", compressedText);

            string decompressedText = Zip.DecompressFromBase64(compressedText);
            WriteFile("DecompressedText.txt", decompressedText);
            
            Debug.Log("Done");
        }

        [Button]
        private void RunTest2()
        {
            EncryptFile("HelloDarkness!!!", "HelloDarkness!!!");
        }

        [Button]
        private void RunTest3()
        {
            EncryptFile("HelloDarkness!!!", "Lately99!!!");
        }
        
        [Button]
        private void RunTest4()
        {
            const string password = "HelloDarknessMyOldFriend.";
            m_encryption = new Encryption(password);
            string encryptedText = m_encryption.EncryptString(JsonAsset.text);
            WriteFile("_1_encryptedText.txt", encryptedText);
            
            string compressedEncryptedText = Zip.CompressToBase64(encryptedText);
            WriteFile("_2_compressed_encryptedText.txt", compressedEncryptedText);

            m_encryption.SetPassword(password);
            string decompressedEncryptedText = Zip.DecompressFromBase64(compressedEncryptedText);
            WriteFile("_3_decompressed_decryptedText.txt", decompressedEncryptedText);
            
            string decryptedText = m_encryption.DecryptString(decompressedEncryptedText);
            WriteFile("_4_decryptedText.txt", decryptedText);
        }

        [Button]
        private void RunTest5()
        {
            string compressedText = Zip.CompressToBase64(JsonAsset.text);
            WriteFile("__1_compressedText.txt", compressedText);
            
            const string password = "HelloDarknessMyOldFriend.";
            m_encryption = new Encryption(password);
            string encryptedCompressedText = m_encryption.EncryptString(compressedText);
            WriteFile("__2_encryptedCompressedText.txt", encryptedCompressedText);
            
            
            m_encryption.SetPassword(password);
            string decryptedCompressedText = m_encryption.DecryptString(encryptedCompressedText);
            WriteFile("__3_decryptedCompressedText.txt", decryptedCompressedText);
            
            string decompressed = Zip.DecompressFromBase64(decryptedCompressedText);
            WriteFile("__4_decompressed.txt", decompressed);
        }
        
        private void EncryptFile(string encryptPassword, string decryptPassword)
        {
            m_encryption = new Encryption(encryptPassword);
            string encryptedText = m_encryption.EncryptString(JsonAsset.text);
            Debug.Log($"encryptedText=\n{encryptedText}");
            WriteFile("encryptedText.txt", encryptedText);

            m_encryption.SetPassword(decryptPassword);
            string decryptedText = m_encryption.DecryptString(encryptedText);
            Debug.Log($"decryptedText=\n{decryptedText}");
            WriteFile("decryptedText.txt", decryptedText);
        }

        private void WriteFile(string fileName, string content)
        {
            DeviceFile.WriteAllText($"Tests/{fileName}", content);
        }
    }
}
