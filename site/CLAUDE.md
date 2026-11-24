# Schlupfwerk: site institucional

Contexto do projeto para o Claude Code. Leia antes de qualquer tarefa.

## O que é
Site institucional da **Schlupfwerk**, um estúdio de jogos indie, independente e
brasileiro, que faz jogos **para toda a família** para Android e iOS. O site
apresenta o estúdio, os valores, os jogos e recebe apoio ("cafezinho").

- Dono e único mantenedor: Manuel Bravo (GitHub: @Bravodin)
- Código **público**, na pasta `site/` do repositório `schlupfwerk/schlupfwerk`
  (o mesmo do README de perfil da org). **Não aceita contribuições.**
- Domínio principal: https://schlupfwerk.com (o `schlupfwerk.com.br` redireciona para ele)
- Contato: contact@schlupfwerk.com (o mesmo endereço nos dois idiomas)

## Pronúncia do nome
Schlupfwerk: /ˈʃlʊpfˌvɛʁk/, "SHLOOPF-vairk" (em português: "chlúpf-vérk").
Em alemão, *Schlupf* é tanto o momento em que o pintinho **sai do ovo** quanto a
**derrapagem** de uma roda. Daí o logo de ovo de pixel com uma rachadura.

## Stack
- **.NET 10, Blazor WebAssembly standalone** (site 100% estático)
- Hospedagem: **Azure Static Web Apps**, plano Free
- CI/CD: **Azure Pipelines** (Azure DevOps), não GitHub Actions. Mesmo modelo da
  ADR 0009 do portfólio do Manuel
  (github.com/Bravodin/portifolio_bravo, `docs/architecture/adr/0009-azure-pipelines-and-static-web-apps.md`):
  - `azure-pipelines.yml` na raiz do repositório, ligado ao GitHub pelo app
    Azure Pipelines, com gatilho só para mudanças em `site/**`
  - etapas: restore → build → test → publish → deploy
  - o pipeline publica o site e faz o deploy da pasta pronta com a task
    `AzureStaticWebApp@0` (`skip_app_build: true`, `skip_api_build: true`)
  - token de deploy do SWA numa **variável secreta** do pipeline
    (`SWA_DEPLOYMENT_TOKEN`), nunca no repositório
  - push no `main` vai para produção; PR vai para um ambiente de preview
    nomeado (`pr<número>`). O plano Free permite 3 previews, que não são
    apagados sozinhos
  - organização nova do Azure DevOps não tem job paralelo gratuito até a
    Microsoft aprovar o pedido; até lá, usar um agente self-hosted
- `staticwebapp.config.json` **sem** `navigationFallback` (ele responde 200 para qualquer
  caminho): cada página tem uma rota com `rewrite` para `/index.html`, e o resto cai no
  `responseOverrides` 404, que entrega o `index.html` com status 404 (o Blazor mostra NotFound)
- Sem backend, sem banco de dados, **sem cookies de rastreamento nem analytics**
- Atenção a desempenho: habilitar trimming e compressão, carregar o mínimo
  possível. O site precisa abrir rápido no celular.
- SEO: Blazor WASM entrega uma página quase vazia antes de carregar. Garanta
  `<title>`, `meta description` e Open Graph no `index.html`, e avalie
  pré-renderização estática se o SEO ficar fraco.

## Estrutura e comandos
```
azure-pipelines.yml            (na raiz do repositório) CI/CD do site
site/
  Schlupfwerk.slnx             solução
  global.json                  SDK .NET 10 fixo
  Directory.Build.props        quality gate: warnings viram erro, analyzers, code style
  Directory.Packages.props     versões dos pacotes (central package management)
  src/Schlupfwerk.Web/         o site (Blazor WebAssembly standalone)
    Localization/              Lang, SiteRoutes (caminhos das páginas), Texts + texts.pt.json / texts.en.json
    Layout/                    MainLayout (cabeçalho, seletor PT/EN, rodapé), BareLayout
    Components/                LocalizedComponent (base com T("chave")), Seo, SwanRaceCard
    Pages/                     Home, About, Games, Cafe, Privacy, NotFound, LanguageRedirect ("/" e "/cafe")
    wwwroot/                   index.html, css/app.css, js/site.js, staticwebapp.config.json, logos, fonte
  tests/Schlupfwerk.Web.Tests/ xUnit v3 + bUnit
  tools/csp-hashes.cs          coloca os hashes dos scripts inline do index.html na CSP, depois do publish
  schlupfwerk-logo/            pacote original do logo (fonte; o site usa cópias em wwwroot)
```

