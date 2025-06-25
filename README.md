OverlayBPT - Rastreador de EXP para BPT

OverlayBPT é uma aplicação C# que monitora a experiência (EXP) em tempo real no jogo BPT (Priston Tale Brasil) usando reconhecimento óptico de caracteres (OCR). Uma vez que a empresa do jogo não disponibiliza de meios adequados para o rastreio de experiência para cálculo de métricas surge então a necessidade de uma aplicação externa para isso. O programa exibe um overlay transparente com estatísticas detalhadas sobre seu progresso no jogo.

Funcionalidades Principais
🖥️ Captura de tela inteligente: Seleciona a área do jogo onde o EXP é exibido
Devido a variação na resolução de monitores, é aconselhável definir por conta própria a área de captura da experiência.

🔍 Reconhecimento de texto: Utiliza Tesseract OCR para ler valores de EXP da tela

📊 Estatísticas em tempo real:

EXP atual formatado (milhões, bilhões, trilhões)

EXP total ganho na sessão

Média de EXP por hora

Tempo restante estimado para próximo nível

⏱️ Monitoramento de eficiência:

Tempo ativo real (considera apenas períodos de hunt)

Histórico de EXP dos últimos 60 minutos para cálculo de média por hora

🖱️ Interface intuitiva:

Overlay transparente e reposicionável

Modo de seleção de área (F10)

Design minimalista que não atrapalha a jogabilidade e simula layout de HUDs do cliente nativo.

Como Usar
Inicie o aplicativo antes ou durante o jogo

Pressione F10 para entrar no modo de seleção de área

Selecione a região da tela onde o EXP é exibido (arraste com o mouse)

Aguarde a captura automática 
Por default: 10 segundos - você pode ajustar de acordo com o que achar que funciona melhor para si.

Acompanhe as estatísticas no overlay transparente

Teclas de Atalho
Tecla	Ação
F10	Ativar/desativar seleção de área
Arrastar	Mover a janela (arrastar pela barra de título)
X	Fechar o aplicativo
Pré-requisitos
Windows 7 ou superior

.NET Framework 4.7.2 ou superior

Pacote de idiomas Tesseract (já incluído)

Jogo BPT em execução em modo janela ou tela cheia

Instalação e Execução
Método 1: Executável pré-compilado
Baixe o último release na página de releases

Extraia o arquivo ZIP

Execute OverlayBPT.exe

Método 2: Compilar a partir do código
bash
# Clone o repositório
git clone https://github.com/felipegodoy96/OverlayBPT.git

# Abra a solução no Visual Studio
cd OverlayBPT
start OverlayBPT.sln

# Compile e execute (Ctrl+F5)
Limitações Conhecidas
Requer que os valores de EXP estejam visíveis na tela

Performance pode variar dependendo do hardware

Pode ter dificuldade com fontes muito pequenas ou contrastes baixos


Estrutura do Projeto
OverlayBPT/
├── Main.cs            # Ponto de entrada da aplicação
├── OverlayForm.cs     # Lógica principal do overlay
├── tessdata/          # Dados de treinamento do OCR
├── packages/          # Dependências do projeto (Tesseract, etc.)
└── Properties/        # Configurações da aplicação
Contribuição
Contribuições são bem-vindas! Siga estes passos:

Faça um fork do projeto

Crie uma branch para sua feature (git checkout -b feature/nova-feature)

Commit suas mudanças (git commit -m 'Adiciona nova feature')

Push para a branch (git push origin feature/nova-feature)

Abra um Pull Request

Licença
Distribuído sob a licença MIT. Veja LICENSE para mais informações.

Contato
Felipe Godoy - https://www.linkedin.com/in/felipegodoy-dev/

Link do Projeto: https://github.com/felipegodoy96/OverlayBPT

Nota: Este projeto é independente e não possui afiliação com a Zenit Games ou qualquer entidade relacionada ao jogo BPT. Use por sua própria conta e risco. O programa não altera memória do jogo, portanto, em hipótese nenhuma pode ser considerado nocivo ou trapaça.
