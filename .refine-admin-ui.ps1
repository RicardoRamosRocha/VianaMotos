$ErrorActionPreference = 'Stop'
$utf8 = [System.Text.UTF8Encoding]::new($false)
$projectRoot = Join-Path $PSScriptRoot 'VianaMotos.Web'
$viewsRoot = Join-Path $projectRoot 'Areas/Admin/Views'

function Read-Utf8([string]$path) {
    [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)
}

function Write-Utf8([string]$path, [string]$content) {
    [System.IO.File]::WriteAllText($path, $content, $utf8)
}

function Replace-Exact([string]$content, [string]$oldValue, [string]$newValue, [string]$description) {
    if (-not $content.Contains($oldValue)) { throw "Trecho não encontrado: $description" }
    $content.Replace($oldValue, $newValue)
}

# Componente compartilhado: o ícone passa a ocupar um container visual fixo.
$partialPath = Join-Path $viewsRoot 'Shared/_PageHeader.cshtml'
$partial = Read-Utf8 $partialPath
$oldPartialContent = @'
    <div class="vm-page-header__content">
        <span class="vm-page-header__eyebrow">
            <i class="bi @Model.Icon" aria-hidden="true"></i>
            @Model.Eyebrow
        </span>
        <h1 class="vm-page-header__title" id="vm-page-header-title">@Model.Title</h1>
        <p class="vm-page-header__subtitle">@Model.Subtitle</p>
    </div>
'@
$newPartialContent = @'
    <div class="vm-page-header__main">
        <span class="vm-page-header__icon" aria-hidden="true">
            <i class="bi @Model.Icon"></i>
        </span>

        <div class="vm-page-header__content">
            <span class="vm-page-header__eyebrow">@Model.Eyebrow</span>
            <h1 class="vm-page-header__title" id="vm-page-header-title">@Model.Title</h1>
            <p class="vm-page-header__subtitle">@Model.Subtitle</p>
        </div>
    </div>
'@
$partial = Replace-Exact $partial $oldPartialContent $newPartialContent 'conteúdo da partial de Page Header'
Write-Utf8 $partialPath $partial

# Tokens semânticos e hero escuro conectado ao sidebar.
$cssPath = Join-Path $projectRoot 'wwwroot/css/admin-premium.css'
$css = Read-Utf8 $cssPath
$tokenAnchor = '    --viana-border: 1px solid rgba(17, 24, 39, .08);'
$semanticTokens = @'
    --viana-border: 1px solid rgba(17, 24, 39, .08);
    --viana-surface-dark: var(--viana-graphite-900);
    --viana-surface-dark-deep: var(--viana-black);
    --viana-hero-text: var(--viana-white);
    --viana-hero-muted: rgba(255, 255, 255, .64);
    --viana-hero-border: 1px solid rgba(255, 255, 255, .09);
    --viana-hero-shadow: 0 20px 45px rgba(8, 9, 11, .16);
'@
$css = Replace-Exact $css $tokenAnchor $semanticTokens 'tokens do tema'
$componentPattern = '(?s)\.vm-page-header \{.*?(?=\.stats-card,)'
$componentMatches = [regex]::Matches($css, $componentPattern)
if ($componentMatches.Count -ne 1) { throw "Esperado um bloco vm-page-header; encontrados $($componentMatches.Count)." }
$darkComponentCss = @'
.vm-page-header {
    position: relative;
    isolation: isolate;
    display: flex;
    width: 100%;
    max-width: 1440px;
    min-height: 164px;
    align-items: center;
    justify-content: space-between;
    gap: 32px;
    margin: 0 auto 28px;
    padding: 28px 30px;
    overflow: hidden;
    color: var(--viana-hero-text);
    background:
        radial-gradient(circle at 92% 8%, rgba(196, 0, 0, .24), transparent 27%),
        linear-gradient(135deg, var(--viana-surface-dark) 0%, var(--viana-surface-dark-deep) 100%);
    border: var(--viana-hero-border);
    border-radius: var(--viana-radius);
    box-shadow: var(--viana-hero-shadow);
}

.vm-page-header::before {
    position: absolute;
    top: 28px;
    bottom: 28px;
    left: 0;
    width: 4px;
    background: var(--viana-red);
    border-radius: 0 999px 999px 0;
    content: "";
}

.vm-page-header::after {
    position: absolute;
    z-index: -1;
    top: -95px;
    right: -70px;
    width: 250px;
    height: 250px;
    border: 1px solid rgba(255, 255, 255, .055);
    border-radius: 50%;
    content: "";
}

