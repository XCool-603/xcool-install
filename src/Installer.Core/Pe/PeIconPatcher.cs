using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Installer.Core.Pe
{
    /// <summary>
    /// 纯托管的 PE 图标替换器。
    /// <para>
    /// 该类型只读写 PE 文件的“资源（.rsrc）”相关部分，不触碰托管元数据（CLI Header）、IL 代码、
    /// 重定位表、导入/导出表等任何其它结构，因此可以安全地用于给已编译的 .NET 程序集替换图标。
    /// </para>
    /// <para>
    /// 支持的输入：PE32（<c>0x10B</c>）与 PE32+（<c>0x20B</c>）格式的可执行文件，以及标准
    /// <c>.ico</c> 容器（含 BMP/DIB 与 PNG 两种图像编码，含 16/32/48/256 等多种尺寸）。
    /// </para>
    /// <para>
    /// 实现要点：
    /// <list type="bullet">
    /// <item><description>完整解析 Type → Name/ID → Language → DataEntry 三层资源目录树，重建后写回。</description></item>
    /// <item><description>只删除 <c>RT_ICON</c>(3) 与 <c>RT_GROUP_ICON</c>(14)，其余资源类型（尤其是
    /// <c>RT_MANIFEST</c>(24)、<c>RT_VERSION</c>(16)）逐字节保留。</description></item>
    /// <item><description>优先原地重写 <c>.rsrc</c> 节；空间不足时在文件末尾新增一个节（默认 <c>.rsrc2</c>）。</description></item>
    /// <item><description>最后一个节之后的附加（overlay）数据原样保留在文件末尾。</description></item>
    /// </list>
    /// </para>
    /// </summary>
    public static class PeIconPatcher
    {
        #region 常量

        /// <summary>DOS 头中 <c>e_lfanew</c> 字段的偏移。</summary>
        private const int DosELfanewOffset = 0x3C;

        /// <summary>NT 头签名 <c>"PE\0\0"</c>。</summary>
        private const uint PeSignature = 0x00004550u;

        /// <summary><c>IMAGE_FILE_HEADER</c> 的长度。</summary>
        private const int FileHeaderSize = 20;

        /// <summary><c>IMAGE_SECTION_HEADER</c> 的长度。</summary>
        private const int SectionHeaderSize = 40;

        /// <summary><c>IMAGE_RESOURCE_DIRECTORY</c> 的长度。</summary>
        private const int ResourceDirectorySize = 16;

        /// <summary><c>IMAGE_RESOURCE_DIRECTORY_ENTRY</c> 的长度。</summary>
        private const int ResourceDirectoryEntrySize = 8;

        /// <summary><c>IMAGE_RESOURCE_DATA_ENTRY</c> 的长度。</summary>
        private const int ResourceDataEntrySize = 16;

        /// <summary><c>RT_ICON</c> 资源类型编号。</summary>
        private const int ResourceTypeIcon = 3;

        /// <summary><c>RT_GROUP_ICON</c> 资源类型编号。</summary>
        private const int ResourceTypeGroupIcon = 14;

        /// <summary>资源目录在可选头数据目录数组中的索引。</summary>
        private const int ResourceDataDirectoryIndex = 2;

        /// <summary>PE32 可选头魔数。</summary>
        private const ushort OptionalHeaderMagicPe32 = 0x10B;

        /// <summary>PE32+ 可选头魔数。</summary>
        private const ushort OptionalHeaderMagicPe32Plus = 0x20B;

        /// <summary>新增节使用的默认名称。</summary>
        private const string NewSectionName = ".rsrc2";

        /// <summary>新增节的特性：<c>IMAGE_SCN_CNT_INITIALIZED_DATA | IMAGE_SCN_MEM_READ</c>。</summary>
        private const uint NewSectionCharacteristics = 0x40000040u;

        /// <summary>可选头中 <c>SizeOfImage</c> 相对可选头起点的偏移（PE32 与 PE32+ 相同）。</summary>
        private const int OptionalHeaderSizeOfImageOffset = 56;

        /// <summary>可选头中 <c>CheckSum</c> 相对可选头起点的偏移（PE32 与 PE32+ 相同）。</summary>
        private const int OptionalHeaderCheckSumOffset = 64;

        #endregion

        #region 公共 API

        /// <summary>
        /// 用 <paramref name="icoBytes"/> 中的图标替换 <paramref name="exeBytes"/> 的图标资源，返回新的完整 exe 字节。
        /// </summary>
        /// <param name="exeBytes">原始 PE 文件的完整字节（不会被修改）。</param>
        /// <param name="icoBytes">图标容器（<c>.ico</c>）的完整字节。</param>
        /// <returns>替换图标后的完整 PE 文件字节。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exeBytes"/> 或 <paramref name="icoBytes"/> 为 <c>null</c>。</exception>
        /// <exception cref="InvalidDataException">输入不是有效 PE，或 <c>.ico</c> 格式非法，或节表空间不足。</exception>
        /// <exception cref="NotSupportedException">PE 没有资源目录，或资源目录树结构超出三层（非标准布局）。</exception>
        public static byte[] ReplaceIcon(byte[] exeBytes, byte[] icoBytes)
        {
            if (exeBytes == null)
            {
                throw new ArgumentNullException("exeBytes", "exe 字节不能为 null。");
            }

            if (icoBytes == null)
            {
                throw new ArgumentNullException("icoBytes", "ico 字节不能为 null。");
            }

            PeImage pe = PeImage.Parse(exeBytes);
            IcoFile ico = IcoFile.Parse(icoBytes);

            int rsrcRva = pe.GetDataDirectoryRva(ResourceDataDirectoryIndex);
            int rsrcSize = pe.GetDataDirectorySize(ResourceDataDirectoryIndex);
            if (rsrcRva == 0 || rsrcSize == 0)
            {
                throw new NotSupportedException(
                    "目标 PE 没有资源目录（数据目录索引 2 的 RVA/Size 为 0），本工具不支持从零创建资源节。");
            }

            SectionHeader rsrcSection = pe.FindSectionByRva(rsrcRva);
            if (rsrcSection == null)
            {
                throw new InvalidDataException(
                    "资源目录 RVA 0x" + rsrcRva.ToString("X8", CultureInfo.InvariantCulture) +
                    " 不属于任何节，PE 结构无效。");
            }

            int offsetInSection = rsrcRva - rsrcSection.VirtualAddress;

            List<ResourceEntry> existing = ResourceTree.Read(pe, rsrcRva);

            // 1) 决定新图标的语言与组图标 ID（尽量沿用原有的，避免资源查找失效）。
            int groupId = 1;
            int? groupLanguage = null;
            int? iconLanguage = null;
            foreach (ResourceEntry entry in existing)
            {
                if (entry.Type.IsString)
                {
                    continue;
                }

                if (entry.Type.Id == ResourceTypeGroupIcon)
                {
                    if (!groupLanguage.HasValue)
                    {
                        groupLanguage = entry.Language;
                        if (!entry.Name.IsString && entry.Name.Id > 0)
                        {
                            groupId = entry.Name.Id;
                        }
                    }
                }
                else if (entry.Type.Id == ResourceTypeIcon)
                {
                    if (!iconLanguage.HasValue)
                    {
                        iconLanguage = entry.Language;
                    }
                }
            }

            int language = groupLanguage ?? iconLanguage ?? MostCommonLanguage(existing);
            if (groupId <= 0 || groupId > 0xFFFF)
            {
                groupId = 1;
            }

            if (ico.Images.Count > 0xFFFF)
            {
                throw new InvalidDataException("图标文件包含 " + ico.Images.Count + " 个图像，超出资源 ID 可表示的范围。");
            }

            // 2) 构造新的资源条目集合：保留除 RT_ICON / RT_GROUP_ICON 之外的一切，再追加新的图标。
            List<ResourceEntry> updated = new List<ResourceEntry>(existing.Count + ico.Images.Count + 1);
            foreach (ResourceEntry entry in existing)
            {
                if (!entry.Type.IsString &&
                    (entry.Type.Id == ResourceTypeIcon || entry.Type.Id == ResourceTypeGroupIcon))
                {
                    continue;
                }

                updated.Add(entry);
            }

            for (int i = 0; i < ico.Images.Count; i++)
            {
                ResourceEntry iconEntry = new ResourceEntry();
                iconEntry.Type = ResKey.FromId(ResourceTypeIcon);
                iconEntry.Name = ResKey.FromId(1 + i);
                iconEntry.Language = language;
                iconEntry.Data = ico.Images[i].Data;
                updated.Add(iconEntry);
            }

            ResourceEntry groupEntry = new ResourceEntry();
            groupEntry.Type = ResKey.FromId(ResourceTypeGroupIcon);
            groupEntry.Name = ResKey.FromId(groupId);
            groupEntry.Language = language;
            groupEntry.Data = BuildGroupIconDirectory(ico, 1);
            updated.Add(groupEntry);

            // 3) 优先原地重写 .rsrc 节；否则在文件末尾新增一个节。
            byte[] inPlace = TryRewriteResourceSectionInPlace(pe, rsrcSection, offsetInSection, updated, rsrcRva, exeBytes);
            if (inPlace != null)
            {
                return inPlace;
            }

            return AppendNewResourceSection(pe, updated, exeBytes);
        }

        /// <summary>
        /// 就地替换文件图标：读取文件、替换图标、写临时文件、再原子替换目标文件。
        /// </summary>
        /// <param name="exePath">要修改的可执行文件路径。</param>
        /// <param name="icoPath">图标文件（<c>.ico</c>）路径。</param>
        /// <exception cref="ArgumentException"><paramref name="exePath"/> 或 <paramref name="icoPath"/> 为空字符串。</exception>
        /// <exception cref="FileNotFoundException">目标文件或图标文件不存在。</exception>
        /// <exception cref="InvalidDataException">PE 或 <c>.ico</c> 格式非法，或节表空间不足。</exception>
        /// <exception cref="NotSupportedException">PE 没有资源目录。</exception>
        public static void ReplaceIcon(string exePath, string icoPath)
        {
            if (string.IsNullOrEmpty(exePath))
            {
                throw new ArgumentException("exePath 不能为空。", "exePath");
            }

            if (string.IsNullOrEmpty(icoPath))
            {
                throw new ArgumentException("icoPath 不能为空。", "icoPath");
            }

            string fullExePath = Path.GetFullPath(exePath);
            string fullIcoPath = Path.GetFullPath(icoPath);

            if (!File.Exists(fullExePath))
            {
                throw new FileNotFoundException("找不到要修改的可执行文件。", fullExePath);
            }

            if (!File.Exists(fullIcoPath))
            {
                throw new FileNotFoundException("找不到图标文件。", fullIcoPath);
            }

            byte[] exeBytes = File.ReadAllBytes(fullExePath);
            byte[] icoBytes = File.ReadAllBytes(fullIcoPath);
            byte[] newBytes = ReplaceIcon(exeBytes, icoBytes);

            string directory = Path.GetDirectoryName(fullExePath);
            if (string.IsNullOrEmpty(directory))
            {
                directory = ".";
            }

            string tempPath = Path.Combine(
                directory,
                Path.GetFileName(fullExePath) + "." + Guid.NewGuid().ToString("N") + ".tmp");

            try
            {
                File.WriteAllBytes(tempPath, newBytes);

                try
                {
                    File.Replace(tempPath, fullExePath, null, true);
                }
                catch (IOException)
                {
                    ReplaceByDeleteAndMove(tempPath, fullExePath);
                }
                catch (UnauthorizedAccessException)
                {
                    ReplaceByDeleteAndMove(tempPath, fullExePath);
                }
                catch (PlatformNotSupportedException)
                {
                    ReplaceByDeleteAndMove(tempPath, fullExePath);
                }
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch (IOException)
                    {
                        // 清理失败不影响主流程。
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // 清理失败不影响主流程。
                    }
                }
            }
        }

        /// <summary>
        /// 判断 exe 中是否存在 <c>RT_ICON</c>(3) 资源。
        /// </summary>
        /// <param name="exeBytes">PE 文件的完整字节。</param>
        /// <returns>存在至少一条 <c>RT_ICON</c> 条目时返回 <c>true</c>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exeBytes"/> 为 <c>null</c>。</exception>
        /// <exception cref="InvalidDataException">输入不是有效 PE。</exception>
        public static bool HasIconResource(byte[] exeBytes)
        {
            return CountIconResources(exeBytes) > 0;
        }

        /// <summary>
        /// 统计 exe 中 <c>RT_ICON</c>(3) 资源的条目数量（供测试断言用）。
        /// </summary>
        /// <param name="exeBytes">PE 文件的完整字节。</param>
        /// <returns><c>RT_ICON</c> 资源条目的数量；没有资源目录时返回 0。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exeBytes"/> 为 <c>null</c>。</exception>
        /// <exception cref="InvalidDataException">输入不是有效 PE。</exception>
        public static int CountIconResources(byte[] exeBytes)
        {
            if (exeBytes == null)
            {
                throw new ArgumentNullException("exeBytes", "exe 字节不能为 null。");
            }

            PeImage pe = PeImage.Parse(exeBytes);
            int rsrcRva = pe.GetDataDirectoryRva(ResourceDataDirectoryIndex);
            int rsrcSize = pe.GetDataDirectorySize(ResourceDataDirectoryIndex);
            if (rsrcRva == 0 || rsrcSize == 0)
            {
                return 0;
            }

            int count = 0;
            List<ResourceEntry> entries = ResourceTree.Read(pe, rsrcRva);
            foreach (ResourceEntry entry in entries)
            {
                if (!entry.Type.IsString && entry.Type.Id == ResourceTypeIcon)
                {
                    count++;
                }
            }

            return count;
        }

        #endregion

        #region 写入策略

        /// <summary>
        /// 尝试把重建后的资源目录原地写回原资源节。空间不足、或该节在资源数据之后还存放着别的数据时返回 <c>null</c>。
        /// </summary>
        /// <remarks>
        /// 之所以要判断“资源数据之后是否还有别的数据”，是因为资源目录未必独占一个 <c>.rsrc</c> 节
        /// （某些加壳/合并工具会把资源塞进 <c>.rdata</c> 之类的节）。若新资源需要向原资源数据范围之外扩张，
        /// 而被覆盖的区间并非全 0（即不是对齐填充），就说明那里有别的数据，此时必须放弃原地写入，
        /// 改用“新增节”策略，以免破坏无关内容。
        /// </remarks>
        private static byte[] TryRewriteResourceSectionInPlace(
            PeImage pe,
            SectionHeader section,
            int offsetInSection,
            List<ResourceEntry> entries,
            int resourceRva,
            byte[] original)
        {
            if (offsetInSection < 0 || section.PointerToRawData <= 0 || section.SizeOfRawData <= 0)
            {
                return null;
            }

            if ((long)section.PointerToRawData + section.SizeOfRawData > original.Length)
            {
                return null;
            }

            byte[] blob = ResourceTree.Build(entries, resourceRva);
            long newEnd = (long)offsetInSection + blob.Length;
            if (newEnd > section.SizeOfRawData)
            {
                return null;
            }

            long oldEnd = (long)offsetInSection + Math.Max(0, pe.GetDataDirectorySize(ResourceDataDirectoryIndex));
            if (oldEnd > section.SizeOfRawData)
            {
                oldEnd = section.SizeOfRawData;
            }

            // 扩张区间必须是纯粹的对齐填充（全 0），否则说明资源数据后面还有别的数据。
            if (newEnd > oldEnd)
            {
                for (long i = oldEnd; i < newEnd; i++)
                {
                    if (original[section.PointerToRawData + (int)i] != 0)
                    {
                        return null;
                    }
                }
            }

            byte[] output = (byte[])original.Clone();
            int start = section.PointerToRawData + offsetInSection;

            // 只清空“新资源所占区间”与“原资源数据区间”的并集，绝不触碰该节里更靠后的其它数据。
            long zeroEnd = Math.Max(newEnd, oldEnd);
            if (zeroEnd > section.SizeOfRawData)
            {
                zeroEnd = section.SizeOfRawData;
            }

            Array.Clear(output, start, (int)(zeroEnd - offsetInSection));
            Buffer.BlockCopy(blob, 0, output, start, blob.Length);

            // 数据目录索引 2 的 RVA 不变，只更新 Size。
            WriteDataDirectory(output, pe, ResourceDataDirectoryIndex, resourceRva, blob.Length);

            // 保持 VirtualSize 与 SizeOfImage 自洽（只在必要时增大，绝不缩小）。
            int neededVirtualSize = offsetInSection + blob.Length;
            if (section.VirtualSize < neededVirtualSize)
            {
                WriteUInt32(output, section.HeaderOffset + 8, (uint)neededVirtualSize);
                section.VirtualSize = neededVirtualSize;
            }

            FixUpSizeOfImage(output, pe, -1, 0);
            FixUpCheckSum(output, pe);
            return output;
        }

        /// <summary>
        /// 在文件末尾新增一个资源节（默认名为 <c>.rsrc2</c>），并把数据目录索引 2 指向它。
        /// </summary>
        private static byte[] AppendNewResourceSection(PeImage pe, List<ResourceEntry> entries, byte[] original)
        {
            int sectionTableEnd = pe.SectionTableOffset + pe.NumberOfSections * SectionHeaderSize;
            int newSectionHeaderOffset = sectionTableEnd;
            int newTableEnd = sectionTableEnd + SectionHeaderSize;

            int firstRawPointer = int.MaxValue;
            int lastRawEnd = 0;
            int lastVirtualEnd = 0;

            foreach (SectionHeader section in pe.Sections)
            {
                if (section.SizeOfRawData > 0 && section.PointerToRawData > 0)
                {
                    if (section.PointerToRawData < firstRawPointer)
                    {
                        firstRawPointer = section.PointerToRawData;
                    }

                    int rawEnd = section.PointerToRawData + section.SizeOfRawData;
                    if (rawEnd > lastRawEnd)
                    {
                        lastRawEnd = rawEnd;
                    }
                }

                int virtualEnd = section.VirtualAddress + Math.Max(section.VirtualSize, section.SizeOfRawData);
                if (virtualEnd > lastVirtualEnd)
                {
                    lastVirtualEnd = virtualEnd;
                }
            }

            if (firstRawPointer == int.MaxValue)
            {
                firstRawPointer = pe.SizeOfHeaders > 0 ? pe.SizeOfHeaders : original.Length;
            }

            int limit = pe.SizeOfHeaders > 0 ? Math.Min(pe.SizeOfHeaders, firstRawPointer) : firstRawPointer;
            if (newTableEnd > limit)
            {
                throw new InvalidDataException(
                    "节表空间不足：新增节表项需要写到文件偏移 0x" + newTableEnd.ToString("X", CultureInfo.InvariantCulture) +
                    "，但头部可用空间只到 0x" + limit.ToString("X", CultureInfo.InvariantCulture) +
                    "（SizeOfHeaders=0x" + pe.SizeOfHeaders.ToString("X", CultureInfo.InvariantCulture) +
                    "，第一个节 PointerToRawData=0x" + firstRawPointer.ToString("X", CultureInfo.InvariantCulture) +
                    "）。请先用链接器预留头部空间（/FILEALIGN 或 /ALIGN）后重试。");
            }

            int newVirtualAddress = AlignUp(lastVirtualEnd, pe.SectionAlignment);
            byte[] blob = ResourceTree.Build(entries, newVirtualAddress);
            int sizeOfRawData = AlignUp(blob.Length, pe.FileAlignment);
            int pointerToRawData = AlignUp(lastRawEnd, pe.FileAlignment);

            int tailStart = lastRawEnd;
            int tailLength = original.Length - tailStart;
            if (tailLength < 0)
            {
                tailLength = 0;
            }

            long totalLength = (long)pointerToRawData + sizeOfRawData + tailLength;
            if (totalLength > int.MaxValue)
            {
                throw new InvalidDataException("生成的文件过大（" + totalLength + " 字节），超出托管数组上限。");
            }

            byte[] output = new byte[(int)totalLength];

            // 结构性内容：[0, 新节原始数据末尾)。新节之前的空洞自动为 0。
            int structuralCopyLength = Math.Min(pointerToRawData, original.Length);
            Buffer.BlockCopy(original, 0, output, 0, structuralCopyLength);
            Buffer.BlockCopy(blob, 0, output, pointerToRawData, blob.Length);

            // 尾部（overlay）数据原样追加到文件末尾。
            if (tailLength > 0)
            {
                Buffer.BlockCopy(original, tailStart, output, pointerToRawData + sizeOfRawData, tailLength);
            }

            // 新节表项
            WriteSectionHeader(
                output,
                newSectionHeaderOffset,
                NewSectionName,
                blob.Length,
                newVirtualAddress,
                sizeOfRawData,
                pointerToRawData,
                NewSectionCharacteristics);

            // NumberOfSections
            WriteUInt16(output, pe.FileHeaderOffset + 2, (ushort)(pe.NumberOfSections + 1));

            // 数据目录索引 2 指向新节
            WriteDataDirectory(output, pe, ResourceDataDirectoryIndex, newVirtualAddress, blob.Length);

            FixUpSizeOfImage(output, pe, newVirtualAddress, Math.Max(blob.Length, sizeOfRawData));
            FixUpCheckSum(output, pe);
            return output;
        }

        /// <summary>
        /// 重新计算并写回 <c>SizeOfImage</c>：取原值与“所有节虚拟末尾对齐后的最大值”中的较大者。
        /// </summary>
        /// <param name="target">目标文件字节。</param>
        /// <param name="pe">已解析的 PE 布局。</param>
        /// <param name="extraVirtualAddress">额外节的虚拟地址；无额外节时传 -1。</param>
        /// <param name="extraVirtualSize">额外节的虚拟大小。</param>
        private static void FixUpSizeOfImage(byte[] target, PeImage pe, int extraVirtualAddress, int extraVirtualSize)
        {
            int required = 0;
            foreach (SectionHeader section in pe.Sections)
            {
                int end = section.VirtualAddress + Math.Max(section.VirtualSize, section.SizeOfRawData);
                if (end > required)
                {
                    required = end;
                }
            }

            if (extraVirtualAddress >= 0)
            {
                int end = extraVirtualAddress + extraVirtualSize;
                if (end > required)
                {
                    required = end;
                }
            }

            required = AlignUp(required, pe.SectionAlignment);
            if (required > pe.SizeOfImage)
            {
                WriteUInt32(target, pe.OptionalHeaderOffset + OptionalHeaderSizeOfImageOffset, (uint)required);
            }
        }

        /// <summary>
        /// 按“原值为 0 就保持 0，非 0 则重算”的规则维护 PE 校验和。
        /// </summary>
        private static void FixUpCheckSum(byte[] target, PeImage pe)
        {
            int offset = pe.OptionalHeaderOffset + OptionalHeaderCheckSumOffset;
            if (ReadUInt32(target, offset) == 0)
            {
                return;
            }

            WriteUInt32(target, offset, ComputeCheckSum(target, offset));
        }

        /// <summary>
        /// 实现标准 PE 校验和算法（等价于 Windows <c>CheckSumMappedFile</c>），计算时把 CheckSum 字段视为 0。
        /// </summary>
        private static uint ComputeCheckSum(byte[] data, int checkSumOffset)
        {
            ulong sum = 0;
            int length = data.Length;
            int index = 0;
            int remaining = length;

            while (remaining > 1)
            {
                ushort word;
                if (index == checkSumOffset || index == checkSumOffset + 2)
                {
                    word = 0;
                }
                else
                {
                    word = (ushort)(data[index] | (data[index + 1] << 8));
                }

                sum += word;
                index += 2;
                remaining -= 2;
                if ((sum & 0xFFFF0000UL) != 0)
                {
                    sum = (sum & 0xFFFFUL) + (sum >> 16);
                }
            }

            if (remaining == 1)
            {
                sum += data[index];
            }

            sum = (sum & 0xFFFFUL) + (sum >> 16);
            sum = (sum & 0xFFFFUL) + (sum >> 16);
            sum += (ulong)length;
            return (uint)(sum & 0xFFFFFFFFUL);
        }

        /// <summary>删除后改名，作为 <see cref="File.Replace(string, string, string, bool)"/> 不可用时的回退方案。</summary>
        private static void ReplaceByDeleteAndMove(string tempPath, string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            File.Move(tempPath, destinationPath);
        }

        #endregion

        #region 资源目录树（读 / 重建）

        /// <summary>PE 资源目录树的解析与重建工具。</summary>
        private static class ResourceTree
        {
            /// <summary>读取整个资源目录树，展开为“类型 / 名称 / 语言 → 数据”的扁平列表。</summary>
            public static List<ResourceEntry> Read(PeImage pe, int resourceRva)
            {
                int baseOffset = pe.RvaToFileOffset(resourceRva);
                if (baseOffset < 0)
                {
                    throw new InvalidDataException(
                        "资源目录 RVA 0x" + resourceRva.ToString("X8", CultureInfo.InvariantCulture) + " 无法映射到文件偏移。");
                }

                List<ResourceEntry> list = new List<ResourceEntry>();
                ReadDirectory(pe, list, baseOffset, baseOffset, 0, ResKey.None, ResKey.None, 0);
                return list;
            }

            private static void ReadDirectory(
                PeImage pe,
                List<ResourceEntry> list,
                int baseOffset,
                int directoryOffset,
                int depth,
                ResKey type,
                ResKey name,
                int language)
            {
                byte[] data = pe.Data;

                if (depth > 2)
                {
                    throw new NotSupportedException(
                        "资源目录树深度超过 3 层（Type → Name → Language），本工具不支持这种非标准布局。");
                }

                if (directoryOffset < 0 || (long)directoryOffset + ResourceDirectorySize > data.Length)
                {
                    throw new InvalidDataException("资源目录结构越界（偏移 0x" + directoryOffset.ToString("X", CultureInfo.InvariantCulture) + "）。");
                }

                int namedCount = ReadUInt16(data, directoryOffset + 12);
                int idCount = ReadUInt16(data, directoryOffset + 14);
                int total = namedCount + idCount;

                if ((long)directoryOffset + ResourceDirectorySize + (long)total * ResourceDirectoryEntrySize > data.Length)
                {
                    throw new InvalidDataException("资源目录条目数组越界。");
                }

                for (int i = 0; i < total; i++)
                {
                    int entryOffset = directoryOffset + ResourceDirectorySize + i * ResourceDirectoryEntrySize;
                    uint nameField = ReadUInt32(data, entryOffset);
                    uint dataField = ReadUInt32(data, entryOffset + 4);

                    ResKey key;
                    if ((nameField & 0x80000000u) != 0)
                    {
                        int stringOffset = baseOffset + (int)(nameField & 0x7FFFFFFFu);
                        if (stringOffset < 0 || (long)stringOffset + 2 > data.Length)
                        {
                            throw new InvalidDataException("资源名称字符串偏移越界。");
                        }

                        int charCount = ReadUInt16(data, stringOffset);
                        if ((long)stringOffset + 2 + (long)charCount * 2 > data.Length)
                        {
                            throw new InvalidDataException("资源名称字符串长度越界。");
                        }

                        key = ResKey.FromString(Encoding.Unicode.GetString(data, stringOffset + 2, charCount * 2));
                    }
                    else
                    {
                        key = ResKey.FromId((int)nameField);
                    }

                    if ((dataField & 0x80000000u) != 0)
                    {
                        int subOffset = baseOffset + (int)(dataField & 0x7FFFFFFFu);
                        if (depth == 0)
                        {
                            ReadDirectory(pe, list, baseOffset, subOffset, 1, key, ResKey.None, 0);
                        }
                        else if (depth == 1)
                        {
                            ReadDirectory(pe, list, baseOffset, subOffset, 2, type, key, 0);
                        }
                        else
                        {
                            if (key.IsString)
                            {
                                throw new NotSupportedException("资源语言层使用了字符串名称，属于非标准资源目录树。");
                            }

                            ReadDirectory(pe, list, baseOffset, subOffset, 3, type, name, key.Id);
                        }

                        continue;
                    }

                    if (depth != 2)
                    {
                        throw new NotSupportedException(
                            "资源目录树结构异常：在第 " + (depth + 1).ToString(CultureInfo.InvariantCulture) + " 层遇到数据项。");
                    }

                    if (key.IsString)
                    {
                        throw new NotSupportedException("资源语言层使用了字符串名称，属于非标准资源目录树。");
                    }

                    int dataEntryOffset = baseOffset + (int)dataField;
                    if (dataEntryOffset < 0 || (long)dataEntryOffset + ResourceDataEntrySize > data.Length)
                    {
                        throw new InvalidDataException("资源数据项越界。");
                    }

                    int dataRva = ReadInt32(data, dataEntryOffset);
                    int dataSize = ReadInt32(data, dataEntryOffset + 4);
                    if (dataSize < 0)
                    {
                        throw new InvalidDataException("资源数据长度为负数，PE 结构无效。");
                    }

                    int dataOffset = pe.RvaToFileOffset(dataRva);
                    if (dataOffset < 0 || (long)dataOffset + dataSize > data.Length)
                    {
                        throw new InvalidDataException(
                            "资源数据 RVA 0x" + dataRva.ToString("X8", CultureInfo.InvariantCulture) +
                            "（长度 " + dataSize.ToString(CultureInfo.InvariantCulture) + "）越界。");
                    }

                    byte[] payload = new byte[dataSize];
                    if (dataSize > 0)
                    {
                        Buffer.BlockCopy(data, dataOffset, payload, 0, dataSize);
                    }

                    ResourceEntry entry = new ResourceEntry();
                    entry.Type = type;
                    entry.Name = name;
                    entry.Language = language;
                    entry.Data = payload;
                    list.Add(entry);
                }
            }

            /// <summary>
            /// 把扁平资源条目列表重建为完整的资源目录二进制块（目录 + 名称字符串 + 数据项 + 载荷）。
            /// </summary>
            /// <param name="entries">资源条目集合。</param>
            /// <param name="baseRva">资源目录自身的 RVA（用于填充数据项中的 <c>OffsetToData</c>）。</param>
            /// <returns>重建后的资源目录字节。</returns>
            public static byte[] Build(List<ResourceEntry> entries, int baseRva)
            {
                DirNode root = new DirNode(ResKey.None);
                foreach (ResourceEntry entry in entries)
                {
                    DirNode typeNode = root.GetOrAddChild(entry.Type);
                    DirNode nameNode = typeNode.GetOrAddChild(entry.Name);
                    DirNode languageNode = nameNode.GetOrAddChild(ResKey.FromId(entry.Language));
                    languageNode.Data = entry.Data;
                }

                root.SortRecursive();

                // 布局：[目录结构][名称字符串][数据项][载荷]
                int cursor = 0;
                AssignDirectoryOffsets(root, ref cursor);
                AssignStringOffsets(root, ref cursor);

                cursor = AlignUp(cursor, 4);

                List<DirNode> leaves = new List<DirNode>();
                CollectLeaves(root, leaves);

                foreach (DirNode leaf in leaves)
                {
                    leaf.DataEntryOffset = cursor;
                    cursor += ResourceDataEntrySize;
                }

                foreach (DirNode leaf in leaves)
                {
                    cursor = AlignUp(cursor, 4);
                    leaf.PayloadOffset = cursor;
                    cursor += leaf.Data == null ? 0 : leaf.Data.Length;
                }

                byte[] blob = new byte[cursor];
                WriteDirectory(root, blob, baseRva);

                foreach (DirNode leaf in leaves)
                {
                    WriteUInt32(blob, leaf.DataEntryOffset, (uint)(baseRva + leaf.PayloadOffset));
                    WriteUInt32(blob, leaf.DataEntryOffset + 4, (uint)(leaf.Data == null ? 0 : leaf.Data.Length));
                    WriteUInt32(blob, leaf.DataEntryOffset + 8, 0);
                    WriteUInt32(blob, leaf.DataEntryOffset + 12, 0);

                    if (leaf.Data != null && leaf.Data.Length > 0)
                    {
                        Buffer.BlockCopy(leaf.Data, 0, blob, leaf.PayloadOffset, leaf.Data.Length);
                    }
                }

                return blob;
            }

            /// <summary>为目录节点分配偏移（叶子节点留给数据项阶段处理）。</summary>
            private static void AssignDirectoryOffsets(DirNode node, ref int cursor)
            {
                if (node.IsLeaf)
                {
                    return;
                }

                node.Offset = cursor;
                cursor += ResourceDirectorySize + node.Children.Count * ResourceDirectoryEntrySize;

                for (int i = 0; i < node.Children.Count; i++)
                {
                    AssignDirectoryOffsets(node.Children[i], ref cursor);
                }
            }

            /// <summary>为字符串名称分配偏移。</summary>
            private static void AssignStringOffsets(DirNode node, ref int cursor)
            {
                if (node.Key.IsString)
                {
                    node.StringOffset = cursor;
                    cursor += 2 + node.Key.Name.Length * 2;
                }

                for (int i = 0; i < node.Children.Count; i++)
                {
                    AssignStringOffsets(node.Children[i], ref cursor);
                }
            }

            /// <summary>按深度优先顺序收集叶子节点。</summary>
            private static void CollectLeaves(DirNode node, List<DirNode> leaves)
            {
                if (node.IsLeaf)
                {
                    if (node.Data != null && node.Key != ResKey.None)
                    {
                        leaves.Add(node);
                    }

                    return;
                }

                for (int i = 0; i < node.Children.Count; i++)
                {
                    CollectLeaves(node.Children[i], leaves);
                }
            }

            /// <summary>写入目录结构与名称字符串。</summary>
            private static void WriteDirectory(DirNode node, byte[] blob, int baseRva)
            {
                if (node.IsLeaf)
                {
                    return;
                }

                int namedCount = 0;
                for (int i = 0; i < node.Children.Count; i++)
                {
                    if (node.Children[i].Key.IsString)
                    {
                        namedCount++;
                    }
                }

                int idCount = node.Children.Count - namedCount;

                WriteUInt32(blob, node.Offset, 0);
                WriteUInt32(blob, node.Offset + 4, 0);
                WriteUInt16(blob, node.Offset + 8, 0);
                WriteUInt16(blob, node.Offset + 10, 0);
                WriteUInt16(blob, node.Offset + 12, (ushort)namedCount);
                WriteUInt16(blob, node.Offset + 14, (ushort)idCount);

                int cursor = node.Offset + ResourceDirectorySize;
                for (int i = 0; i < node.Children.Count; i++)
                {
                    DirNode child = node.Children[i];

                    uint nameField = child.Key.IsString
                        ? (0x80000000u | (uint)child.StringOffset)
                        : (uint)child.Key.Id;

                    uint dataField = child.IsLeaf
                        ? (uint)child.DataEntryOffset
                        : (0x80000000u | (uint)child.Offset);

                    WriteUInt32(blob, cursor, nameField);
                    WriteUInt32(blob, cursor + 4, dataField);
                    cursor += ResourceDirectoryEntrySize;

                    if (child.Key.IsString)
                    {
                        string text = child.Key.Name;
                        WriteUInt16(blob, child.StringOffset, (ushort)text.Length);
                        for (int c = 0; c < text.Length; c++)
                        {
                            WriteUInt16(blob, child.StringOffset + 2 + c * 2, text[c]);
                        }
                    }
                }

                for (int i = 0; i < node.Children.Count; i++)
                {
                    WriteDirectory(node.Children[i], blob, baseRva);
                }
            }
        }

        #endregion

        #region 组图标构造

        /// <summary>
        /// 依据 <c>.ico</c> 内容构造 <c>RT_GROUP_ICON</c>（<c>GRPICONDIR</c>）数据。
        /// </summary>
        /// <param name="ico">已解析的图标容器。</param>
        /// <param name="firstIconId">第一个 <c>RT_ICON</c> 资源 ID。</param>
        private static byte[] BuildGroupIconDirectory(IcoFile ico, int firstIconId)
        {
            int count = ico.Images.Count;
            byte[] buffer = new byte[6 + count * 14];

            WriteUInt16(buffer, 0, 0);              // idReserved
            WriteUInt16(buffer, 2, 1);              // idType = 1 (icon)
            WriteUInt16(buffer, 4, (ushort)count);  // idCount

            int offset = 6;
            for (int i = 0; i < count; i++)
            {
                IcoImage image = ico.Images[i];
                buffer[offset] = image.Width;
                buffer[offset + 1] = image.Height;
                buffer[offset + 2] = image.ColorCount;
                buffer[offset + 3] = 0;                                  // bReserved
                WriteUInt16(buffer, offset + 4, image.Planes);           // wPlanes
                WriteUInt16(buffer, offset + 6, image.BitCount);         // wBitCount
                WriteUInt32(buffer, offset + 8, (uint)image.Data.Length); // dwBytesInRes
                WriteUInt16(buffer, offset + 12, (ushort)(firstIconId + i)); // nID（注意是 WORD，不是 4 字节偏移）
                offset += 14;
            }

            return buffer;
        }

        /// <summary>统计出现次数最多的资源语言；没有资源时返回 0（中性语言）。</summary>
        private static int MostCommonLanguage(List<ResourceEntry> entries)
        {
            if (entries.Count == 0)
            {
                return 0;
            }

            Dictionary<int, int> histogram = new Dictionary<int, int>();
            int bestLanguage = 0;
            int bestCount = 0;

            foreach (ResourceEntry entry in entries)
            {
                int count;
                histogram.TryGetValue(entry.Language, out count);
                count++;
                histogram[entry.Language] = count;

                if (count > bestCount)
                {
                    bestCount = count;
                    bestLanguage = entry.Language;
                }
            }

            return bestLanguage;
        }

        #endregion

        #region 基础读写工具

        /// <summary>向上对齐到 <paramref name="alignment"/> 的整数倍。</summary>
        private static int AlignUp(int value, int alignment)
        {
            if (alignment <= 1)
            {
                return value;
            }

            int remainder = value % alignment;
            return remainder == 0 ? value : value + (alignment - remainder);
        }

        /// <summary>读取小端 16 位无符号整数。</summary>
        private static ushort ReadUInt16(byte[] data, int offset)
        {
            return (ushort)(data[offset] | (data[offset + 1] << 8));
        }

        /// <summary>读取小端 32 位无符号整数。</summary>
        private static uint ReadUInt32(byte[] data, int offset)
        {
            return (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));
        }

        /// <summary>读取小端 32 位有符号整数。</summary>
        private static int ReadInt32(byte[] data, int offset)
        {
            return (int)ReadUInt32(data, offset);
        }

        /// <summary>写入小端 16 位无符号整数。</summary>
        private static void WriteUInt16(byte[] data, int offset, ushort value)
        {
            data[offset] = (byte)(value & 0xFF);
            data[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        /// <summary>写入小端 32 位无符号整数。</summary>
        private static void WriteUInt32(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)(value & 0xFF);
            data[offset + 1] = (byte)((value >> 8) & 0xFF);
            data[offset + 2] = (byte)((value >> 16) & 0xFF);
            data[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        /// <summary>写入一个数据目录项。</summary>
        private static void WriteDataDirectory(byte[] target, PeImage pe, int index, int rva, int size)
        {
            int offset = pe.DataDirectoryOffset + index * 8;
            WriteUInt32(target, offset, (uint)rva);
            WriteUInt32(target, offset + 4, (uint)size);
        }

        /// <summary>写入一个节表项。</summary>
        private static void WriteSectionHeader(
            byte[] target,
            int offset,
            string name,
            int virtualSize,
            int virtualAddress,
            int sizeOfRawData,
            int pointerToRawData,
            uint characteristics)
        {
            for (int i = 0; i < 8; i++)
            {
                target[offset + i] = 0;
            }

            if (!string.IsNullOrEmpty(name))
            {
                int length = Math.Min(name.Length, 8);
                for (int i = 0; i < length; i++)
                {
                    target[offset + i] = (byte)name[i];
                }
            }

            WriteUInt32(target, offset + 8, (uint)virtualSize);
            WriteUInt32(target, offset + 12, (uint)virtualAddress);
            WriteUInt32(target, offset + 16, (uint)sizeOfRawData);
            WriteUInt32(target, offset + 20, (uint)pointerToRawData);
            WriteUInt32(target, offset + 24, 0);
            WriteUInt32(target, offset + 28, 0);
            WriteUInt16(target, offset + 32, 0);
            WriteUInt16(target, offset + 34, 0);
            WriteUInt32(target, offset + 36, characteristics);
        }

        #endregion

        #region 嵌套类型

        /// <summary>资源目录中的“键”：可以是整数 ID，也可以是字符串名称。</summary>
        private sealed class ResKey : IEquatable<ResKey>, IComparable<ResKey>
        {
            /// <summary>根节点的占位键。</summary>
            public static readonly ResKey None = new ResKey(false, 0, null);

            private readonly bool _isString;
            private readonly int _id;
            private readonly string _name;

            private ResKey(bool isString, int id, string name)
            {
                _isString = isString;
                _id = id;
                _name = name;
            }

            /// <summary>是否为字符串名称。</summary>
            public bool IsString
            {
                get { return _isString; }
            }

            /// <summary>整数 ID（<see cref="IsString"/> 为 <c>true</c> 时无意义）。</summary>
            public int Id
            {
                get { return _id; }
            }

            /// <summary>字符串名称（<see cref="IsString"/> 为 <c>false</c> 时为 <c>null</c>）。</summary>
            public string Name
            {
                get { return _name; }
            }

            /// <summary>用整数 ID 构造键。</summary>
            public static ResKey FromId(int id)
            {
                return new ResKey(false, id, null);
            }

            /// <summary>用字符串名称构造键。</summary>
            public static ResKey FromString(string name)
            {
                return new ResKey(true, 0, name);
            }

            /// <inheritdoc />
            public bool Equals(ResKey other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }

                if (_isString != other._isString)
                {
                    return false;
                }

                if (_isString)
                {
                    return string.Equals(_name, other._name, StringComparison.OrdinalIgnoreCase);
                }

                return _id == other._id;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as ResKey);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                if (_isString)
                {
                    return _name == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(_name);
                }

                return _id;
            }

            /// <summary>按 Windows 资源目录约定排序：命名项在前（忽略大小写），ID 项在后并按 ID 升序。</summary>
            public int CompareTo(ResKey other)
            {
                if (ReferenceEquals(other, null))
                {
                    return 1;
                }

                if (_isString != other._isString)
                {
                    return _isString ? -1 : 1;
                }

                if (_isString)
                {
                    return string.Compare(_name, other._name, StringComparison.OrdinalIgnoreCase);
                }

                return _id.CompareTo(other._id);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return _isString ? (_name ?? string.Empty) : _id.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>一条扁平化的资源条目。</summary>
        private sealed class ResourceEntry
        {
            /// <summary>资源类型（整数 ID 或字符串名称）。</summary>
            public ResKey Type;

            /// <summary>资源名称（整数 ID 或字符串名称）。</summary>
            public ResKey Name;

            /// <summary>资源语言 ID。</summary>
            public int Language;

            /// <summary>资源数据。</summary>
            public byte[] Data;
        }

        /// <summary>重建资源目录时使用的树节点。</summary>
        private sealed class DirNode
        {
            private readonly Dictionary<ResKey, DirNode> _index = new Dictionary<ResKey, DirNode>();

            /// <summary>构造节点。</summary>
            public DirNode(ResKey key)
            {
                Key = key;
                Children = new List<DirNode>();
            }

            /// <summary>本节点在父目录中的键。</summary>
            public ResKey Key { get; private set; }

            /// <summary>子节点（已排序）。</summary>
            public List<DirNode> Children { get; private set; }

            /// <summary>本目录结构在资源块中的偏移。</summary>
            public int Offset;

            /// <summary>本节点字符串名称在资源块中的偏移；非字符串键为 -1。</summary>
            public int StringOffset = -1;

            /// <summary>数据项在资源块中的偏移（仅叶子节点有效）。</summary>
            public int DataEntryOffset;

            /// <summary>载荷在资源块中的偏移（仅叶子节点有效）。</summary>
            public int PayloadOffset;

            /// <summary>载荷数据（仅叶子节点有效）。</summary>
            public byte[] Data;

            /// <summary>是否为叶子节点（语言层）。</summary>
            public bool IsLeaf
            {
                get { return Children.Count == 0; }
            }

            /// <summary>按键盘获取或创建子节点。</summary>
            public DirNode GetOrAddChild(ResKey key)
            {
                DirNode child;
                if (_index.TryGetValue(key, out child))
                {
                    return child;
                }

                child = new DirNode(key);
                _index.Add(key, child);
                Children.Add(child);
                return child;
            }

            /// <summary>递归排序子节点，保证“命名项在前、ID 项在后”的约定。</summary>
            public void SortRecursive()
            {
                Children.Sort(CompareNodes);
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].SortRecursive();
                }
            }

            private static int CompareNodes(DirNode a, DirNode b)
            {
                return a.Key.CompareTo(b.Key);
            }
        }

        /// <summary>已解析的 PE 布局信息。</summary>
        private sealed class PeImage
        {
            /// <summary>文件完整字节。</summary>
            public byte[] Data;

            /// <summary>NT 头（<c>e_lfanew</c>）的文件偏移。</summary>
            public int PeHeaderOffset;

            /// <summary><c>IMAGE_FILE_HEADER</c> 的文件偏移。</summary>
            public int FileHeaderOffset;

            /// <summary>可选头的文件偏移。</summary>
            public int OptionalHeaderOffset;

            /// <summary>可选头长度。</summary>
            public int SizeOfOptionalHeader;

            /// <summary>是否为 PE32+。</summary>
            public bool IsPe32Plus;

            /// <summary>节表起始的文件偏移。</summary>
            public int SectionTableOffset;

            /// <summary>节数量。</summary>
            public int NumberOfSections;

            /// <summary>节对齐。</summary>
            public int SectionAlignment;

            /// <summary>文件对齐。</summary>
            public int FileAlignment;

            /// <summary>头部总长度。</summary>
            public int SizeOfHeaders;

            /// <summary>内存映像总长度。</summary>
            public int SizeOfImage;

            /// <summary>数据目录数组的文件偏移。</summary>
            public int DataDirectoryOffset;

            /// <summary>数据目录项数量。</summary>
            public int NumberOfRvaAndSizes;

            /// <summary>节表。</summary>
            public SectionHeader[] Sections;

            /// <summary>解析 PE 头、可选头与节表。</summary>
            public static PeImage Parse(byte[] data)
            {
                if (data == null)
                {
                    throw new ArgumentNullException("data", "PE 字节不能为 null。");
                }

                if (data.Length < 0x40)
                {
                    throw new InvalidDataException(
                        "文件太小（" + data.Length.ToString(CultureInfo.InvariantCulture) + " 字节），不是有效的 PE 文件。");
                }

                if (data[0] != (byte)'M' || data[1] != (byte)'Z')
                {
                    throw new InvalidDataException("缺少 DOS 头签名 \"MZ\"，不是有效的 PE 文件。");
                }

                int peHeaderOffset = ReadInt32(data, DosELfanewOffset);
                if (peHeaderOffset <= 0 || (long)peHeaderOffset + 24 > data.Length)
                {
                    throw new InvalidDataException(
                        "e_lfanew（0x" + peHeaderOffset.ToString("X", CultureInfo.InvariantCulture) + "）越界，PE 头无效。");
                }

                if (ReadUInt32(data, peHeaderOffset) != PeSignature)
                {
                    throw new InvalidDataException("NT 头签名不是 \"PE\\0\\0\"，不是有效的 PE 文件。");
                }

                PeImage pe = new PeImage();
                pe.Data = data;
                pe.PeHeaderOffset = peHeaderOffset;
                pe.FileHeaderOffset = peHeaderOffset + 4;

                pe.NumberOfSections = ReadUInt16(data, pe.FileHeaderOffset + 2);
                pe.SizeOfOptionalHeader = ReadUInt16(data, pe.FileHeaderOffset + 16);

                if (pe.NumberOfSections <= 0)
                {
                    throw new InvalidDataException("PE 文件包含 0 个节，结构无效。");
                }

                pe.OptionalHeaderOffset = pe.FileHeaderOffset + FileHeaderSize;
                if ((long)pe.OptionalHeaderOffset + pe.SizeOfOptionalHeader > data.Length)
                {
                    throw new InvalidDataException("可选头越界，PE 文件被截断。");
                }

                ushort magic = ReadUInt16(data, pe.OptionalHeaderOffset);
                if (magic == OptionalHeaderMagicPe32)
                {
                    pe.IsPe32Plus = false;
                }
                else if (magic == OptionalHeaderMagicPe32Plus)
                {
                    pe.IsPe32Plus = true;
                }
                else
                {
                    throw new InvalidDataException(
                        "不支持的可选头魔数 0x" + magic.ToString("X4", CultureInfo.InvariantCulture) +
                        "，只支持 PE32(0x10B) 与 PE32+(0x20B)。");
                }

                int numberOffset = pe.OptionalHeaderOffset + (pe.IsPe32Plus ? 108 : 92);
                int directoryOffset = pe.OptionalHeaderOffset + (pe.IsPe32Plus ? 112 : 96);
                int minimumOptionalHeaderSize = pe.IsPe32Plus ? 112 : 96;

                if (pe.SizeOfOptionalHeader < minimumOptionalHeaderSize)
                {
                    throw new InvalidDataException("可选头长度不足，无法容纳数据目录。");
                }

                pe.NumberOfRvaAndSizes = ReadInt32(data, numberOffset);
                pe.DataDirectoryOffset = directoryOffset;
                pe.SectionAlignment = ReadInt32(data, pe.OptionalHeaderOffset + 32);
                pe.FileAlignment = ReadInt32(data, pe.OptionalHeaderOffset + 36);
                pe.SizeOfImage = ReadInt32(data, pe.OptionalHeaderOffset + OptionalHeaderSizeOfImageOffset);
                pe.SizeOfHeaders = ReadInt32(data, pe.OptionalHeaderOffset + 60);

                if (pe.SectionAlignment <= 0)
                {
                    throw new InvalidDataException("SectionAlignment 为 0，PE 结构无效。");
                }

                if (pe.FileAlignment <= 0)
                {
                    throw new InvalidDataException("FileAlignment 为 0，PE 结构无效。");
                }

                pe.SectionTableOffset = pe.OptionalHeaderOffset + pe.SizeOfOptionalHeader;
                if ((long)pe.SectionTableOffset + (long)pe.NumberOfSections * SectionHeaderSize > data.Length)
                {
                    throw new InvalidDataException("节表越界，PE 文件被截断。");
                }

                pe.Sections = new SectionHeader[pe.NumberOfSections];
                for (int i = 0; i < pe.NumberOfSections; i++)
                {
                    int offset = pe.SectionTableOffset + i * SectionHeaderSize;
                    SectionHeader section = new SectionHeader();
                    section.HeaderOffset = offset;
                    section.Name = ReadSectionName(data, offset);
                    section.VirtualSize = ReadInt32(data, offset + 8);
                    section.VirtualAddress = ReadInt32(data, offset + 12);
                    section.SizeOfRawData = ReadInt32(data, offset + 16);
                    section.PointerToRawData = ReadInt32(data, offset + 20);
                    section.Characteristics = ReadUInt32(data, offset + 36);
                    pe.Sections[i] = section;
                }

                return pe;
            }

            /// <summary>读取数据目录中某一项的 RVA。</summary>
            public int GetDataDirectoryRva(int index)
            {
                if (index < 0 || index >= NumberOfRvaAndSizes)
                {
                    return 0;
                }

                return ReadInt32(Data, DataDirectoryOffset + index * 8);
            }

            /// <summary>读取数据目录中某一项的 Size。</summary>
            public int GetDataDirectorySize(int index)
            {
                if (index < 0 || index >= NumberOfRvaAndSizes)
                {
                    return 0;
                }

                return ReadInt32(Data, DataDirectoryOffset + index * 8 + 4);
            }

            /// <summary>把 RVA 转换为文件偏移；无法映射时返回 -1。</summary>
            public int RvaToFileOffset(int rva)
            {
                if (rva < 0)
                {
                    return -1;
                }

                for (int i = 0; i < Sections.Length; i++)
                {
                    SectionHeader section = Sections[i];
                    if (section.SizeOfRawData <= 0)
                    {
                        continue;
                    }

                    int virtualSize = Math.Max(section.VirtualSize, section.SizeOfRawData);
                    if (rva >= section.VirtualAddress && rva < section.VirtualAddress + virtualSize)
                    {
                        long offset = (long)section.PointerToRawData + (rva - section.VirtualAddress);
                        if (offset < 0 || offset >= Data.Length)
                        {
                            return -1;
                        }

                        return (int)offset;
                    }
                }

                if (SizeOfHeaders > 0 && rva < SizeOfHeaders && rva < Data.Length)
                {
                    return rva;
                }

                return -1;
            }

            /// <summary>查找包含指定 RVA 的节；找不到返回 <c>null</c>。</summary>
            public SectionHeader FindSectionByRva(int rva)
            {
                for (int i = 0; i < Sections.Length; i++)
                {
                    SectionHeader section = Sections[i];
                    int virtualSize = Math.Max(section.VirtualSize, section.SizeOfRawData);
                    if (rva >= section.VirtualAddress && rva < section.VirtualAddress + virtualSize)
                    {
                        return section;
                    }
                }

                return null;
            }

            private static string ReadSectionName(byte[] data, int offset)
            {
                int length = 0;
                while (length < 8 && data[offset + length] != 0)
                {
                    length++;
                }

                return Encoding.ASCII.GetString(data, offset, length);
            }
        }

        /// <summary>节表项。</summary>
        private sealed class SectionHeader
        {
            /// <summary>节表项在文件中的偏移。</summary>
            public int HeaderOffset;

            /// <summary>节名（最长 8 字节）。</summary>
            public string Name;

            /// <summary>节在内存中的长度。</summary>
            public int VirtualSize;

            /// <summary>节在内存中的 RVA。</summary>
            public int VirtualAddress;

            /// <summary>节在文件中的原始数据长度。</summary>
            public int SizeOfRawData;

            /// <summary>节在文件中的原始数据偏移。</summary>
            public int PointerToRawData;

            /// <summary>节特性标志。</summary>
            public uint Characteristics;
        }

        /// <summary><c>.ico</c> 中的单个图像。</summary>
        private sealed class IcoImage
        {
            /// <summary>宽度（0 表示 256）。</summary>
            public byte Width;

            /// <summary>高度（0 表示 256）。</summary>
            public byte Height;

            /// <summary>调色板颜色数（0 表示真彩色）。</summary>
            public byte ColorCount;

            /// <summary>保留字节。</summary>
            public byte Reserved;

            /// <summary>色彩平面数。</summary>
            public ushort Planes;

            /// <summary>每像素位数。</summary>
            public ushort BitCount;

            /// <summary>图像数据（BMP/DIB 或 PNG）。</summary>
            public byte[] Data;
        }

        /// <summary>已解析的 <c>.ico</c> 容器。</summary>
        private sealed class IcoFile
        {
            /// <summary>图标中的所有图像，按文件中的顺序排列。</summary>
            public List<IcoImage> Images;

            /// <summary>解析 <c>.ico</c> 容器。</summary>
            public static IcoFile Parse(byte[] data)
            {
                if (data == null)
                {
                    throw new ArgumentNullException("data", "ico 字节不能为 null。");
                }

                if (data.Length < 6)
                {
                    throw new InvalidDataException("ico 文件太小（" + data.Length.ToString(CultureInfo.InvariantCulture) + " 字节），缺少 ICONDIR 头。");
                }

                int reserved = ReadUInt16(data, 0);
                int type = ReadUInt16(data, 2);
                int count = ReadUInt16(data, 4);

                if (reserved != 0)
                {
                    throw new InvalidDataException("ico 的 ICONDIR.idReserved 必须为 0，实际为 " + reserved.ToString(CultureInfo.InvariantCulture) + "。");
                }

                if (type != 1)
                {
                    throw new InvalidDataException("ico 的 ICONDIR.idType 必须为 1（图标），实际为 " + type.ToString(CultureInfo.InvariantCulture) + "。");
                }

                if (count <= 0)
                {
                    throw new InvalidDataException("ico 中不包含任何图像（idCount = 0）。");
                }

                long directoryEnd = 6L + (long)count * 16L;
                if (directoryEnd > data.Length)
                {
                    throw new InvalidDataException("ico 的 ICONDIRENTRY 数组越界，文件被截断。");
                }

                IcoFile ico = new IcoFile();
                ico.Images = new List<IcoImage>(count);

                for (int i = 0; i < count; i++)
                {
                    int offset = 6 + i * 16;
                    IcoImage image = new IcoImage();
                    image.Width = data[offset];
                    image.Height = data[offset + 1];
                    image.ColorCount = data[offset + 2];
                    image.Reserved = data[offset + 3];
                    image.Planes = ReadUInt16(data, offset + 4);
                    image.BitCount = ReadUInt16(data, offset + 6);

                    int size = ReadInt32(data, offset + 8);
                    int imageOffset = ReadInt32(data, offset + 12);

                    if (size <= 0)
                    {
                        throw new InvalidDataException("ico 第 " + (i + 1).ToString(CultureInfo.InvariantCulture) + " 个图像的 dwBytesInRes 非法（" + size.ToString(CultureInfo.InvariantCulture) + "）。");
                    }

                    if (imageOffset < 0 || (long)imageOffset + size > data.Length)
                    {
                        throw new InvalidDataException("ico 第 " + (i + 1).ToString(CultureInfo.InvariantCulture) + " 个图像的偏移/长度越界。");
                    }

                    image.Data = new byte[size];
                    Buffer.BlockCopy(data, imageOffset, image.Data, 0, size);

                    ValidateImagePayload(image.Data, i);
                    ico.Images.Add(image);
                }

                return ico;
            }

            /// <summary>校验单个图像载荷是否为可识别的 PNG 或 DIB。</summary>
            private static void ValidateImagePayload(byte[] payload, int index)
            {
                string prefix = "ico 第 " + (index + 1).ToString(CultureInfo.InvariantCulture) + " 个图像";

                if (payload.Length < 8)
                {
                    throw new InvalidDataException(prefix + "的数据长度不足 8 字节，无法识别图像格式。");
                }

                bool isPng = payload[0] == 0x89 && payload[1] == 0x50 && payload[2] == 0x4E && payload[3] == 0x47 &&
                             payload[4] == 0x0D && payload[5] == 0x0A && payload[6] == 0x1A && payload[7] == 0x0A;
                if (isPng)
                {
                    return;
                }

                uint dibHeaderSize = ReadUInt32(payload, 0);
                if (dibHeaderSize != 12 && dibHeaderSize != 40 && dibHeaderSize != 52 && dibHeaderSize != 56 &&
                    dibHeaderSize != 64 && dibHeaderSize != 108 && dibHeaderSize != 124)
                {
                    throw new InvalidDataException(
                        prefix + "既不是 PNG，也不是合法的 BMP/DIB（BITMAPINFOHEADER 大小 = " +
                        dibHeaderSize.ToString(CultureInfo.InvariantCulture) + "）。");
                }

                if (dibHeaderSize > payload.Length)
                {
                    throw new InvalidDataException(prefix + "的 DIB 头长度超过图像数据长度。");
                }
            }
        }

        #endregion
    }
}
