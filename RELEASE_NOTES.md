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
- **合并转发与特殊字符日志异常修复**：
  - 修复接收合并转发消息解析子消息时，`ResolveContact` / `ResolveReceiver` 因未知消息类型抛出 `NotImplementedException` 导致合并转发解析失败的 BUG，增加 208（语音等）及兜底解析逻辑。
  - 修复当消息内容包含 `{}` 等字符时，`BotContext.Log*` 误将其当做 `string.Format` 格式化占位符导致抛出 `System.FormatException: Input string was not in a correct format` 的崩溃 BUG。
- **历史消息与获取消息全链路提速与 DB 优先机制**：
  - **`get_history_messages` 优先读库**：大幅优化历史消息拉取策略，只要本地 DB 存在记录即直接高速返回，不再因差一两条消息强行向腾讯发起昂贵的网络请求，解决撤回消息拉不动及拉取延迟过高问题。
  - **`get_message` / 引用消息多级缓存加速**：解析回复引用（Reply）时接入本地 `MessageStore`，先查内存 `MessageCache` -> 本地 SQLite `MessageStore` -> 远端网络，杜绝每次查单条消息都等待远程网络请求。
  - **消除 `0xFE1_2` 警告**：群成员信息转换增加 `Uin != 0` 保护，杜绝向陌生人资料接口传 0 导致的 `Error: 30008, Message: kReqUinNil` 警告日志。
- **消息接口性别补全提速（本次重点）**：
  - 此前每条群消息在转换时都会为发送者**单独串行**发起一次 `FetchStranger` 陌生人资料请求以补全性别，导致 `get_history_messages` 拉取 30 条消息需约 21 秒、`get_message` 需 4~5 秒。
  - 现引入群成员性别内存缓存（`ConcurrentDictionary`），同一发送者只请求一次，后续直接命中缓存。
  - 批量拉取历史消息时，先对整批消息的发送者做**受限并发（8 路）预取**，把 N 次串行网络往返压缩为一个并发批次。
  - 单次陌生人请求超时收紧至 3 秒，杜绝个别卡死请求拖垮整批响应；历史消息拉取耗时从 21 秒级降至秒级。
- **`light_app`（小程序卡片 / 音乐卡片）发送卡死修复（本次重点）**：
  - 修复了通过 `send_group_message` 发送 `light_app` 段（如网易云音乐卡片、Ark 卡片）时 HTTP 接口**永久卡住不返回**的严重 BUG。
  - 根因：`BinaryPacket` 以无参构造创建时初始容量为 0，而 `GrowSize` 中的 `while (_offset + additional > _capacity) _capacity *= 2;` 由于 `0 * 2` 恒等于 0 而陷入**死循环**，首次写入即卡死；`LightAppEntity.Build()` 恰好是唯一使用无参构造的调用点，因此只有 `light_app` 发送会卡住。
  - 修复：`GrowSize` 增加零容量兜底（容量为 0 时取 `max(additional, 16)`），并为 `LightAppEntity` 预分配合理初始容量。
- **协议解析健壮性增强**：
  - 修复 `ProtoReader.SkipVarInt` 在缓冲区末尾**越界读取 16 字节**，导致 varint 长度计算错误、进而使子消息（如 `ContentHead`）解析整体错位并抛出 `SkipLengthDelimited size is ... but only ... bytes are available` / `Malformed proto message` 的问题；改为剩余字节不足时走逐字节安全路径。
  - tag 解码统一改用带边界保护的 `DecodeVarInt`，避免缓冲区尾部越界读取。
  - `SocketContext` 在连接断开 / Socket 异常时，将全部挂起请求以异常结束，避免网络中断后 API 调用**永久卡住**。
- **新增 Ark 卡片服务端签名通路 `send_oidb_0xb77`（本次重点）**：
  - 背景：Ark / 音乐卡片等 `light_app` 消息中的 `config.token`（`signedArk`）需要由腾讯服务端签发，本地不存在可复现的签名算法；token 无效时服务端会**静默丢弃**消息（返回 `retcode 0` 但 `message_seq` 为 `0`）。
  - 新增 `OidbSvc.0xb77_9` 协议服务（`Oidb0Xb77Service` / `Oidb0Xb77Event`）及 Milky HTTP 接口 `send_oidb_0xb77`，可直接把复现构造的 OIDB 0xb77 请求体发往腾讯服务端，取回服务端签发的签名数据。
  - 请求参数：`body_hex`（OIDB 0xb77 内层请求体的十六进制字符串）；响应返回 `result`、`message` 与 `body_hex`（服务端响应体十六进制）。
  - `OidbSvc.0xb77_9` 已存在于 PC / Android 签名白名单中，无需额外改动签名链路。

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
