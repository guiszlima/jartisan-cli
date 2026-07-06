using System.IO;
using Jartisan.Application.Ports;

namespace Jartisan.Application.UseCases.Init
{
    public class InitTemplatesUseCase
    {
        private readonly IFileWriter _writer;

        public InitTemplatesUseCase(IFileWriter writer)
        {
            _writer = writer;
        }

        public void Execute(string templatesFolder)
        {
            // A REGRA INDESTRUTÍVEL: Se a pasta física já existir no disco rígido do usuário,
            // nós encerramos a execução imediatamente. Respeita se o usuário deletou o README!
            if (Directory.Exists(templatesFolder))
            {
                return;
            }

            // O fluxo abaixo só roda se for a PRIMEIRA vez absoluta que o CLI cria a pasta no projeto
            string readmeFile = Path.Combine(templatesFolder, "README.txt");

            string readmeContent = 
                "======================================================================\n" +
                " JARTISAN CLI - TEMPLATES CUSTOMIZADOS\n" +
                "======================================================================\n\n" +
                "Coloque seus arquivos de template (.tpl) nesta pasta para estender\n" +
                "ou sobrescrever as gerações nativas do comando 'jartisan make'.\n\n" +
                "Arquivos criados aqui têm prioridade máxima de leitura sobre o binário.\n\n" +
                "Variáveis universais disponíveis para substituição automática:\n" +
                "  {{package}}      -> O pacote correto do Java (ex: com.jartisan.controller)\n" +
                "  {{ClassName}}    -> O nome da classe informado no terminal (ex: Usuario)\n" +
                "  {{endpointName}} -> O nome em kebab-case automatizado para rotas (ex: ordem-servico)\n\n" +
                "Exemplo de uso para criar um template novo 'handler.tpl':\n" +
                "  jartisan make handler CriarUsuario\n";

            // O seu FileWriter assume o controle técnico e faz a pasta nascer de fábrica com o manual
            _writer.Write(readmeFile, readmeContent);
        }
    }
}
