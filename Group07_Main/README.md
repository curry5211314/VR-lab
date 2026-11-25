# TiCeNX

HandType=L_Hand / R_Hand（分别对应左侧 / 右侧训练）。

## 1. 环境与打开方式

| 项目 | 配置 |
| --- | --- |
| 工程目录 | `E:\VR\TiCeNX` |
| Unity | `2022.3.62f1c1` |
| 产品名 | `TiCeNX` |
| Android 应用标识 | `com.cc.TiCeNX` |
| 唯一主场景 | `Assets/Standard main.unity` |
| 渲染管线 | URP `14.0.12` |

## 2. 本地操作说明

1. 佩戴并校准设备，确认追踪与活动空间正常。
2. 在 `Popup_Net` 的臂长滑块设定 50–70 cm，首次默认 60 cm；通过 `shoulderLeng` 保存到本机。必须在生成目标之前设置，之后修改不会自动重新生成已有目标。
3. 选择本组训练，进入训练界面并观看现有示范/引导。
4. 使用开始按钮生成目标，同时启动 CSV 记录。开始按钮随后禁用，防止重复生成。
5. 按引导触碰目标并观察计分和奖励。参考按钮切换参考人物前移/还原，使用参考前移功能后，只是将星星以及识别人体骨骼画面前移，不影响碰撞箱的位置。
6. 结束时通过返回菜单按钮停止并保存记录，再加载主场景。

## 3. 数据记录与导出

当前采集器是 `Assets/Scripts/Tool/DoubleSpace2Csv.cs`，按约 30 Hz 的定时逻辑采样；开始生成目标时启动记录，返回菜单时停止并写入文件。

- 目录：`Application.persistentDataPath/APPData`。
- 文件名：`FitRecord_{MMdd_HHmmss}_{动作名称}_{fileIndex:D2}.csv`。
- Android 通常为 `/storage/emulated/0/Android/data/com.cc.TiCeNX/files/APPData`，以设备实际路径与权限为准。
- 保存后通过允许的文件访问方式导出；已有 ADB 访问条件时可用 `adb pull /sdcard/Android/data/com.cc.TiCeNX/files/APPData <电脑目标目录>`。

| CSV 列 | 含义 |
| --- | --- |
| User | 设备标识，不是人工登记的受试者编号；共享前注意脱敏 |
| Action / Difficulty | 动作名称与当前难度 |
| Arm length / HandType | 启动时臂长（cm）及原代码的训练侧别字段 |
| DateTime | 本地时间 `MM-dd HH:mm:ss`，非高精度时间戳 |
| Score | 累计分数 |
| StarL / StarR | 左右目标触发索引，不是奖励星数 |
| Pain | 周期记录 NONE，手柄事件 L/R，不是数值量表 |
| frameIndex | 内部序号 |
| 左右肩、肘、手 XYZ | 共 18 列，世界坐标，不是关节角度 |

## 4. 工程结构

| 位置 | 用途 |
| --- | --- |
| Assets/Standard main.unity | 唯一主场景与对象绑定 |
| Assets/Scripts/MainControl.cs | 本组流程、臂长、计分、返回及记录 |
| Assets/Scripts/ActionButton.cs | 本组课程选择 |
| Assets/Scripts/StarCreater.cs、Star.cs | 本组目标生成和公共碰撞 |
| Assets/Tools/_IKDemno/IKControl.cs | 公共 IK 与本组实际使用的示范方法 |
| Assets/Scripts/Old | 公共身体追踪 |
| Assets/Scripts/UDP_Pad | 平板连接与远程命令 |
| Assets/Scripts/Tool | CSV 与公共辅助类 |
| Packages、ProjectSettings | SDK 与项目配置 |
