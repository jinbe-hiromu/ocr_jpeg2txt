using Tesseract;

namespace OcrFolderToTxt
{
    class Program
    {
        static int Main(string[] args)
        {
            // 引数: [0] 画像フォルダ [1] 言語指定)
            if (args.Length < 2)
            {
                Console.WriteLine("使い方: OcrFolderToTxt <画像フォルダ> <言語>");
                Console.WriteLine(@"例:    OcrFolderToTxt ""C:\Images"" ""jpn+eng""");
                return 1;
            }

            var imageDir = args[0];
            var lang = args[1];
            var tessdataPath = @".\";

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
                var allText = ""; // まとめて書き出す用

                foreach (var imgPath in imageFiles)
                {
                    try
                    {
                        using var pix = Pix.LoadFromFile(imgPath);
                        using var page = engine.Process(pix);

                        var text = page.GetText() ?? string.Empty;

                        // ファイル名で区切る
                        allText += $"----- {Path.GetFileName(imgPath)} -----{Environment.NewLine}";
                        allText += text + Environment.NewLine;

                        Console.WriteLine($"[OK] {Path.GetFileName(imgPath)}");
                        success++;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"[NG] {Path.GetFileName(imgPath)}: {ex.Message}");
                        fail++;
                    }
                }

                // まとめて1ファイルに出力
                var outputPath = Path.Combine(imageDir, "all_ocr_result.txt");
                File.WriteAllText(outputPath, allText);

                Console.WriteLine($"[DONE] 成功: {success}, 失敗: {fail}");
                Console.WriteLine($"[INFO] 出力ファイル: {outputPath}");
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
    }
}
