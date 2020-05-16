using System.Web;
using System.Web.Optimization;

namespace ExaminationSystem
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备就绪，请使用 https://modernizr.com 上的生成工具仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new ScriptBundle("~/bundles/vue").Include(
                      "~/Scripts/vue.js"));
            bundles.Add(new ScriptBundle("~/bundles/element").Include(
                      "~/Scripts/ElementUI/element-ui.js"));
            bundles.Add(new StyleBundle("~/Content/elementcss").Include(
                      "~/Content/ElementUI/element-ui.css"));
            bundles.Add(new ScriptBundle("~/bundles/axios").Include(
                      "~/Scripts/axios.js"));
            bundles.Add(new StyleBundle("~/Content/quillcss").Include(
                      "~/Content/quill/quill.core.css",
                      "~/Content/quill/quill.snow.css",
                      "~/Content/quill/quill.bubble.css"));
            bundles.Add(new ScriptBundle("~/bundles/quill").Include(
                      "~/Scripts/quill/quill.js"));
            bundles.Add(new ScriptBundle("~/bundles/vue-quill").Include(
                      "~/Scripts/vue-quill-editor.js"));

            // 组件
            bundles.Add(new ScriptBundle("~/component/new-user").Include(
                      "~/Components/new-user.js"));
            bundles.Add(new ScriptBundle("~/component/user-table").Include(
                      "~/Components/user-table.js"));
            bundles.Add(new ScriptBundle("~/component/score-table").Include(
                      "~/Components/score-table.js"));
            bundles.Add(new ScriptBundle("~/component/exam-table").Include(
                      "~/Components/exam-table.js"));
            bundles.Add(new ScriptBundle("~/component/change-user-exam").Include(
                      "~/Components/change-user-exam.js"));
            bundles.Add(new ScriptBundle("~/component/new-exam").Include(
                      "~/Components/new-exam.js"));
            bundles.Add(new ScriptBundle("~/component/new-paper").Include(
                      "~/Components/new-paper.js"));
            bundles.Add(new ScriptBundle("~/component/new-question").Include(
                      "~/Components/new-question.js"));
            bundles.Add(new ScriptBundle("~/component/paper-table").Include(
                      "~/Components/paper-table.js"));
            bundles.Add(new ScriptBundle("~/component/question-table").Include(
                      "~/Components/question-table.js"));
            bundles.Add(new ScriptBundle("~/component/tag-control").Include(
                      "~/Components/tag-control.js"));
        }
    }
}
