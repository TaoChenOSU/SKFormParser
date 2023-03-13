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

            var localImageOption = new Option<FileInfo?>(
                name: "--local-image",
                description: "The path to an image of a recept stored locally."
            );

            // var setSettingsFileOption = new Option<FileInfo?>(
            //     name: "--set-settings",
            //     description: "Remember the path to the settings file containing the endpoints and the keys to the services."
            // );

            var receiptCommand = new Command("receipt", "Parse a receipt."){settingsFileOption, localImageOption};
            Settings? settings = null;
            FileInfo? imageLocal = null;
            receiptCommand.SetHandler(
                (settingsFile, localImageOption) => {
                    settings = Settings.FromJson(File.ReadAllText(settingsFile!.FullName));
                    imageLocal = localImageOption;
                },
                settingsFileOption, localImageOption
            );

            var rootCommand = new RootCommand("Experimental app for form parsing using the Semantic Kernel.");
            rootCommand.AddCommand(receiptCommand);

            try
            {
                rootCommand.Invoke(args);
                if (settings == null || imageLocal == null)
                {
                    throw new FileNotFoundException("Unable to find settings or image.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            IKernel kernel = Kernel.Builder.Build();
            if (settings.Type == "azure_openai" && settings.AzureOpenAI != null)
            {
                kernel.Config.AddAzureOpenAICompletionBackend(
                    settings.AzureOpenAI.DeploymentLabel,
                    settings.AzureOpenAI.Model,
                    settings.AzureOpenAI.Endpoint, 
                    settings.AzureOpenAI.Key);
            } 
            else if (settings.Type == "openai" && settings.OpenAI != null)
            {
                kernel.Config.AddOpenAICompletionBackend(
                    settings.OpenAI.Model,
                    settings.OpenAI.Model,
                    settings.OpenAI.Key, 
                    settings.OpenAI.Organization);
            }
            else
            {
                Console.WriteLine("Please correctly specify a completion backend.");
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

            var result = await kernel.RunAsync(
                imageLocal.FullName,
                recognizerSkill["RunRecognitionOnLocalFile"],
                formParsingSkill["ParseReceipt"]);

            Console.WriteLine(result);
            return;
        }
    }
}