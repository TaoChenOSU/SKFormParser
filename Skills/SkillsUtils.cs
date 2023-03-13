using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SKFormParser.Skills
{
    internal class SkillsUtils
    {
        private const string SKILLS_FOLDER = "Skills";
        internal static string GetSkillsFolderPath(int maxAttempts = 10)
        {
            var currPath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            string targetPath;
            bool found;
            do
            {
                targetPath = Path.Join(currPath, SKILLS_FOLDER);
                found = Directory.Exists(targetPath);
                currPath = Path.GetFullPath(Path.Combine(currPath, ".."));
            } while (maxAttempts-- > 0 && !found);

            if (!found)
            {
                throw new FileNotFoundException(
                    "Skills directory not found. The app needs the skills from the repo to work.");
            }

            return targetPath;
        }
    }
}
