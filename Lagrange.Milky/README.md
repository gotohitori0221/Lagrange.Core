<div align="center">

![Lagrange.Milky](./Resources/banner.svg)

_[Milky](https://github.com/SaltifyDev/milky) protocol implementation based on [Lagrange.Core V2](https://github.com/LagrangeDev/LagrangeV2)_

</div>

## Document

- [Milky 官方规范文档](https://milky.ntqqrev.org/)
- [Lagrange.Milky Document](https://lagrangedev.github.io/Lagrange.Milky.Document)

## Sign Server appinfo_v2 获取逻辑

Lagrange.Milky 在启动阶段（`ConfigureLagrange`）集成了向签名服务器动态拉取客户端协议指纹的机制：

1. **配置检查**：检查 `Lagrange.Protocol.Signer.NormalizedBaseUrl` 是否已配置有效地址。
2. **远程拉取**：若配置了签名服务器地址，在程序初始化 `BotContext` 前，通过 `HttpSigner.FetchAppInfoAsync` 向签名端发起 `GET sign/sec-sign/appinfo_v2` 请求（支持配置 `ProxyUrl` 代理与 `Authorization: Bearer <token>` 鉴权头）。
3. **协议参数注入**：成功获取后，反序列化 `SignerResponse<AppInfoResult>`，动态提取系统与签名参数并构造 `BotAppInfo`：
   - 平台与环境：`Os`、`Kernel`、`VendorOs`、`Qua`
   - 版本标识：`CurrentVersion`、`PtVersion`、`SsoVersion`、`AppClientVersion`
   - 包与签名信息：`PackageName`、`ApkSignatureMd5`、`AppId`、`SubAppId`
   - 登录 SDK 信息：`WtLoginSdkInfo`（`SdkBuildTime`、`SdkVersion`、`MiscBitMap`、`SubSigMap`、`MainSigMap`）
4. **优先级与回退机制**：
   - **最高优先级**：Sign Server 动态返回的 `appinfo_v2`；
   - **次级优先级**：用户在本地 `appsettings.json` 的 `Protocol.AppInfo` 中自定义覆盖的参数；
   - **保底回退**：若远程请求失败或未配置 Signer，自动回退到 Core 库内置的默认硬编码协议信息（`BotAppInfo.ProtocolToAppInfo[Platform]`）。
5. **作用与价值**：使 Bot 运行时协议版本、QUA 指纹与远程签名服务算法版本严格对齐，有效规避因本地协议硬编码版本过旧而导致的签名校验失败或风控拦截。

---

## 实现情况清单 (Milky v1.3)

### 通信层 (Communication)

- [x] **HTTP API** (`POST /api/:api`)：支持 Bearer Token / query 鉴权、CORS、标准 HTTP 状态码（200/401/404/415）及 JSON 响应规范
- [x] **SSE** (`GET /event`)：支持 `text/event-stream`、心跳保活、Token 鉴权、正确排除 WebSocket 升级请求
- [x] **WebSocket** (`ws://.../event`)：支持 Upgrade 请求升级与实时双向/单向事件流
- [x] **WebHook**：支持并发向多个目标 URL 发起事件推送信标（带 Bearer 鉴权）

---

### API 接口实现情况（全量 65/65）

#### 系统相关 (System)
- [x] `get_login_info`（实时走 OIDB 0xfe1_2 拉取最新账号资料，修复缓存导致老昵称问题）
- [x] `get_impl_info`（返回 Lagrange.Core、协议平台及 Milky 1.3 版本信息）
- [x] `get_user_profile`（获取用户/好友/陌生人详细资料）
- [x] `get_friend_list`
- [x] `get_friend_info`
- [x] `get_group_list`
- [x] `get_group_info`
- [x] `get_group_member_list`
- [x] `get_group_member_info`
- [x] `get_peer_pins`（获取置顶会话）
- [x] `set_peer_pin`（设置/取消会话置顶）
- [x] `set_avatar`（设置机器人自身头像）
- [x] `set_nickname`（修改昵称，成功后同步刷新本地内存缓存）
- [x] `set_bio`（设置个性签名）
- [x] `get_custom_face_url_list`（获取大表情资源列表）
- [x] `get_cookies`
- [x] `get_csrf_token`

#### 消息相关 (Message)
- [x] `send_private_message`
- [x] `send_group_message`
- [x] `recall_private_message`
- [x] `recall_group_message`
- [x] `get_message`
- [x] `get_history_messages`
- [x] `get_resource_temp_url`
- [x] `get_forwarded_messages`（获取合并转发消息）
- [x] `mark_message_as_read`（标记消息已读）

#### 好友相关 (Friend)
- [x] `send_friend_nudge`（好友戳一戳）
- [x] `send_profile_like`（个人资料点赞）
- [x] `delete_friend`（删除好友）
- [x] `get_friend_requests`（获取好友申请列表）
- [x] `accept_friend_request`（同意好友申请）
- [x] `reject_friend_request`（拒绝好友申请）

#### 群组相关 (Group)
- [x] `set_group_name`（修改群名）
- [x] `set_group_avatar`（修改群头像）
- [x] `set_group_member_card`（修改群名片）
- [x] `set_group_member_special_title`（设置专属头衔）
- [x] `set_group_member_admin`（设置/取消群管理员）
- [x] `set_group_member_mute`（群成员禁言）
- [x] `set_group_whole_mute`（全员禁言）
- [x] `kick_group_member`（踢出群成员）
- [x] `get_group_announcements`（获取群公告列表）
- [x] `send_group_announcement`（发布群公告）
- [x] `delete_group_announcement`（删除群公告）
- [x] `get_group_essence_messages`（获取精华消息列表）
- [x] `set_group_essence_message`（设置/移除精华消息）
- [x] `quit_group`（退出群聊）
- [x] `send_group_message_reaction`（发送群消息表情回应）
- [x] `send_group_nudge`（群内戳一戳）
- [x] `get_group_notifications`（获取群系统通知列表）
- [x] `accept_group_request`（同意入群申请）
- [x] `reject_group_request`（拒绝入群申请）
- [x] `accept_group_invitation`（接受入群邀请）
- [x] `reject_group_invitation`（拒绝入群邀请）

#### 文件相关 (File)
- [x] `upload_private_file`（上传私聊文件，正常返回真实 `file_id`）
- [x] `upload_group_file`（上传群文件，返回 `file_id`）
- [x] `get_private_file_download_url`（支持 `is_self_send` 区分自身/好友发送的文件）
- [x] `get_group_file_download_url`（获取群文件下载直链）
- [x] `get_group_files`（获取群文件/文件夹列表）
- [x] `move_group_file`（移动群文件）
- [x] `rename_group_file`（重命名群文件，使用 OIDB 0x6d6_4 与 Reserved=1）
- [x] `delete_group_file`（删除群文件）
- [x] `create_group_folder`（创建群文件夹，返回生成的 `folder_id`）
- [x] `rename_group_folder`（重命名群文件夹）
- [x] `delete_group_folder`（删除群文件夹）
- [x] `persist_group_file`（转存群文件为永久文件，使用 OIDB 0x6d9_0，Milky 1.3）

---

### 事件类型实现情况（全量 21/21 类）

- [x] `bot_offline`（机器人离线）
- [x] `message_receive`（接收好友消息、群消息、临时会话消息）
- [x] `message_recall`（好友/群消息撤回）
- [x] `peer_pin_change`（会话置顶状态改变）
- [x] `friend_request`（收到好友申请）
- [x] `group_join_request`（收到加群申请）
- [x] `group_invited_join_request`（成员邀请他人入群申请）
- [x] `group_invitation`（自身收到入群邀请）
- [x] `friend_nudge`（好友戳一戳事件）
- [x] `friend_file_upload`（好友文件上传事件，已打通 MsgType 529）
- [x] `group_admin_change`（群管理员变动）
- [x] `group_essence_message_change`（群精华消息变动）
- [x] `group_member_increase`（群成员增加）
- [x] `group_member_decrease`（群成员减少）
- [x] `group_disband`（群解散事件，Milky 1.3）
- [x] `group_name_change`（群名称变更）
- [x] `group_message_reaction`（群消息表情回应）
- [x] `group_mute`（群成员禁言事件）
- [x] `group_whole_mute`（群全员禁言事件）
- [x] `group_nudge`（群内戳一戳事件）
- [x] `group_file_upload`（群文件上传事件）

---

### 消息段实现情况 (Segments)

#### 接收消息段 (IncomingSegment)
- [x] `text`（纯文本）
- [x] `mention`（@特定用户）
- [x] `mention_all`（@全体成员）
- [x] `face`（基础/超级表情）
- [x] `reply`（回复引用，附带原消息内容链）
- [x] `image`（图片，含图片临时直链及宽高尺寸）
- [x] `record`（语音，含临时直链及播放时长）
- [x] `video`（短视频，含临时直链及播放时长）
- [x] `file`（私聊/群文件消息段，含 `file_id`、文件名、大小、哈希）
- [x] `forward`（合并转发卡片）
- [x] `market_face`（商城表情）
- [x] `light_app`（小程序/Ark 消息）
- [x] `xml`（XML 结构消息）
- [x] `markdown`（原生 Markdown 消息，Milky 1.3）

#### 发送消息段 (OutgoingSegment)
- [x] `text`（发送纯文本）
- [x] `mention`（发送 @用户）
- [x] `mention_all`（发送 @全体成员）
- [x] `face`（发送表情/超级表情）
- [x] `reply`（引用回复）
- [x] `image`（发送图片，支持 `file://`、`http(s)://`、`base64://`）
- [x] `record`（发送语音，支持 `file://`、`http(s)://`、`base64://`）
- [x] `video`（发送视频，支持 `file://`、`http(s)://`、`base64://`）
- [x] `forward`（发送合并转发，支持自定义标题、摘要及消息列表）
- [x] `light_app`（发送小程序 JSON）
