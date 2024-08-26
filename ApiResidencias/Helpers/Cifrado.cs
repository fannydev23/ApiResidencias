using System.Security.Cryptography;
using System.Text;

namespace ApiResidencias.Helpers
{
    public class Cifrado
    {
        private readonly int tamanyoClave = 32;
        private readonly int tamanyoVector = 16;
        private readonly string Vector = "8b5ef91df0fe09606482f383ace81f78b0acc7fc";

        private byte[] Key;
        private byte[] IV;

        public Cifrado()
        {
            IV = UTF8Encoding.UTF8.GetBytes(Vector);
            string clave = "ebb882770bf7e389793e5ca655834c3e6ff7f7f0" + DateTime.Now.ToString("ddMMyyhh");
            Key = UTF8Encoding.UTF8.GetBytes(clave);
        }

        public string CifradoTexto(String txtPlano)
        {
            Array.Resize(ref Key, tamanyoClave);
            Array.Resize(ref IV, tamanyoVector);

            // se instancia el Rijndael

            //Rijndael RijndaelAlg = Rijndael.Create();
            AesManaged aes = new AesManaged();

            // se establece cifrado

            MemoryStream memoryStream = new MemoryStream();

            // se crea el flujo de datos de cifrado

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                aes.CreateEncryptor(Key, IV),
                CryptoStreamMode.Write);

            // se obtine la información a cifrar

            byte[] txtPlanoBytes = UTF8Encoding.UTF8.GetBytes(txtPlano);

            // se cifran los datos

            cryptoStream.Write(txtPlanoBytes, 0, txtPlanoBytes.Length);

            cryptoStream.FlushFinalBlock();

            // se obtinen los datos cifrados

            byte[] cipherMessageBytes = memoryStream.ToArray();

            // se cierra todo

            memoryStream.Close();
            cryptoStream.Close();

            // Se devuelve la cadena cifrada

            return Convert.ToBase64String(cipherMessageBytes);
        }
        public string DescifradoTexto(String txtCifrado)
        {

            Array.Resize(ref Key, tamanyoClave);
            //Array.Resize(ref Key, 32);
            Array.Resize(ref IV, tamanyoVector);

            // se obtienen los bytes para el cifrado

            byte[] cipherTextBytes = Convert.FromBase64String(txtCifrado);

            // se almacenan los datos descifrados

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];


            // se crea una instancia del Rijndael			


            //Rijndael RijndaelAlg = Rijndael.Create();
            AesManaged aes = new AesManaged();

            // se crean los datos cifrados

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // se descifran los datos

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                aes.CreateDecryptor(Key, IV),
                CryptoStreamMode.Read);

            // se obtienen datos descifrados

            //int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length); 
            int readbytes = 0;
            int decryptedByteCount = 0;
            while (readbytes < plainTextBytes.Length)
            {
                decryptedByteCount = cryptoStream.Read(plainTextBytes, readbytes, plainTextBytes.Length - readbytes);
                if (decryptedByteCount == 0) break;
                readbytes += decryptedByteCount;
            }

            // se cierra todo

            memoryStream.Close();
            cryptoStream.Close();

            // se devuelve los datos descifrados

            return Encoding.UTF8.GetString(plainTextBytes, 0, readbytes);
        }
    }
}
