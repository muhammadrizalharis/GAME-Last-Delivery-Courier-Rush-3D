# Last Delivery: Courier Rush 3D

Game 3D bergenre **endless-runner** — seorang **kurir** mengantar paket melewati berbagai kawasan kota sambil menghindari beragam rintangan. Tiap level punya **tema dan cara main yang berbeda**. Dibuat untuk tugas mata kuliah **Teknologi Game**.

> 🎮 Semua isi game (karakter kurir, rintangan, lingkungan, dan efek) dibuat **100% dari kode + bentuk dasar (primitif) Unity** — tanpa asset, model, atau gambar jadi.

## Teknologi
- **Unity 6** (Universal Render Pipeline)
- **C#** dengan **New Input System**
- Grafik dari primitif + material/tekstur yang dihasilkan lewat kode

## Kontrol
| Tombol | Aksi |
|---|---|
| **A / D** (atau ← / →) | Geser lajur kiri / kanan |
| **Spasi / W / ↑** | Lompat |
| **S / ↓ / Ctrl** | Menunduk (lewati palang rendah) |
| **WASD** | Jalan bebas (khusus scene Pos Kurir) |
| **R** | Ulangi level saat kalah |

## Level & Kawasan
Kurir berlari otomatis ke depan. Tiap kawasan punya rintangan khas yang berbeda:

1. **Level 1 — Kota** 🏙️  
   Ambil paket di pos kurir (jalan bebas WASD), lalu lari di jalan raya menghindari **mobil** yang datang dari depan.
2. **Level 2 — Rel Kereta** 🚂 *(malam)*  
   Hindari **kereta** dari depan, **lompati batu**, dan **menunduk** di bawah gerbang. Ada **koin**, **perisai**, serta **bos** di ujung.
3. **Level 3 — Terowongan Tambang** ⛏️ *(gelap)*  
   Waspadai **batu jatuh dari langit-langit** dan **batu menggelinding** dari depan, sambil menunduk di bawah balok. Diterangi lampu helm kurir, dengan **boss makhluk** penunggu terowongan.
4. **Level 4 — Pasar Malam** 🏮  
   **Orang menyeberang** dari samping — lewat hanya saat lajur kosong! Suasana lampion warna-warni & kios pasar, dengan **vendor besar** sebagai bos.
5. **Level 5 — Kawasan Industri** 🏭 *(segera hadir)*  
   Ban berjalan, mesin, dan **bos terakhir** menuju layar **TAMAT**.

## Fitur
- ❤️ **Nyawa** (3 hati) & ⏱️ **timer** di tiap level
- 🪙 **Skor** koin + 🏆 **Rekor** tersimpan (High Score)
- 🛡️ **Perisai** kebal sementara serta item **nyawa** & **waktu**
- ✨ **Efek**: layar berkedip merah + kamera bergetar saat terkena, percikan kilau saat mengambil item
- 🎬 **Transisi fade** antar level & layar **milestone** tiap level selesai

## Struktur Proyek
- `Assets/Scripts/` — kode game: `Player/`, `Enemy/`, `Level/`, `Items/`, `UI/`, `Core/`
- `Assets/Scenes/` — Menu, Level 1–4, serta layar antar-level (Unlocked/Selesai)
- `Assets/Materials/`, `Assets/Textures/` — material & tekstur hasil kode