.vm-page-header__main {
    display: flex;
    min-width: 0;
    align-items: center;
    gap: 20px;
}

.vm-page-header__icon {
    display: inline-flex;
    width: 50px;
    height: 50px;
    flex: 0 0 50px;
    align-items: center;
    justify-content: center;
    color: #ff7b86;
    background: rgba(196, 0, 0, .18);
    border: 1px solid rgba(255, 99, 112, .2);
    border-radius: 14px;
    font-size: 1.18rem;
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, .06);
}

.vm-page-header__content {
    min-width: 0;
    max-width: 800px;
}

.vm-page-header__eyebrow {
    display: block;
    margin-bottom: 8px;
    color: #ff7b86;
    font-size: .69rem;
    font-weight: 900;
    letter-spacing: .14em;
    line-height: 1.2;
    text-transform: uppercase;
}

.vm-page-header__title {
    margin: 0;
    color: var(--viana-hero-text);
    font-size: clamp(2rem, 3.5vw, 2.7rem);
    font-weight: 850;
    letter-spacing: -.035em;
    line-height: 1.05;
    overflow-wrap: anywhere;
}

.vm-page-header__subtitle {
    max-width: 700px;
    margin: 9px 0 0;
    color: var(--viana-hero-muted);
    font-size: .94rem;
    line-height: 1.55;
}

.vm-page-header__actions {
    position: relative;
    z-index: 1;
    display: flex;
    flex: 0 0 auto;
    align-items: center;
    justify-content: flex-end;
    gap: 10px;
}

.vm-page-header__actions .btn {
    display: inline-flex;
    min-height: 46px;
    align-items: center;
    justify-content: center;
    gap: 9px;
    padding: .7rem 1.1rem;
    white-space: nowrap;
}

.vm-page-header__actions .btn i {
    font-size: .92rem;
}

.vm-page-header__actions .btn-outline-secondary {
    color: rgba(255, 255, 255, .84);
    background: rgba(255, 255, 255, .055);
    border-color: rgba(255, 255, 255, .18);
    box-shadow: none;
}

.vm-page-header__actions .btn-outline-secondary:hover,
.vm-page-header__actions .btn-outline-secondary:focus-visible {
    color: var(--viana-white);
    background: rgba(255, 255, 255, .11);
    border-color: rgba(255, 255, 255, .3);
}

@media (max-width: 767.98px) {
    .vm-page-header {
        min-height: 0;
        align-items: stretch;
        flex-direction: column;
        gap: 22px;
        margin-bottom: 22px;
        padding: 24px 22px 22px 26px;
    }

    .vm-page-header::before {
        top: 24px;
        bottom: 24px;
    }

    .vm-page-header__main {
        align-items: flex-start;
        gap: 15px;
    }

    .vm-page-header__icon {
        width: 44px;
        height: 44px;
        flex-basis: 44px;
        border-radius: 12px;
        font-size: 1.05rem;
    }

    .vm-page-header__title {
        font-size: clamp(1.75rem, 9vw, 2.25rem);
    }

    .vm-page-header__actions {
        align-items: stretch;
        flex-direction: column;
    }

    .vm-page-header__actions .btn {
        width: 100%;
        white-space: normal;
    }
}

'@
$css = [regex]::Replace($css, $componentPattern, $darkComponentCss, 1)
Write-Utf8 $cssPath $css

# Correções de codificação e português visível nas Views administrativas.
$textReplacements = [ordered]@{
    'Vis�o geral' = 'Visão geral'
    'gest�o' = 'gestão'
    'informa��es' = 'informações'
    'Cat�logo' = 'Catálogo'
    'Combust�veis' = 'Combustíveis'
    'combust�veis' = 'combustíveis'
    'combust�vel' = 'combustível'
    'Combust�vel' = 'Combustível'
    'dispon�veis' = 'disponíveis'
    'cat�logo' = 'catálogo'
    'respons�veis' = 'responsáveis'
    'condi��es' = 'condições'
    'negocia��es' = 'negociações'
    'negocia��o' = 'negociação'
    'apresenta��o' = 'apresentação'
    'p�blica' = 'pública'
    'CombustÃ­veis' = 'Combustíveis'
    '<th>Email</th>' = '<th>E-mail</th>'
    '<dt class="col-sm-3">Email</dt>' = '<dt class="col-sm-3">E-mail</dt>'
    'Dados do veiculo' = 'Dados do veículo'
    'ViewData["Title"] = "Nova Moto";' = 'ViewData["Title"] = "Nova motocicleta";'
    '<label class="form-label">Fotos da Moto</label>' = '<label class="form-label">Fotos da motocicleta</label>'
    'Salvar Moto' = 'Salvar motocicleta'
    'Adicionar Novas Fotos' = 'Adicionar novas fotos'
    'Enviar Fotos' = 'Enviar fotos'
    'Confirmar Exclusão' = 'Confirmar exclusão'
    '🗑 Confirmar exclusão' = '<i class="bi bi-trash3 me-2"></i>Confirmar exclusão'
    '⭐ Principal' = '<i class="bi bi-star-fill me-1"></i>Principal'
    'Viana Motos <span>/</span> Workspace' = 'Viana Motos <span>/</span> Área administrativa'
    'Viana Motos <strong>·</strong> Workspace comercial' = 'Viana Motos <strong>·</strong> Gestão comercial'
    'Powered by Orizon UI' = 'Tecnologia Orizon UI'
}

