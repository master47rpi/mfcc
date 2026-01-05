using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace mirada_finanza_control_central
{
    public static class LicenseValidator
    {
        // FÜGE HIER DEINEN KOMPLETTEN PUBLIC KEY AUS DEM GENERATOR EIN (mit <RSAKeyValue>...)
        private const string PublicKeyXml = @"<RSAKeyValue><Modulus>2ELEqH8ED2FKVuq3nk4GxkgszYDWDgbFzejsWeGa8gK3dQyeRA2sRVK1Rz+lZFpgZ5wIJYLEpdy6Fwt8TzY12+tCHwygTSkGf9RdI083lKxDUEp3U3cYLX4fzGWRurtT1E9XcX3NOeb34J2Yah7AayQmcnfs96GQraf/sddUGur7F2LlcmGj6ZnOiWVsLX9FF7BtqUsqh/+4w8WsKiQObXwEEcQipKPhCdJCsUeKljKvxe3ZJzwnFomYOHybpOlHA198ObkcPGrz5teNcLawkJOaZ6vDsid3AkCgZ8Wjd4KUfbcJD+CI1h3UxCmCp/O3LkecpVGqnwWvX1bD4l4SZQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static bool IsLicenseValid(string userName, string signatureBase64)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(signatureBase64))
                return false;

            try
            {
                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    // Lädt das "Vorhängeschloss" (Public Key)
                    rsa.FromXmlString(PublicKeyXml);

                    // Bereitet die Prüfung vor
                    var rsaFormatter = new RSAPKCS1SignatureDeformatter(rsa);
                    rsaFormatter.SetHashAlgorithm("SHA256");

                    // Den Namen in Bytes umwandeln und hashen (muss exakt wie im Generator sein)
                    byte[] nameBytes = Encoding.UTF8.GetBytes(userName);
                    using (var sha256 = SHA256.Create())
                    {
                        byte[] hash = sha256.ComputeHash(nameBytes);

                        // Den Lizenzschlüssel von Base64 zurück in Bytes wandeln
                        byte[] signature = Convert.FromBase64String(signatureBase64);

                        // Die mathematische Prüfung: Passt die Signatur zum Namen?
                        return rsaFormatter.VerifySignature(hash, signature);
                    }
                }
            }
            catch
            {
                // Bei Fehlern (z.B. manipulierter Key) wird der Zugriff verweigert
                return false;
            }
        }
    }
}
