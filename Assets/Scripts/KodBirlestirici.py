import os

# --- AYARLAR ---
# Taranacak kök dizin ('.' şu anki klasör demektir)
ROOT_DIR = '.' 

# Oluşturulacak çıktı dosyası
OUTPUT_FILE = 'TumProjeKodlari.txt' 

# Dahil edilecek uzantılar
EXTENSIONS = ['.cs']

# Kesinlikle taranmayacak klasörler (Unity sistem dosyaları ve Asset Store eklentileri)
# Eğer LeanTouch kodlarını da atmak istersen 'Lean' kısmını bu listeden çıkar.
EXCLUDE_DIRS = {
    'Library', 'Temp', 'obj', 'Build', '.git', '.vs', 'Packages', 
    'Lean', 'LeanTouch', 'Plugins' 
}

def merge_scripts():
    print("Kod taraması başlıyor...")
    
    with open(OUTPUT_FILE, 'w', encoding='utf-8') as outfile:
        file_count = 0
        
        for dirpath, dirnames, filenames in os.walk(ROOT_DIR):
            # İstenmeyen klasörleri tarama listesinden çıkar (yerinde değişiklik yapar)
            dirnames[:] = [d for d in dirnames if d not in EXCLUDE_DIRS]
            
            for filename in filenames:
                # Sadece .cs dosyalarını al
                if any(filename.endswith(ext) for ext in EXTENSIONS):
                    filepath = os.path.join(dirpath, filename)
                    
                    # Çıktı dosyasının kendisini tekrar okumaya çalışma
                    if filename == OUTPUT_FILE:
                        continue

                    # Dosya yolunu oluştur (Örn: Assets/Scripts/GameManager.cs)
                    rel_path = os.path.relpath(filepath, ROOT_DIR)
                    
                    try:
                        with open(filepath, 'r', encoding='utf-8') as infile:
                            content = infile.read()
                            
                            # Ayrıştırıcı Başlık Ekle
                            outfile.write(f"\n{'='*50}\n")
                            outfile.write(f"DOSYA YOLU: {rel_path}\n")
                            outfile.write(f"{'='*50}\n\n")
                            
                            # Kodu Yaz
                            outfile.write(content)
                            outfile.write("\n")
                            
                            print(f"Eklendi: {rel_path}")
                            file_count += 1
                            
                    except Exception as e:
                        print(f"HATA - Okunamadı: {rel_path} -> {e}")

    print(f"\nİşlem Tamamlandı! Toplam {file_count} dosya birleştirildi.")
    print(f"Çıktı dosyası: {os.path.abspath(OUTPUT_FILE)}")

if __name__ == "__main__":
    merge_scripts()