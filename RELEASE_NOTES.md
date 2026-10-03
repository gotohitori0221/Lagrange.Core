# Lagrange.Milky 社区维护版 - Nightly 构建说明

> 📢 **声明**：本项目属于**社区维护版本（非官方版本）**，由开源社区爱好者根据 Milky 协议规范进行跟进、重构与修复，纯属为爱发电。

---

大家好！这是 Lagrange.Milky 的最新 Nightly 构建版本。

如果你只是想开箱即用、不想在机器上折腾任何 .NET 环境，直接在下方 Assets 列表里下载你对应系统和芯片的 **`self-contained`（自带运行时）** 压缩包即可，解压后直接运行；如果你本机已经装好了 .NET 10，也可以选择体积小很多的 **`framework-dependent`** 版本。全部包体均为直接的 `.tar.gz` 格式。

---

## 💡 本次更新内容（做了一些微小的工作）

### 1. Milky 协议全量对齐与异常修复
- **好友历史消息查询平滑化**：修复了调用 `get_history_messages`（场景为好友）且不传 `start_message_seq` 时直接炸 `NotSupportedException` 的问题。现在本地存储会自动根据最新记录推导起始序列号，无参也能正常拉取好友消息记录。
- **URL 路径结尾斜杠容错**：处理了类似 `/api/get_login_info/` 末尾带 `/` 时路由直接返回 404 的问题，增加了路径清洗。
- **名片点赞接口支持陌生人**：此前 `send_profile_like` 只查好友列表，给群友或非好友点赞会因为找不到目标而报错。现在增加了陌生人信息回退机制，群成员点赞也能正常工作了。
- **群通知字段按需序列化**：修复了 `get_group_notifications` 在没有下一页通知序号时显式吐出 `"next_notification_seq": null` 的毛病，严格遵循规范仅在有值时返回。
- **富媒体与转发消息健全**：理顺了合并转发多层消息的解析与打包逻辑，以及 Markdown 实体防重复渲染。
- **图片重复解析与旧版 URL 修复**：彻底解决了发送图片时因协议兼容垫片被重复解析导致一条消息出现两个图片实体（且一个为 0 字节）的 BUG；优化了图片 URL 解析逻辑，不再误用老旧的 `gchat.qpic.cn` 链接，统一自动向腾讯多媒体接口换取官方高可用 NT 下载直链（`https://multimedia.nt.qq.com.cn/download?appid=...`）。
- **群成员性别 (`sex`) 自动补全**：修复了通过 `get_group_member_info` 获取成员信息时，由于底层 OIDB 0xfe7_3 协议缺少性别属性导致 `sex` 恒为 `unknown` 的问题。现在当性别未知时，系统会自动通过 `FetchStranger` 异步补全真实性别。
- **私聊消息撤回 (`recall_private_message`) 序列号修正**：区分了私聊协议中的 C2C 序列号 (`ClientSequence`) 与事件序列号 (`Sequence`)，修复了私聊撤回时发送错误序列号导致的撤回报错与失败。
- **动画表情 (`sub_type: sticker`) 真正生效**：此前即便指定 `sub_type` 为 `sticker`，发出去的仍是一张普通图片。现在参考 acidify 的实现，在图片上传请求中正确写入 `PicExtBizInfo.BizType` 以及真实的 `PbReserve{subType}` 子消息（同时兼容 C2C 与群聊的两种保留字段），好友与群聊发动画表情都能被客户端正确识别为表情包；接收侧解析也同步支持从 `PbReserve` 兜底判断 `sub_type`。
- **运行时内存深度优化**：
  - 针对此前内存常驻高达 120MB+ 的问题进行了专项治理，将 .NET 默认的 Server GC 调整为轻量化的 Workstation GC（工作站单堆模式），并开启 `RetainVMGarbageCollection=false`，杜绝未使用的虚拟内存长期滞留。
  - 禁用了 Tiered PGO 避免在长驻运行时额外产生分支插桩内存开销。
  - 限制 SQLite 内部页面缓存大小并开启 WAL 模式。
  - 引入了 `MemoryManagementService`，在登录启动完成后及后台运行期间自动对启动阶段产生的瞬态垃圾进行深度压缩回收（Windows 系统配合释放工作集），大幅压低物理内存与工作集占用。

### 2. 代码仓库整洁化
- 彻底清理了代码中散落的历史注释、无用废弃说明以及调试标记，代码结构利落干净。
- 移除了无用的中间缓存及多余构建垃圾，专注交付稳定运行的二进制。

### 3. 纯正单文件构建（Single-File）与全平台支持
- **真正的一键单文件**：开启了 `-p:PublishSingleFile=true` 与原生自解压嵌入，去除了所有零散 PDB 符号及零散依赖 DLL，压缩包解压后**只有唯一的一个独立可执行文件**（Windows 下为 `Lagrange.Milky.exe`，Linux/macOS 下为 `Lagrange.Milky`），开箱即用，极其清爽！
- **全平台架构覆盖**：只针对 `Lagrange.Milky` 构建，支持所有主流桌面与服务器平台：
  - **Windows**：`win-x64`、`win-x86`、`win-arm64`
  - **Linux**：`linux-x64`、`linux-arm`（支持树莓派等 32 位嵌入式）、`linux-arm64`
  - **macOS**：`osx-x64`（Intel Mac）、`osx-arm64`（Apple Silicon M 系列）

---

如果在日常使用中遇到问题，欢迎向仓库反馈 Issue！
