using System;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("=== BAI 2 - BUOC 6: CHU KY SO RSA ===\n");

// 1. Import cap khoa
string noiDungPublicPem = File.ReadAllText("public_key.pem");
string noiDungPrivatePem = File.ReadAllText("private_key.pem");

using (RSA rsaPublic = RSA.Create())
using (RSA rsaPrivate = RSA.Create())
{
    rsaPublic.ImportFromPem(noiDungPublicPem);
    rsaPrivate.ImportFromPem(noiDungPrivatePem);
    Console.WriteLine("Da import cap khoa thanh cong!\n");

    // 2. Du lieu can ky
    string duLieuCanKy = "Hop dong mua ban giua Cong ty A va Cong ty B, ngay 25/10/2026";
    byte[] duLieuBytes = Encoding.UTF8.GetBytes(duLieuCanKy);
    Console.WriteLine($"Du lieu can ky: \"{duLieuCanKy}\"");
    Console.WriteLine($"Kich thuoc: {duLieuBytes.Length} bytes\n");

    // 3. Tao bam SHA256 cua du lieu
    byte[] bamSHA256 = SHA256.HashData(duLieuBytes);
    Console.WriteLine($"Bam SHA256 cua du lieu: {BitConverter.ToString(bamSHA256).Replace("-", "").ToLower()}\n");

    // ============================================
    // PHUONG PHAP 1: PKCS1v1.5 (CHU KY GIONG NHAU)
    // ============================================
    Console.WriteLine("=== PHUONG PHAP 1: RSA PKCS1v1.5 (Khong co salt ngau nhien) ===\n");
    
    string[] chuKyPKCS1 = new string[3];
    for (int i = 0; i < 3; i++)
    {
        byte[] chuKyBytes = rsaPrivate.SignData(duLieuBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        chuKyPKCS1[i] = Convert.ToBase64String(chuKyBytes);
        Console.WriteLine($"Lan ky {i + 1}: {chuKyPKCS1[i].Substring(0, 50)}...");
    }

    Console.WriteLine("\nNhan xet: Cac chu ky PKCS1v1.5 co GIONG nhau khong?");
    bool giongNhauPKCS1 = (chuKyPKCS1[0] == chuKyPKCS1[1]) && (chuKyPKCS1[1] == chuKyPKCS1[2]);
    Console.WriteLine(giongNhauPKCS1 ? "✅ CO - Cac chu ky GIONG NHAU (khong an toan lam!)" : " KHONG - Cac chu ky KHAC NHAU");

    // ============================================
    // PHUONG PHAP 2: RSA-PSS (CHU KY KHAC NHAU - DE XUAT)
    // ============================================
    Console.WriteLine("\n=== PHUONG PHAP 2: RSA-PSS (Co salt ngau nhien) - DE XUAT ===\n");
    
    string[] chuKyPSS = new string[3];
    for (int i = 0; i < 3; i++)
    {
        byte[] chuKyBytes = rsaPrivate.SignData(duLieuBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        chuKyPSS[i] = Convert.ToBase64String(chuKyBytes);
        Console.WriteLine($"Lan ky {i + 1}: {chuKyPSS[i].Substring(0, 50)}...");
    }

    Console.WriteLine("\nNhan xet: Cac chu ky PSS co GIONG nhau khong?");
    bool giongNhauPSS = (chuKyPSS[0] == chuKyPSS[1]) && (chuKyPSS[1] == chuKyPSS[2]);
    Console.WriteLine(giongNhauPSS ? "✅ CO - Cac chu ky GIONG NHAU" : "❌ KHONG - Cac chu ky KHAC NHAU (DUNG NHU YEU CAU!)");

    // ============================================
    // 4. VERIFY CHU KY PSS BANG PUBLIC KEY
    // ============================================
    Console.WriteLine("\n=== VERIFY CHU KY PSS BANG PUBLIC KEY ===\n");
    
    for (int i = 0; i < 3; i++)
    {
        byte[] chuKyBytes = Convert.FromBase64String(chuKyPSS[i]);
        bool hopLe = rsaPublic.VerifyData(duLieuBytes, chuKyBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        Console.WriteLine($"Chu ky PSS lan {i + 1}: {(hopLe ? "✅ HOP LE" : " KHONG HOP LE")}");
    }

    // ============================================
    // 5. THU VERIFY VOI DU LIEU SAI
    // ============================================
    Console.WriteLine("\n=== THU VERIFY VOI DU LIEU SAI ===\n");
    
    string duLieuSai = "Hop dong mua ban giua Cong ty A va Cong ty B, ngay 26/10/2026"; // Sai 1 ngay
    byte[] duLieuSaiBytes = Encoding.UTF8.GetBytes(duLieuSai);
    byte[] chuKyBytes0 = Convert.FromBase64String(chuKyPSS[0]);
    bool hopLeSai = rsaPublic.VerifyData(duLieuSaiBytes, chuKyBytes0, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
    Console.WriteLine($"Du lieu sai: \"{duLieuSai}\"");
    Console.WriteLine($"Verify chu ky lan 1: {(hopLeSai ? "✅ HOP LE" : " KHONG HOP LE (DUNG!)")}");

    // ============================================
    // 6. KET LUAN VA DE XUAT
    // ============================================
    Console.WriteLine("\n=== KET LUAN VA DE XUAT PHUONG PHAP ===\n");
    Console.WriteLine("YEUC AU DAC BIET CUA THAY:");
    Console.WriteLine("- Cung 1 du lieu + Cung 1 Private Key => Chu ky o cac lan ky KHAC NHAU");
    Console.WriteLine("\nPHUONG PHAP DE XUAT: Su dung RSA-PSS (Probabilistic Signature Scheme)");
    Console.WriteLine("\nTAI SAO PSS DAT DUOC DIEU NAY?");
    Console.WriteLine("1. PSS them 'salt ngau nhien' (random salt) vao qua trinh ky");
    Console.WriteLine("2. Moi lan ky, salt khac nhau => chu ky khac nhau");
    Console.WriteLine("3. Nhung van verify duoc bang Public Key (vi salt duoc ma hoa cung chu ky)");
    Console.WriteLine("\nSO SANH PKCS1v1.5 VA PSS:");
    Console.WriteLine("- PKCS1v1.5: Deterministic => Cung du lieu + Cung khoa = Cung chu ky (KHONG an toan)");
    Console.WriteLine("- PSS: Probabilistic => Cung du lieu + Cung khoa = Chu ky KHAC NHAU (AN TOAN HON)");
    Console.WriteLine("\nUNG DUNG THUC TE:");
    Console.WriteLine("- Chu ky so trong chung thu so (digital certificate)");
    Console.WriteLine("- Ky hop dong dien tu, van ban phap ly");
    Console.WriteLine("- Bao dam tinh toan ven, chong gia mau");
}

Console.WriteLine("\nNhan phim bat ky de thoat...");
Console.ReadKey();