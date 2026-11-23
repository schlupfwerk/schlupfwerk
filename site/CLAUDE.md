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
- Contato: contato@schlupfwerk.com (também contact@)

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
- `staticwebapp.config.json` com `navigationFallback` para `index.html`
  (exclua `/_framework/*`, `/css/*`, `/img/*` e arquivos com extensão)
- Sem backend, sem banco de dados, **sem cookies de rastreamento nem analytics**
- Atenção a desempenho: habilitar trimming e compressão, carregar o mínimo
  possível. O site precisa abrir rápido no celular.
- SEO: Blazor WASM entrega uma página quase vazia antes de carregar. Garanta
  `<title>`, `meta description` e Open Graph no `index.html`, e avalie
  pré-renderização estática se o SEO ficar fraco.

## Idiomas
**Português (pt-BR) e inglês (en)**, com um seletor PT/EN no cabeçalho.
- Rotas sugeridas: `/` (detecta o idioma do navegador), `/pt/...` e `/en/...`
- Textos em arquivos de recurso (`.resx` ou JSON), nunca fixos nos componentes
- Guardar a escolha de idioma em `localStorage`, com fallback se não estiver
  disponível

## Páginas (primeira versão)
1. **Início**: o logo, a frase "Independent games, hatched with care. 🥚"
   (em PT: "Jogos independentes, chocados com carinho."), os destaques e o jogo atual.
2. **Sobre / Manifesto**: quem somos e os valores:
   - para toda a família
   - sem anúncios e sem coleta de dados
   - jogo justo: só compras cosméticas, nada que dê vantagem
   - offline primeiro, com recursos online opcionais
   - código aberto (GPLv3) e aberto a colaboração no GitHub
   - jogos leves, que rodam bem em celulares comuns
3. **Jogos**: por enquanto, o **Swan Race** (nome provisório), com o status "🥚 ainda no ovo".
   Corrida 3D em que um cisne desce montanhas esquiando, corre, nada (decolando ao
   correr sobre a água) e voa. Pistas: Suíça, Alemanha e Rio de Janeiro.
   Bots com IA, funciona offline, skins cosméticas. Link: github.com/schlupfwerk/swanrace
4. **Café** (`/cafe`): botão do Ko-fi (https://ko-fi.com/schlupfwerk) para o público
   internacional e **QR Code do Pix** para brasileiros.
   - Use a imagem pronta `profile/pix-qr.png` (na raiz do repositório), a mesma
     do README, já testada com pagamento real. Não gere outro QR Code.
   - Chave aleatória `ad349d7a-7922-49e9-9e87-9dfa4535a375`, sem valor fixo
     (quem doa escolhe quanto).
   - Abaixo do QR Code, mostrar: "Beneficiário: MANUEL ANDREZ BRAVO CANDIA
     CONSULTORIA EM TI LTDA".
   - Sem código "copia e cola": só o QR Code, para ficar limpo.
   - O CNPJ **não** está cadastrado como chave Pix; não use.
5. **Privacidade**: política simples e honesta, exigida pelas lojas:
   - o site não usa cookies de rastreamento nem analytics
   - os jogos não coletam dados pessoais
   - placares e conquistas opcionais usam o Game Center (iOS) e o Google Play
     Games (Android), que guardam apelido e pontuação sob as políticas da
     Apple e do Google
   - as compras são processadas pela App Store e pela Google Play
   - as doações são processadas pelo Ko-fi/Stripe ou pelo Pix
   - contato: contato@schlupfwerk.com
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
