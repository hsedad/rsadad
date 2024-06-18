using System.Numerics;

namespace rsadad
{
    internal class GenExpEnc
    {
        internal int value;
        public GenExpEnc(BigInteger input) // метод подбора значения числа e взаимно простым с input
        {
            value = 0;
            if (GetComMaxDiv(input,17) == 1) { value = 17; } // если 17 взаимно простое с input, тогда e=17
            if (GetComMaxDiv(input, 257) == 1) { value = 257; } // если 257 взаимно простое с input, тогда e=257
            if (GetComMaxDiv(input, 65537) == 1) { value = 65537; } // если 65537 взаимно простое с input, тогда e=65537
            if (value > 16) { return; } // завершаем подбор числа e
            value = 11; //если "быстрые" значения числа e не подошли, тогда подбираем другие значения
            while (GetComMaxDiv(input, value) != 1) { value += 1;} //подбираем значение числа e
        }

        public BigInteger GetComMaxDiv(BigInteger a, int b) // метод поиска наибольшего общего делителя чисел a и b (алгоритм Евклида)
        {
            BigInteger _a = a;
            BigInteger _b = b;
            BigInteger _c = new BigInteger();
            _c = 1;
            while (_c != 0) // задаем цикл, пока остаток от целочисленного деления не ноль
            {
                if (_a > _b) { _c = _a % _b; _a = _c; } // если a > b тогда a = a % b
                else { _c = _b % _a; _b = _c; } // если же b > a тогда b = b % a
            }
            if (_a == 0) { return (int) _b; } // возвращаем значеие
            else { return (int) _a; }
        }

    }
}