Rodar e verificar (dentro de `site/`):
```
dotnet run --project src/Schlupfwerk.Web          # servidor de desenvolvimento
dotnet build Schlupfwerk.slnx -warnaserror
dotnet format Schlupfwerk.slnx --verify-no-changes
dotnet test --solution Schlupfwerk.slnx
dotnet publish src/Schlupfwerk.Web -c Release -o artifacts/publish
dotnet run tools/csp-hashes.cs -- artifacts/publish/wwwroot
```

Detalhes que já deram problema:
- Os `texts.*.json` são EmbeddedResource com `WithCulture="false"`; sem isso o `.pt`/`.en` do
  nome vira assembly satélite e os textos somem.
- Não use `TrimMode=full`: ele remove os componentes e rotas do próprio site.
- A CSP não permite script inline. O import map que o .NET escreve no `index.html` só funciona
  porque `tools/csp-hashes.cs` coloca o hash dele no `staticwebapp.config.json` (com as quebras de
  linha normalizadas para LF, como o navegador faz). Se aparecer outro script inline, ele precisa
  passar pelo mesmo caminho.
- Fonte Nunito self-hosted em `wwwroot/fonts/v1/` (licença OFL ao lado). Arquivo novo de fonte vai
  para `fonts/v2/`, porque `fonts/v1/*` tem cache imutável.
- `wwwroot/img/pix-qr.png` é cópia de `profile/pix-qr.png`; um teste garante que são iguais.
- `wwwroot/sitemap.xml` lista todas as páginas nos dois idiomas; um teste compara com `SiteRoutes`.
- Página nova: adicione em `SitePage` e `SiteRoutes`, crie o `.razor` com os dois `@page`, os
  textos nos dois JSON, a entrada no `sitemap.xml` e as duas rotas no `staticwebapp.config.json`
  (sem elas a página responde 404). Os testes apontam o que faltar.

## Idiomas
**Português (pt-BR) e inglês (en)**, com um seletor PT/EN no cabeçalho.
- Rotas: `/` (detecta o idioma do navegador), `/pt/...` e `/en/...`, com o caminho traduzido
  (`/pt/sobre` ↔ `/en/about`, `/pt/jogos` ↔ `/en/games`, `/pt/privacidade` ↔ `/en/privacy`;
  o café é `/pt/cafe` e `/en/cafe`, e o link curto `/cafe` leva ao idioma do visitante)
- Textos em `Localization/texts.pt.json` e `texts.en.json`, nunca fixos nos componentes
- Guardar a escolha de idioma em `localStorage`, com fallback se não estiver
  disponível

## Páginas (primeira versão)
1. **Início**: o logo, a frase "Independent games, hatched with care. 🥚"
   (em PT: "Jogos independentes, chocados com carinho."), os destaques e o jogo atual.
2. **Sobre / Manifesto**: quem somos e os valores:
   - para toda a família
   - sem anúncios e sem rastreamento (não diga "sem coleta de dados": os recursos
     online usam dados, que ficam com a Apple e o Google, não com o estúdio)
   - jogo justo: só compras cosméticas, nada que dê vantagem
   - offline primeiro, com recursos online opcionais
   - código aberto (GPLv3) e aberto a colaboração no GitHub
   - jogos leves, que rodam bem em celulares comuns
3. **Jogos**: por enquanto, o **Swan Race** (nome provisório), com o status "🥚 ainda no ovo".
   Corrida 3D em que um cisne desce montanhas esquiando, corre, nada (decolando ao
   correr sobre a água) e voa. Pistas: Suíça, Alemanha e Rio de Janeiro.
   Bots com IA, funciona offline, skins cosméticas. Link: github.com/schlupfwerk/swanrace
   Plano da 1.0, sem servidor do estúdio: single player contra a IA; skins e moeda do jogo
   (ganha jogando ou comprada pela loja), com o progresso salvo no iCloud / Play Games;
   placar entre amigos pelo Game Center / Play Games; convite por link (WhatsApp).
   Multiplayer e indicação com recompensa ficam para a 2.0. Não ler a agenda de contatos.
