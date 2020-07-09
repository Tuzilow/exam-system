# 在线考试系统

本项目是一个简单的在线考试系统，主要有考生考试模块和后台管理模块

![](https://cdn.jsdelivr.net/gh/Tuzilow/blog-image/img/examsystemendhome.png)

![](https://cdn.jsdelivr.net/gh/Tuzilow/blog-image/img/examsystemhome.png)

## 技术栈

- ASP.NET MVC
- [Vue](https://cn.vuejs.org/)
- SQL Server
- [Element-UI](https://element.eleme.cn/#/zh-CN)

## 主要功能

正常的在线考试系统功能，但是因为需求只需要客观题，因此没有手动批改功能。

- 自动生成试卷
- 自动计算分数
- 导出学生成绩

## 权限

### 考生

- 考试
- 查成绩

### 出卷人

- 管理题库
- 管理场次
- 管理题库标签
- 管理用户

### 管理员

- 可以添加内部题目
- 出卷子时可以选择内部题目标签