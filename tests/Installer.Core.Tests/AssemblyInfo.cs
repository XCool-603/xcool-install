using Xunit;

// WinForms 控件测试不能跨类并行，否则共享的 GDI/Cursor 会在并发加载时产生资源竞争
[assembly: CollectionBehavior(DisableTestParallelization = true)]
