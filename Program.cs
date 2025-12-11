
using System;
using System.IO;
using System.Linq;
using Tesseract;

namespace OcrFolderToTxt
{
    class Program
    {
        static int Main(string[] args)
        {
            // 引数: [0] 対象フォルダ, [1] tessdata のパス, [2] 言語 (例: "jpn+eng")
            if (args.Length < 3)
            {
                Console.WriteLine("使い方: OcrFolderToTxt <画像フォルダ> <tessdataフォルダ> <言語>");
                Console.WriteLine(@"例:    OcrFolderToTxt ""C:\Images"" ""C:\tessdata"" ""jpn+eng""");
                return 1;
            }

            var imageDir = args[0];
            var tessdataPath = args[1];
            var lang = args[2];

            if (!Directory.Exists(imageDir))
            {
                Console.Error.WriteLine($"[ERROR] 画像フォルダが存在しません: {imageDir}");
                return 2;
            }
            if (!Directory.Exists(tessdataPath))
            {
                Console.Error.WriteLine($"[ERROR] tessdataフォルダが存在しません: {tessdataPath}");
                return 3;
            }

            var imageFiles = Directory.EnumerateFiles(imageDir, "*.*", SearchOption.TopDirectoryOnly)
                                      .Where(p => HasJpegExtension(p))
                                      .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                                      .ToList();

            if (imageFiles.Count == 0)
            {
                Console.WriteLine("[INFO] 対象フォルダに .jpg/.jpeg がありません。");
                return 0;
            }

            Console.WriteLine($"[INFO] 対象: {imageDir}");
            Console.WriteLine($"[INFO] 件数: {imageFiles.Count}");
            Console.WriteLine($"[INFO] 言語: {lang}");
            Console.WriteLine($"[INFO] tessdata: {tessdataPath}");

            try
            {
                // Tesseract OCR エンジン初期化
                using var engine = new TesseractEngine(tessdataPath, lang, EngineMode.Default);

                int success = 0, fail = 0;
                foreach (var imgPath in imageFiles)
                {
                    var txtPath = Path.Combine(Path.GetDirectoryName(imgPath)!, "txt",
                                               Path.GetFileNameWithoutExtension(imgPath) + ".txt");

                    try
                    {
                        using var pix = Pix.LoadFromFile(imgPath);
                        using var page = engine.Process(pix);

                        var text = page.GetText() ?? string.Empty;

                        // 改行や空白の整形（必要に応じて）
                        // text = NormalizeText(text);

                        File.WriteAllText(txtPath, text);
                        Console.WriteLine($"[OK] {Path.GetFileName(imgPath)} -> {Path.GetFileName(txtPath)}");
                        success++;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"[NG] {Path.GetFileName(imgPath)}: {ex.Message}");
                        fail++;
                    }
                }

                Console.WriteLine($"[DONE] 成功: {success}, 失敗: {fail}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[FATAL] OCR 初期化/処理に失敗: {ex.Message}");
                return 9;
            }

            return 0;
        }

        static bool HasJpegExtension(string path)
        {
            var ext = Path.GetExtension(path)?.ToLowerInvariant();
            return ext == ".jpg" || ext == ".jpeg";
        }

        // 必要なら出力整形を行う
        // static string NormalizeText(string input)
        // {
        //     // 連続空白の削除、CRLF統一など
        //     return input.Replace("\r\n", "\n").Trim();
        // }
    }
}
