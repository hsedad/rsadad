using System.Numerics;

namespace rsadad
{
    internal class GenPrime
    {
        internal BigInteger value;

        public GenPrime(int length, int k) // метод, который генерирует простое число, используя последующие
        {
            BigInteger result = GenBigInteger(length); // генерируем число заданной битовой длины
            if (result.IsEven) result--; // если число четное, то уменьшаем его на 1
            while (!IsPrime(result, k))  // запускаем цикл с проверкой на простоту
            {
                result += 2; //добавляем к числу 2 пока не получим простое число
            }
            value = result; //возвращаем результат
        }

        public BigInteger GenBigInteger(int length) // метод, который создает случайное число заданной длиной бит
        {
            if (length < 1) return BigInteger.Zero; // рассматриваем даже нулевые значения

            int bytes_length = (length / 8) + 1; // число байт в числе (добавляем 1, т.к. при использовании int 1 теряется на знак 
            int bits_length = length % 8; // число оставшихся бит (если длина не кратна 8 битам)
            int bigint_len = bytes_length; //длина числа в байт
            if (bits_length > 0) bigint_len ++; // если длина не кратна 8 бит, добавляем ещё байт

            Random rnd = new Random(); // клас генерации случайных чисел
            byte[] bytes = new byte[bigint_len]; // создаем массив байт (наше большое число)
            rnd.NextBytes(bytes); // заполняем массив байт случайными значениями

            byte mask = (byte)(0xFF >> (8 - bits_length)); // формируем "голову" числа - маска для числа значащих бит
            bytes[bigint_len - 1] &= mask; // выделяем значащие биты, остальные обнуляем

            return new BigInteger(bytes); //преобразуем получившийся массив байт в число из length бит
        }

        public bool IsPrime(BigInteger n, int k) // метод реализующий тест Миллера-Рабина на простоту числа с числом раундов k
        {
            if (n == 2 || n == 3) // проверяем число на 2 и 3 
                return true;
            if (n < 2 || n % 2 == 0) // проверяем число на 1 и четность
                return false;

            BigInteger d = n - 1; // число d в тесте Миллера-Рабина
            int s = 0; // показатель 2 в тесте 

            while (d % 2 == 0) // ищем числа d и s
            {
                d /= 2;
                s += 1;
            }

            BigInteger a;

            for (int i = 0; i < k; i++) // цикл A теста Миллера-Рабина
            {
                do
                {
                    a = GenBigInteger((int)n.GetBitLength()); // генерируем число a 
                }
                while (a < 2 || a >= n - 2); // пока не получим a<2 или a>=n-2

                BigInteger x = BigInteger.ModPow(a, d, n); // получаем x = a^d(mod n)
                if (x == 1 || x == n - 1) // проверяем x и в случае равенства переходим к следующему шагу цикла А
                    continue;

                for (int r = 1; r < s; r++) // цикл B теста Миллера-Рабина
                {
                    x = BigInteger.ModPow(x, 2, n); // вычисляем x <- x^2 (mod n)
                    if (x == 1)
                        return false; // если ==1 то число составное
                    if (x == n - 1)
                        break; // если равно n-1 то перейти на следующий шаг цикла А
                }

                if (x != n - 1)
                    return false; // если не равно n-1 то число составное
            }

            return true; // если прошли все проверки, то число с вероятностью 4^(-k) простое
        }
    }
}
