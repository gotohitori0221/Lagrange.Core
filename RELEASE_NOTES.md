# Lagrange.Core V2 / Milky Nightly 构建说明

欢迎使用 Lagrange.Core V2 与 Milky 的最新 Nightly 构建！

这次版本我们花了不少时间做底层的协议对齐、隐蔽 Bug 修复以及跨平台自动化打包。如果你需要开箱即用、无需配置环境的体验，可以直接下载下方对应平台的 **自带运行时（self-contained）** 压缩包；如果你本地已有 .NET 10 环境并希望追求极小体积，可以选择 **不自带运行时（framework-dependent）** 版本。

---

## 🛠️ 本次主要更新与细节打磨

### 1. Milky 协议全面对齐（65 个 API，21 个 Event 满血支持）
- **获取好友历史消息修复**：此前如果不传 `start_message_seq`，接口会直接触发 `NotSupportedException` 报错退出。现在已接入本地消息存储的时序推导，无参数时也能从容拉取最新私聊历史。
- **URL 尾部斜杠容错**：处理了类似 `/api/get_login_info/` 末尾带斜杠导致路由意外 404 的小坑，调用更省心。
- **点赞接口场景扩充**：`send_profile_like` 此前仅从好友列表中检索 UID，现已增加陌生人信息回退查询，现在也能给群成员或非好友正常点赞了。
- **群通知字段规范**：修复了 `get_group_notifications` 在没有下一页序号时 `next_notification_seq` 输出显式 `null` 的问题，严格遵循按需序列化。
- **富媒体与转发消息健全**：理顺了合并转发多层消息的解析与打包逻辑，以及 Markdown 实体防重复渲染。

### 2. 代码库与结构净化
- 彻底清理了历史代码中各种杂乱的调试注释、临时注记与失效文档，整体工程结构更加清爽利落。
- 梳理了 `.gitignore` 与构建依赖，隔离所有本地临时数据库、缓存与构建垃圾。

### 3. 跨平台打包与全架构支持
所有可执行文件均统一打包为纯正的 `.tar.gz` 格式（解压即可直接使用，不再受 GitHub Actions 网页双重套 zip 的困扰）：
- **Windows**：支持 `win-x64`、`win-x86`、`win-arm64`
- **Linux**：支持 `linux-x64`、`linux-arm`（树莓派等 32 位嵌入式）、`linux-arm64`
- **macOS**：支持 `osx-x64`（Intel Mac）、`osx-arm64`（Apple Silicon M 系列）
- **发布版本**：每种架构均分为 `self-contained`（自带运行时）与 `framework-dependent`（依赖已安装的 .NET 10）

---

> 💡 **提示**：Nightly 构建由 CI 自动流转生成。使用过程中遇到任何问题或体验不顺畅的地方，欢迎随时反馈提 Issue！
