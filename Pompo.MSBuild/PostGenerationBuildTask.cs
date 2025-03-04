using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace Pompo
{
    /// <summary>
    /// MSBuild task for JS transmitter uncommenting and copying it to a target destination.
    /// </summary>
    public class PostGenerationBuildTask : Task
    {
        /// <summary>
        /// Generated source file path.
        /// </summary>
        [Required]
        public string Src { get; set; }

        /// <summary>
        /// Target file path.
        /// </summary>
        [Required]
        public string Dest { get; set; }

        /// <summary>
        /// Executes task.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            File.WriteAllText(
                Dest,
                File.ReadAllText(Src).Replace("/*", string.Empty).Replace("*/", string.Empty)
            );

            Log.LogMessage(MessageImportance.High, $"Pompo JS transmitter has uncommented and copied:");
            Log.LogMessage(MessageImportance.High, $"\tfrom {Src}");
            Log.LogMessage(MessageImportance.High, $"\tto {Dest}");

            return true;
        }
    }
}