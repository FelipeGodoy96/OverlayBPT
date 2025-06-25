🎮 OverlayBPT - Rastreador de EXP para BPT
OverlayBPT é uma aplicação em C# que monitora a experiência (EXP) em tempo real no jogo BPT (Priston Tale Brasil) usando OCR (Reconhecimento Óptico de Caracteres).
Como o jogo não disponibiliza meios adequados para rastreio de EXP, essa ferramenta surge como solução externa, exibindo um overlay transparente com estatísticas detalhadas sobre seu progresso.

✨ Funcionalidades Principais
🖥️ Captura de Tela Inteligente
Permite selecionar a área do jogo onde o EXP é exibido.

Ideal para diferentes resoluções de monitores.

Modo de seleção ativado com F10.

🔍 Reconhecimento de Texto
Utiliza Tesseract OCR para ler os valores de EXP diretamente da tela.

📊 Estatísticas em Tempo Real
EXP atual formatado (milhões, bilhões, trilhões).

EXP total ganho na sessão.

Média de EXP por hora.

Estimativa de tempo restante para o próximo nível.

⏱️ Monitoramento de Eficiência
Considera apenas o tempo real de caça.

Histórico dos últimos 60 minutos para cálculo de média de EXP/hora.

🖱️ Interface Intuitiva
Overlay transparente e reposicionável.

Modo de seleção de área fácil de usar.

Design minimalista inspirado no HUD nativo do jogo.

▶️ Como Usar
Inicie o aplicativo antes ou durante o jogo.

Pressione F10 para entrar no modo de seleção de área.

Selecione com o mouse a região onde o EXP aparece.

Aguarde a captura automática (padrão: a cada 10 segundos — configurável).

Acompanhe as estatísticas diretamente no overlay.

⌨️ Teclas de Atalho
Tecla	Ação
F10	Ativar/desativar seleção de área
Arrastar	Mover a janela (pela barra de título)
X	Fechar o aplicativo

📦 Pré-requisitos
Windows 7 ou superior

.NET Framework 4.7.2 ou superior

Pacote de idiomas do Tesseract (já incluído)

Jogo BPT rodando em modo janela ou tela cheia

⚙️ Instalação e Execução
Método 1: Executável Pré-Compilado
Baixe o último release na página de releases.

Extraia o arquivo .zip.

Execute OverlayBPT.exe.

Método 2: Compilar a Partir do Código
bash
Copy
Edit
# Clone o repositório
git clone https://github.com/felipegodoy96/OverlayBPT.git

# Acesse a pasta do projeto
cd OverlayBPT

# Abra a solução no Visual Studio
start OverlayBPT.sln

# Compile e execute (Ctrl + F5)
⚠️ Limitações Conhecidas
O EXP precisa estar visível na tela.

Performance pode variar conforme o hardware.

Pode ter dificuldades com fontes pequenas ou baixo contraste.

🗂️ Estrutura do Projeto
bash
Copy
Edit
OverlayBPT/
├── Main.cs            # Ponto de entrada da aplicação
├── OverlayForm.cs     # Lógica principal do overlay
├── tessdata/          # Dados de treinamento do OCR
├── packages/          # Dependências do projeto (Tesseract, etc.)
└── Properties/        # Configurações da aplicação
🤝 Contribuição
Contribuições são bem-vindas! Para colaborar:

Faça um fork do projeto.

Crie uma branch:
git checkout -b feature/nova-feature

Commit suas mudanças:
git commit -m 'Adiciona nova feature'

Envie para seu fork:
git push origin feature/nova-feature

Abra um Pull Request.

📄 Licença
Distribuído sob a licença MIT.
Consulte o arquivo LICENSE para mais informações.

📬 Contato
Felipe Godoy
🔗 LinkedIn
🔗 GitHub do Projeto

⚠️ Este projeto é independente e não possui afiliação com a Zenit Games ou qualquer entidade relacionada ao jogo BPT. Use por sua própria conta e risco. O programa não altera a memória do jogo, portanto, em hipótese alguma pode ser considerado trapaça.
