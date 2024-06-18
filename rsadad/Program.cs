using rsadad;
using System.Numerics;
using System.Text;

bool encrypt = false, decrypt = false, genkey = false, man = false, test = true;
string message_file = "message.txt";
string cipher_file = "cipher.dat";
string uncipher_file = "uncipher.txt";
string e_file = "e-key.key";
string d_file = "d-key.key";
string n_file = "n-key.key";
int key_length = 32, k = 32;

if (args.Length != 6) // проверяем количество аргументов консольного приложения и их корректность
{
    Console.WriteLine("Using program: rsadad -e input file -ef e-file -nf n-file (for encryption)");
    Console.WriteLine("Using program: rsadad -d input file -df d-file -nf n-file (for decryption)");
    Console.WriteLine("Using program: rsadad -g e-file -nf n-file -l key length (512, 1024, ...");
}
else
{
    if (!args[0].Contains("-e") && !args[0].Contains("-d") && !args[0].Contains("-g"))
    { Console.WriteLine("Using program: rsadad -e(-d) input file -ef(df) e(d)-file -nf n-file");
      Console.WriteLine("or using program: rsadad -g e-file -nf n-file -l key length"); return;}
    if (!args[2].Contains("-ef") && !args[2].Contains("-df") && !args[2].Contains("-nf"))
    { Console.WriteLine("Using program: rsadad -e(-d) input file -ef(df) e(d)-file -nf n-file");
      Console.WriteLine("or using program: rsadad -g e-file -nf n-file -l key length"); return;}
    if (!File.Exists(args[1]) && !args[1].Contains("-g"))
    { Console.WriteLine("File for encryption/decryption not found"); return; }
    if (!File.Exists(args[3]) && !File.Exists(args[5]) && !args[1].Contains("-g"))
    { Console.WriteLine("Key files not found"); return; }
    if (args[0] == "-e") { encrypt = true; message_file = args[1]; e_file = args[3]; n_file = args[5]; }
    if (args[0] == "-d") { decrypt = true; cipher_file = args[1]; d_file = args[3]; n_file = args[5]; }
    if (args[0] == "-g") { e_file = args[1]; n_file = args[3]; key_length = int.Parse(args[5]); genkey = true; }
}

BigInteger p, q, n, d, phi, e = 0; // переменные для алгоритма RSA
ComRSA comRSA = new ComRSA();

if (genkey) // действия, если в строке параметров задана генерация ключей
{
    GenPrime prime = new GenPrime(key_length, k);
    p = prime.value;
    prime = new GenPrime(key_length - 5 , k);
    q = prime.value;
    n = p * q;
    phi = (p - 1) * (q - 1);
    GenExpEnc exp = new GenExpEnc(phi);
    e = exp.value;
    d = comRSA.GetExpDec(phi, e);
    File.WriteAllBytes(e_file, e.ToByteArray()); //файл с ключом e
    File.WriteAllBytes(n_file, n.ToByteArray()); //файл с ключом n
    File.WriteAllBytes(d_file, d.ToByteArray()); //файл с ключом d
}

if (encrypt) // действия, если в строке параметров задано зашифрование
{
    byte[] ae = File.ReadAllBytes(e_file); //читаем файл с ключом e
    e = new BigInteger(ae);
    byte[] an = File.ReadAllBytes(n_file); //читаем файл с ключом n
    n = new BigInteger(an);
    byte[] message = File.ReadAllBytes(message_file); //читаем файл сообщения
    byte[] cipher = comRSA.Encrypt(message, n, e); //зашифровываем сообщение
    File.WriteAllBytes(cipher_file, cipher); //записываем шифр текст
}

if (decrypt) // действия, если в строке параметров задано расшифрование
{
    byte[] ad = File.ReadAllBytes(d_file); //читаем файл с ключом d
    d = new BigInteger(ad);
    byte[] an = File.ReadAllBytes(n_file); //читаем файл с ключом n
    n = new BigInteger(an);
    byte[] cipher = File.ReadAllBytes(cipher_file); //читаем файл шифр текста
    byte[] uncipher = comRSA.Decrypt(cipher, n, d); //расшифровываем шифр текст
    File.WriteAllBytes(uncipher_file, uncipher); //записываем расшифровку
}

if (man) // режим запуска программы без параметров с генерацией ключей заданной длины
{
    GenPrime prime = new GenPrime(512, 32); //p 512 бит k=32
    p = prime.value;
    prime = new GenPrime(504, 32); //q 504 бита k=32
    q = prime.value;
    n = p * q; 
    phi = (p - 1) * (q - 1);
    GenExpEnc exp = new GenExpEnc(phi); //подбираем e
    e = exp.value;
    d = comRSA.GetExpDec(phi, e); //вычисляем d
    File.WriteAllBytes(e_file, e.ToByteArray()); //файл с ключом e
    File.WriteAllBytes(n_file, n.ToByteArray()); //файл с ключом n
    File.WriteAllBytes(d_file, d.ToByteArray()); //файл с ключом d
    byte[] mess = File.ReadAllBytes(message_file); //читаем сообщение из файла
    Console.WriteLine("Original message:");
    for (int i = 0; i < mess.Length; i++) { Console.Write(" " + mess[i].ToString("X")); }
    Console.WriteLine();
    byte[] emess = comRSA.Encrypt(mess, n, e);
    File.WriteAllBytes(cipher_file, emess); //записываем файл шифр текста
    Console.WriteLine("Encrypted message:");
    for (int i = 0; i < emess.Length; i++) { Console.Write(" " + emess[i].ToString("X")); }
    Console.WriteLine();
    byte[] dmess = comRSA.Decrypt(emess, n, d);
    File.WriteAllBytes(uncipher_file, dmess); //записываем файл расшифровки шифр текста
    Console.WriteLine("Decrypted message:");
    for (int i = 0; i < dmess.Length; i++) { Console.Write(" " + dmess[i].ToString("X")); }
    Console.WriteLine();
}

if (test) // режим запуска в тестовом режиме (с ручными параметрами)
{
    p = 113; q = 191; n = p * q; phi = (p-1)*(q-1); e = 13;
    d = comRSA.GetExpDec(phi, e);
    string message = "CRYPTO";
    byte[] mess = Encoding.ASCII.GetBytes(message);
    Console.WriteLine("Original message:");
    for (int i = 0; i < mess.Length; i++) { Console.Write(" " + mess[i].ToString("X")); }
    Console.WriteLine();
    byte[] emess = comRSA.Encrypt(mess, n, e);
    Console.WriteLine("Encrypted message:");
    for (int i = 0; i < emess.Length; i++) { Console.Write(" " + emess[i].ToString("X")); }
    Console.WriteLine();
    byte[] dmess = comRSA.Decrypt(emess, n, d);
    Console.WriteLine("Decrypted message:");
    for (int i = 0; i < dmess.Length; i++) { Console.Write(" " + dmess[i].ToString("X")); }
    Console.WriteLine();
}
