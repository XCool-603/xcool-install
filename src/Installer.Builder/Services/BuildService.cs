using System;
using Installer.Abstractions.Packaging;
using Installer.Core.Packaging;

namespace Installer.Builder.Services
{
    /// <summary>
    /// 打包服务。
    ///
    /// 把"调用 <see cref="PackageBuilder"/>"这件事从 Presenter 里抽出来：
    /// ① Presenter 只负责"界面状态 ↔ 工程文件"，不直接 new 打包器；
    /// ② 测试时可以换成假的实现，不必真的落盘。
    ///
    /// 注意：本文件属于 Services 层，**不允许** <c>using System.Windows.Forms;</c>。
    /// </summary>
    public class BuildService
    {
        /// <summary>执行打包。</summary>
        public virtual PackageBuildResult Build(PackageBuildOptions options,
                                                IProgress<BuildProgress> progress = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            return new PackageBuilder().Build(options, progress);
        }

        /// <summary>打包器版本（写进清单的 build 段）。</summary>
        public virtual string PackerVersion
        {
            get { return PackageBuilder.PackerVersion; }
        }
    }
}
