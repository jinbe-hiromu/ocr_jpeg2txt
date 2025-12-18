# ocr_jpeg2txt

## 概要
`ocr_jpeg2txt` は、指定したフォルダ内の JPEG 画像（.jpg/.jpeg）を一括で OCR 処理し、全画像の認識結果を1つのテキストファイル（`all_ocr_result.txt`）にまとめて出力する .NET 8 コンソールアプリケーションです。Tesseract OCR エンジンを利用しています。

## 特徴
- フォルダ内の全 JPEG 画像を自動で検出
- 画像ごとに OCR を実行し、全画像のテキストを1つのファイルにまとめて保存
- 日本語・英語など複数言語に対応（Tesseractの言語データを指定可能）
- エラーや進捗をコンソールに出力

## 必要条件
- .NET 8 SDK
- [Tesseract OCR 言語データ（tessdata）](https://github.com/tesseract-ocr/tessdata)
- Tesseract 公式 NuGet パッケージ（ユーザーが各自で取得）

## 使い方

1. 必要な NuGet パッケージ（例: `Tesseract`）をインストールしてください。
2. [tessdata](https://github.com/tesseract-ocr/tessdata) から必要な言語データをダウンロードし、プログラム直下に配置してください。
3. コマンドラインから以下のように実行します。

### 実行例
```
ocr_jpeg2txt.exe "<画像フォルダ>" "<言語>"

- `<画像フォルダ>` : OCR 対象の画像フォルダ
- `<言語>` : 使用する言語（例: `jpn+eng`）
```

### 出力
- 各画像と同じフォルダ内に`all_ocr_result.txt` ファイルが出力されます。

## ライセンス
本プロジェクトは MIT License です。
本ソフトウェアは以下の外部コンポーネントを利用しています：

- [Tesseract OCR](https://github.com/tesseract-ocr/tesseract)（Apache License 2.0）
- [tessdata](https://github.com/tesseract-ocr/tessdata)（Apache License 2.0）

## 注意事項
- 画像ファイルは `.jpg` または `.jpeg` のみ対応しています。
- 大量の画像を処理する場合、処理時間がかかることがあります。