4. **Café** (`/cafe`): botão do Ko-fi (https://ko-fi.com/schlupfwerk) para o público
   internacional e **QR Code do Pix** para brasileiros.
   - Use a imagem pronta `profile/pix-qr.png` (na raiz do repositório), a mesma
     do README. Não gere outro QR Code sem o Manuel pedir.
   - Chave aleatória `d5555f67-5b99-4471-a9a0-3af5881e61f9`, sem valor fixo
     (quem doa escolhe quanto). Payload (BR Code estático):
     `00020126580014br.gov.bcb.pix0136d5555f67-5b99-4471-a9a0-3af5881e61f95204000053039865802BR5924BRAVO CANDIA CONSULTORIA6006BRASIL62070503***6304028B`
     Se a chave mudar de novo, gere o QR só trocando a chave (nome, cidade e txid `***`
     iguais, CRC16-CCITT recalculado) e atualize as duas cópias do PNG.
   - Abaixo do QR Code, mostrar: "Beneficiário: MANUEL ANDREZ BRAVO CANDIA
     CONSULTORIA EM TI LTDA".
   - Sem código "copia e cola": só o QR Code, para ficar limpo.
   - O CNPJ **não** está cadastrado como chave Pix; não use.
5. **Privacidade**: política simples e honesta, exigida pelas lojas:
   - o site não usa cookies de rastreamento nem analytics
   - os jogos não enviam dados pessoais ao estúdio, nem exigem conta
   - placares (inclusive entre amigos), conquistas e progresso na nuvem são opcionais
     e usam o Game Center e o iCloud (iOS) ou o Google Play Games (Android), que guardam
     apelido, amigos, pontuação e progresso sob as políticas da Apple e do Google
   - se um recurso novo precisar de outros dados (ex.: multiplayer na 2.0), a política
     diz quais antes de ele existir
   - as compras são processadas pela App Store e pela Google Play
   - as doações são processadas pelo Ko-fi/Stripe ou pelo Pix
   - contato: contact@schlupfwerk.com
   - mencionar a LGPD e um cuidado especial com crianças

Rodapé: links para GitHub (github.com/schlupfwerk), Ko-fi e e-mail, mais
"© Schlupfwerk".

## Identidade visual
Estilo **claro, neutro, acolhedor e para toda a família**. Limpo, com muito
espaço em branco e toques de pixel art inspirados no logo. Nada escuro ou
agressivo.

Cores (do logo):
| Nome | Hex | Uso |
|---|---|---|
| Marinho | `#13233A` | texto principal, títulos, logo |
| Marinho escuro | `#0A1626` | rodapé, contraste forte |
| Casca | `#F4F1EA` | fundos de seção, cartões |
| Branco quente | `#FFFDF8` | fundo da página |
| Gema | `#FFB020` | **só destaques decorativos** (rachadura, ícones, sublinhados). Não use como cor de texto sobre fundo claro: o contraste é baixo. |

- Contraste de texto de pelo menos 4.5:1 (WCAG AA)
- Tipografia: uma sans amigável e legível para o texto (ex.: Nunito ou Inter);
  títulos podem ter um toque pixelado só em detalhes, nunca no texto corrido
- Detalhes pixel: divisores em zigue-zague como a rachadura do ovo, cantos
  levemente arredondados, ícones simples
- Mobile first, com alvos de toque de pelo menos 44px
- Sem gradientes chamativos e sem fotos de pessoas

## Logos
Os arquivos do logo estão no pacote `schlupfwerk-logo` (pastas `site/`,
`logo-svg/` e `logo-png/`). Use:
- `site/favicon.ico`, `site/favicon.svg`, `site/apple-touch-icon.png`,
  `site/icon-192.png`, `site/icon-512.png`
- `site/logo-header.svg` (fundo claro) e `site/logo-header-dark.svg` (fundo escuro)
- `site/og-image.png` (1200×630) para Open Graph

## Regras
- Não invente números, depoimentos, datas de lançamento ou parceiros. O que
  não se sabe ainda fica como `[A DEFINIR]`.
- Nada de analytics, pixels de rastreamento ou scripts de terceiros além do
  necessário.
- Comentários de código em inglês; textos do site em PT e EN.
