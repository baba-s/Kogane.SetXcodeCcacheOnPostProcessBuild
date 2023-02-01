using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Kogane
{
    /// <summary>
    /// iOS ビルド完了時に Xcode プロジェクトに ccache を設定するエディタ拡張
    /// https://zenn.dev/pobo380/articles/1a5d838ee857e1
    /// </summary>
    public sealed class SetXcodeCcacheOnPostProcessBuild
    {
        //================================================================================
        // プロパティ(static)
        //================================================================================
        public static Func<bool> OnIsEnable { get; set; } = () => true;

        //================================================================================
        // 関数(static)
        //================================================================================
        /// <summary>
        /// ビルド完了時に呼び出されます
        /// </summary>
        [PostProcessBuild]
        private static void OnPostProcessBuild
        (
            BuildTarget buildTarget,
            string      pathToBuiltProject
        )
        {
            if ( buildTarget != BuildTarget.iOS ) return;
            if ( OnIsEnable == null || !OnIsEnable() ) return;

            const string guid = "b39433204aac24fadb214e7318dfad8b";

            var assetPath = AssetDatabase.GUIDToAssetPath( guid );

            File.Copy
            (
                sourceFileName: assetPath,
                destFileName: $"{pathToBuiltProject}/ccache_wrapper",
                overwrite: true
            );

            var projectPath = PBXProject.GetPBXProjectPath( pathToBuiltProject );
            var project     = new PBXProject();

            project.ReadFromFile( projectPath );

            var targetGuid = project.ProjectGuid();

            project.AddBuildProperty( targetGuid, "CC", "$(PROJECT_DIR)/ccache_wrapper" );
            project.WriteToFile( projectPath );
        }
    }
}