$viewFiles = Get-ChildItem -Path $viewsRoot -Recurse -Filter '*.cshtml'
foreach ($viewFile in $viewFiles) {
    $viewContent = Read-Utf8 $viewFile.FullName
    $viewContent = $viewContent.Replace('Combust�veis', 'Combustíveis').Replace('Combust�vel', 'Combustível').Replace('Cat�logo', 'Catálogo')
    foreach ($entry in $textReplacements.GetEnumerator()) {
        $viewContent = $viewContent.Replace($entry.Key, $entry.Value)
    }
    Write-Utf8 $viewFile.FullName $viewContent
}

# Remove a única chamada duplicada identificada na auditoria.
$fuelIndexPath = Join-Path $viewsRoot 'Combustiveis/Index.cshtml'
$fuelIndex = Read-Utf8 $fuelIndexPath
$duplicatePattern = '(?s)^(@model[^\r\n]+\r?\n)\s*@await Html\.PartialAsync\("_PageHeader".*?\}\)\s*(?=@\{)'
$duplicateMatches = [regex]::Matches($fuelIndex, $duplicatePattern)
if ($duplicateMatches.Count -ne 1) { throw "Esperada uma chamada duplicada em Combustiveis/Index; encontradas $($duplicateMatches.Count)." }
$fuelIndex = [regex]::Replace($fuelIndex, $duplicatePattern, '$1', 1)
Write-Utf8 $fuelIndexPath $fuelIndex

# Booleanos deixam de aparecer como True/False na interface.
$booleanReplacements = [ordered]@{
    '<th>Ativa</th>' = '<th>Situação</th>'
    '<th>Ativo</th>' = '<th>Situação</th>'
    '<td>@categoria.Ativa</td>' = '<td><span class="status-badge @(categoria.Ativa ? "is-success" : "is-neutral")">@(categoria.Ativa ? "Ativa" : "Inativa")</span></td>'
    '<td>@marca.Ativa</td>' = '<td><span class="status-badge @(marca.Ativa ? "is-success" : "is-neutral")">@(marca.Ativa ? "Ativa" : "Inativa")</span></td>'
    '<td>@combustivel.Ativo</td>' = '<td><span class="status-badge @(combustivel.Ativo ? "is-success" : "is-neutral")">@(combustivel.Ativo ? "Ativo" : "Inativo")</span></td>'
    '<dt class="col-sm-2">`r`n        Ativa`r`n    </dt>' = '<dt class="col-sm-2">`r`n        Situação`r`n    </dt>'
    '<dt class="col-sm-2">`r`n        Ativo`r`n    </dt>' = '<dt class="col-sm-2">`r`n        Situação`r`n    </dt>'
    '@Model.Ativa' = '@(Model.Ativa ? "Ativa" : "Inativa")'
    '@Model.Ativo' = '@(Model.Ativo ? "Ativo" : "Inativo")'
}

$booleanViews = @(
    'Categorias/Index.cshtml', 'Categorias/Delete.cshtml',
    'Marcas/Index.cshtml', 'Marcas/Delete.cshtml',
    'Combustiveis/Index.cshtml', 'Combustiveis/Delete.cshtml'
)
foreach ($relativePath in $booleanViews) {
    $viewPath = Join-Path $viewsRoot $relativePath
    $viewContent = Read-Utf8 $viewPath
    foreach ($entry in $booleanReplacements.GetEnumerator()) {
        $viewContent = $viewContent.Replace($entry.Key, $entry.Value)
    }
    Write-Utf8 $viewPath $viewContent
}

Write-Output 'Hero escuro e revisão textual aplicados.'
