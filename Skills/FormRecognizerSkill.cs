using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SKFormParser.Skills
{
    internal class FormRecognizerSkill
    {
        #region private

        private const string DEFAULT_MODEL = "prebuilt-read";
        private DocumentAnalysisClient _client;
        private string _model;

        #endregion

        public FormRecognizerSkill(string endpoint, string key, string? model = null) {
            this._client = new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
            this._model = model ?? DEFAULT_MODEL;
        }

        /// <summary>
        /// Extract text from an image file specified by the uri.
        /// </summary>
        /// <param name="uri">Uri of the file</param>
        /// <returns>Text of the file as a string.</returns>
        [SKFunction("Extract text from an image file specified by the uri")]
        [SKFunctionName("RunRecognitionOnUri")]
        public async Task<string> RunRecognitionOnUriAsync(string uri)
        {
            AnalyzeDocumentOperation op = await this._client.AnalyzeDocumentFromUriAsync(
                WaitUntil.Completed,
                this._model,
                new Uri(uri));

            AnalyzeResult result = op.Value;
            return result.Content;
        }

        /// <summary>
        /// Extract text from a local image file specified by the path.
        /// </summary>
        /// <param name="filePath">Path to the local path including the file name</param>
        /// <returns>Text of the file as a string.</returns>
        [SKFunction("Extract text from a local image file specified by the path")]
        [SKFunctionName("RunRecognitionOnLocalFile")]
        public async Task<string> RunRecognitionOnLocalFileAsync(string filePath)
        {
            AnalyzeDocumentOperation op = await this._client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                this._model,
                File.Open(filePath, FileMode.Open));

            AnalyzeResult result = op.Value;
            return result.Content;
        }
    }
}
