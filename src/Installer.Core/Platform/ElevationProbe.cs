using System;
using System.IO;

namespace Installer.Core.Platform
{
    /// <summary>
    /// 判断"当前用户能不能往这个目录里写"。
    ///
    /// 用途：在**开始安装之前**就判断要不要提权。
    ///
    /// 以前只有"安装范围 = 所有用户"才会提权 —— 用户手工把安装目录改成
    /// <c>C:\Program Files\XXX</c> 时不会提权，于是安装跑到一半才失败，
    /// 前面已经建好的目录/写好的注册表还要靠回滚收拾。
    /// 现在改成：先探一下这个目录能不能写，不能写就在**进向导之前**提权。
    ///
    /// 实现上直接"试着建个临时文件再删掉" —— 这是唯一能同时覆盖
    /// 目录 ACL、只读属性、父目录权限、网络共享权限的判断方式。
    /// 读 ACL 的做法看着更"优雅"，但漏掉的情况太多（继承、Deny 项、
    /// 所有者、EFS……），而且慢。
    /// </summary>
    public static class ElevationProbe
    {
        /// <summary>
        /// 当前用户能否在 <paramref name="dir"/> 下创建文件。
        /// 目录还不存在时，看**最近的已存在祖先目录**能不能写。
        /// </summary>
        /// <param name="dir">目标安装目录（可以不存在）。</param>
        /// <returns>能写返回 true；参数为空、路径非法、探测异常都返回 true（宁可少提一次权）。</returns>
        public static bool CanWrite(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return true;
            }

            string probeDir;
            try
            {
                probeDir = NearestExistingAncestor(dir);
            }
            catch (Exception)
            {
                return true;
            }

            if (string.IsNullOrEmpty(probeDir))
            {
                // 连盘符都不存在（比如 U 盘没插）—— 这里不该由"提权"来背锅，
                // 交给后续步骤去报"路径不可用"。
                return true;
            }

            var probe = Path.Combine(probeDir, ".wm-write-probe-" + Guid.NewGuid().ToString("N") + ".tmp");

            try
            {
                using (var fs = new FileStream(probe, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose))
                {
                    fs.WriteByte(0);
                }

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                // 只读介质、磁盘满、被安全软件拦……都当"写不了"处理：
                // 提权后重试一次总比装到一半失败强。
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// 从 <paramref name="dir"/> 往上找第一个**已存在**的目录。
        /// 全都找不到时返回 null（盘符不存在）。
        /// </summary>
        public static string NearestExistingAncestor(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return null;
            }

            var current = Path.GetFullPath(Environment.ExpandEnvironmentVariables(dir));

            while (!string.IsNullOrEmpty(current))
            {
                if (Directory.Exists(current))
                {
                    return current;
                }

                var parent = Path.GetDirectoryName(current);
                if (string.IsNullOrEmpty(parent) || string.Equals(parent, current, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                current = parent;
            }

            return null;
        }
    }
}
