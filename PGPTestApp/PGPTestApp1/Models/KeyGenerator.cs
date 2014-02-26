// -----------------------------------------------------------------------
// <copyright file="KeyGenerator.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace PGPTestAppConsole.KeyModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using System.IO;
    using Org.BouncyCastle.Bcpg;
    using Org.BouncyCastle.Bcpg.OpenPgp;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class KeyGenerator
    {
        public void GenerateKey(string username, string password, string keyStoreUrl)
        {
            IAsymmetricCipherKeyPairGenerator kpg = new RsaKeyPairGenerator();
            kpg.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(0x13), new SecureRandom(), 1024, 8));
            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            FileStream out1 = new FileInfo(string.Format("{0}secret.txt", keyStoreUrl)).OpenWrite();
            FileStream out2 = new FileInfo(string.Format("{0}pub.txt", keyStoreUrl)).OpenWrite();

            ExportKeyPair(out1, out2, kp.Public, kp.Private, username, password.ToCharArray(), true);
            out1.Close();
            out2.Close();
        }

        private static void ExportKeyPair(
                    Stream secretOut,
                    Stream publicOut,
                    AsymmetricKeyParameter publicKey,
                    AsymmetricKeyParameter privateKey,
                    string identity,
                    char[] passPhrase,
                    bool armor)
        {
            if (armor)
            {
                secretOut = new ArmoredOutputStream(secretOut);
            }

            PgpSecretKey secretKey = new PgpSecretKey(
                PgpSignature.DefaultCertification,
                PublicKeyAlgorithmTag.RsaGeneral,
                publicKey,
                privateKey,
                DateTime.Now,
                identity,
                SymmetricKeyAlgorithmTag.Cast5,
                passPhrase,
                null,
                null,
                new SecureRandom()
                //                ,"BC"
                );

            secretKey.Encode(secretOut);

            secretOut.Close();

            if (armor)
            {
                publicOut = new ArmoredOutputStream(publicOut);
            }

            PgpPublicKey key = secretKey.PublicKey;

            key.Encode(publicOut);

            publicOut.Close();
        }
    }
}
