# Schlupfwerk: pacote de logo (ovo de pixel)

## Cores
| Nome | Hex | Uso |
|---|---|---|
| Marinho | `#13233A` | cor principal, fundos de ícone |
| Marinho escuro | `#0A1626` | fundos mais profundos |
| Casca | `#F4F1EA` | fundo claro e ovo em fundo escuro |
| Gema | `#FFB020` | rachadura (destaque) |

## Pastas
- **logo-svg/**: logos vetoriais (use sempre que possível)
  - `simbolo` = só o ovo · `horizontal` = ovo + nome lado a lado · `empilhado` = ovo sobre o nome
  - `color` = para fundo claro · `white` = para fundo escuro · `mono-black` / `mono-white` = uma cor só (carimbo, marca d'água)
- **logo-png/**: os mesmos logos em PNG transparente de alta resolução
- **site/**
  - `favicon.ico`, `favicon.svg`, `favicon-32.png`, `apple-touch-icon.png` (180), `icon-192.png`, `icon-512.png`
  - `logo-header.svg` / `logo-header-dark.svg`: logo do cabeçalho
  - `og-image.png` (1200×630): imagem que aparece ao compartilhar o link
- **github/**: `avatar-500.png`, foto da organização
- **lojas/**
  - `app-store/app-store-icon-1024.png`: 1024×1024, sem transparência e sem cantos arredondados (a Apple arredonda)
  - `google-play/play-icon-512.png` e `play-feature-graphic-1024x500.png`
  - `android-adaptive/`: foreground, background e monochrome (432 px) para o ícone adaptativo do Android (a Godot pede esses três na exportação)
- **redes/**: `avatar-1080.png` para Instagram e TikTok

## HTML do favicon
```html
<link rel="icon" href="/favicon.ico" sizes="any">
<link rel="icon" href="/favicon.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="/apple-touch-icon.png">
<meta property="og:image" content="https://schlupfwerk.com/og-image.png">
```

## Licença
O nome e o logo Schlupfwerk não fazem parte da licença do código (GPLv3).
