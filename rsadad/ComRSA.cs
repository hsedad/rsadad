using System.Numerics;

namespace rsadad
{
    internal class ComRSA
    {
        public BigInteger GetExpDec(BigInteger phi, BigInteger e) // метод реализует расширенный алгоритм Eвклида
        {
            BigInteger q, r, y, _phi, _e, y2, y1;
            _phi = phi; _e = e; y2 = 0; y1 = 1; r = 1; y = 0; q = 0; // определяем переменные
            do
            {
                q = _phi / _e; r = _phi % _e; // производим вычисления и замены
                y = y2 - q * y1; _phi = _e; _e = r;
                y2 = y1; y1 = y;
            }
            while (r != 0); // пока остаток от деления не будет равен 0
            if (y2 < 0) { y2 += phi;} // если результат получился отрицательным, приводим его к положительным значениям
                return y2; 
        }

        public byte[] Encrypt(byte[] input, BigInteger n, BigInteger e) // метод шифрования согласно алгоритма RSA
        {
            long block_len = (long) BigInteger.Log2(n); // длина блока исходного сообщения в битах = log2(n)
            long nob = input.Length * 8 / block_len; // количество блоков сообщения

            BigInteger message = new BigInteger(ReverseBytes(input)); // записываем массив входных байт сообщения в одно число

            BigInteger bit_mask = BigInteger.Pow(2, (int) block_len) - 1;  // получаем маску из block_len бит (блок зашифрования)

            BigInteger[] messages = new BigInteger[nob + 1]; // создаем массив из блоков

            for (int i = 0; i < messages.Length; i++) // формируем из исходного сообщения блоки
            {
                messages[i] = message & bit_mask; // берем исходное сообщение, накладываем маску и получаем блок
                message >>= (int) block_len; // сдвигаем исходное сообщение на число бит в блоке вправо
            }

            BigInteger[] ciphertexts = new BigInteger[nob + 1]; // создаем массив больших чисел для зашифрованных блоков
            BigInteger ciphertext = new BigInteger(); // большое число для зашифрованного сообщения

            for (int i = 0; i < ciphertexts.Length; i++) // шифруем блоки
            {
                ciphertexts[i] = BigInteger.ModPow(messages[i], e, n); // блок сообщения возводим в степень e и приводим по модулю n
                ciphertext <<= (int)block_len; // делаем сдвиг шифртекста на длину блока
                ciphertext |= ciphertexts[i]; // и записываем в конец шифртекста зашифрованный блок
            }
            byte[] result = ciphertext.ToByteArray(); // выходной массив байт - шифртекст
            return ReverseBytes(result); // разворачиваем массив и возвращаем результат
        }

        public byte[] Decrypt(byte[] input, BigInteger n, BigInteger d) // метод расшифрования согласно алгоритма RSA
        {
            long block_len = (long) BigInteger.Log2(n); // длина блока исходного шифртекста в битах = log2(n)
            long nob = input.Length * 8 / block_len; // количество блоков шифртекста

            BigInteger ciphertext = new BigInteger(ReverseBytes(input)); // записываем массив входных байт шифртекста в одно число

            BigInteger bit_mask = BigInteger.Pow(2, (int)block_len) - 1;  // получаем маску из block_len бит (блок расшифрования)

            BigInteger[] ciphertexts = new BigInteger[nob]; // создаем массив из блоков (здесь отличие от процедуры зашифрования)

            for (int i = 0; i < ciphertexts.Length; i++) // формируем из исходного шифртекста блоки
            {
                ciphertexts[i] = ciphertext & bit_mask; // берем исходный шифртекст, накладываем маску и получаем блок
                ciphertext >>= (int)block_len; // сдвигаем исходный шифртекст на число бит в блоке вправо
            }

            BigInteger[] messages = new BigInteger[nob]; // создаем массив больших чисел для расшифрованных блоков
            BigInteger message = new BigInteger(); // большое число для расшифрованного сообщения

            for (int i = 0; i < ciphertexts.Length; i++) // расшифровываем блоки
            {
                messages[i] = BigInteger.ModPow(ciphertexts[i], d, n); // блок шифртекста возводим в степень d и приводим по модулю n
                message <<= (int)block_len; // делаем сдвиг сообщения на длину блока
                message |= messages[i]; // и записываем в конец сообщения расшифрованный блок
            }

            byte[] result = message.ToByteArray(); // выходной массив байт - расшифрованное сообщение
            return ReverseBytes(result); // разворачиваем массив и возвращаем результат
        }

        public byte[] ReverseBytes(byte[] bytes) // метод для "разворота" массива
        {
            byte[] result = new byte[bytes.Length]; // т.к. метод BigInteger.ToByteArray записывает элементы в обратном порядке
            int j = bytes.Length - 1;
            for (int i = 0;i < bytes.Length; i++)
            {
                result[j] = bytes[i]; j--;
            }
            return result;
        }
    }
}
