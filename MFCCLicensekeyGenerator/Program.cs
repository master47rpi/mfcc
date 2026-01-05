using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MFCCLicensekeyGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MFCC Lizenz-System (Sicherer Modus) ===");

            // Pfad außerhalb des Projektordners festlegen (AppData/Roaming/MFCC_Keys)
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string keyFolder = Path.Combine(appData, "MFCC_Keys");
            string keyFilePath = Path.Combine(keyFolder, "mfcc_private_keys.xml");

            // Ordner erstellen, falls er nicht existiert
            if (!Directory.Exists(keyFolder))
            {
                Directory.CreateDirectory(keyFolder);
            }

            // 1. Prüfen, ob bereits Schlüssel existieren
            if (!File.Exists(keyFilePath))
            {
                GenerateAndSaveKeys(keyFilePath);
            }
            else
            {
                Console.WriteLine($"Schlüssel geladen aus: {keyFilePath}");
            }

            // 2. Schlüssel laden
            string privateKeyXml = File.ReadAllText(keyFilePath);

            // 3. Lizenz-Schleife
            while (true)
            {
                Console.WriteLine("\n--- Neue Lizenz erstellen ---");
                Console.Write("Name des Nutzers (oder 'exit'): ");
                string name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name) || name.ToLower() == "exit") break;

                string licenseKey = CreateLicense(name, privateKeyXml);

                Console.WriteLine("\nERGEBNIS:");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"Name:    {name}");
                Console.WriteLine($"Lizenz:  {licenseKey}");
                Console.WriteLine("--------------------------------------------------");
            }
        }

        static void GenerateAndSaveKeys(string filePath)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                File.WriteAllText(filePath, rsa.ToXmlString(true));

                Console.WriteLine("!!! NEUE SCHLÜSSEL GENERIERT !!!");
                Console.WriteLine($"Speicherort: {filePath}");
                Console.WriteLine("\nKopiere diesen PUBLIC KEY jetzt in deine HAUPT-APP:");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(rsa.ToXmlString(false));
                Console.WriteLine("--------------------------------------------------\n");
                Console.WriteLine("Drücke Enter zum Fortfahren...");
                Console.ReadLine();
            }
        }

        static string CreateLicense(string name, string privateKeyXml)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(privateKeyXml);
                byte[] nameBytes = Encoding.UTF8.GetBytes(name);
                using (var sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(nameBytes);
                    byte[] signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    return Convert.ToBase64String(signature);
                }
            }
        }
    }
}