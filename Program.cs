using System.CommandLine;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using SKFormParser.Skills;

namespace SKFormParser
{
    class Program
    {
        static async Task Main(string[] args) {
            var settingsFileOption = new Option<FileInfo?>(
                name: "--settings",
                description: "The path to the settings file containing the endpoints and the keys to the services."
            );

            // var setSettingsFileOption = new Option<FileInfo?>(
            //     name: "--set-settings",
            //     description: "Remember the path to the settings file containing the endpoints and the keys to the services."
            // );

            var rootCommand = new RootCommand(
                "Experimental app for form parsing using the Semantic Kernel."
            );
            rootCommand.AddOption(settingsFileOption);
            // rootCommand.AddOption(setSettingsFileOption);

            Settings? settings = null;
            rootCommand.SetHandler(
                (file) => {
                    settings = Settings.FromJson(File.ReadAllText(file!.FullName)); 
                },
                settingsFileOption
            );

            // rootCommand.SetHandler(
            //     (file) => {},
            //     setSettingsFileOption
            // );

            try
            {
                rootCommand.Invoke(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            IKernel kernel = Kernel.Builder.Build();
            if (settings!.Type == "azure_openai")
            {
                kernel.Config.AddAzureOpenAICompletionBackend(
                    settings.AzureOpenAI!.DeploymentLabel,
                    settings.AzureOpenAI.Model,
                    settings.AzureOpenAI.Endpoint, 
                    settings.AzureOpenAI.Key);
            } 
            else if (settings?.Type == "openai")
            {
                kernel.Config.AddOpenAICompletionBackend(
                    settings.OpenAI!.Model,
                    settings.OpenAI.Model,
                    settings.OpenAI.Key, 
                    settings.OpenAI.Organization);
            }
            else
            {
                Console.WriteLine();
                Environment.Exit(1);
            }

            var recognizerSkill = kernel.ImportSkill(
                new FormRecognizerSkill(
                    settings.recognizer.Endpoint,
                    settings.recognizer.Key
                ),
                "FormRecognizer");

            var formParsingSkill = kernel.ImportSemanticSkillFromDirectory(
                SkillsUtils.GetSkillsFolderPath(),
                "FormParsingSkill");

            var pathToFile = @"C:\Users\taochen\Downloads\MicrosoftTeams-image.png";
            var result = await kernel.RunAsync(
                pathToFile,
                recognizerSkill["RunRecognitionOnLocalFile"],
                formParsingSkill["ParseReceipt"]);

            Console.WriteLine(result);
            return;
        }
    }
}