using NFine.Application.FileManage;
using NFine.Code;
using NFine.Domain.Entity.FileManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.IO;

namespace NFine.Web.Areas.FileManage.Controllers
{
    public class AttachmentController : ControllerBase
    {
        private AttachmentApp attachmentApp = new AttachmentApp();

        [HttpGet]
        [HandlerAuthorize(false)]
        public override ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize(false)]
        public override ActionResult Form()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize(false)]
        public override ActionResult Details()
        {
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            var data = new
            {
                rows = attachmentApp.GetList(pagination, keyword),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = attachmentApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(AttachmentEntity attachmentEntity, string keyValue)
        {
            attachmentApp.SubmitForm(attachmentEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAuthorize(false)]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            attachmentApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }

        [HandlerAuthorize(false)]
        public ActionResult Download(string id, string contentType, bool inline = false)
        {
            byte[] content = null;
            AttachmentEntity attachment = attachmentApp.GetForm(id);
            if (attachment != null)
                return new FileDownloadResult(attachment.F_Name + "." + attachment.F_ExtName, attachment.F_Path) { Content = content, ContentType = contentType, Inline = inline };
            else
                return null;
        }

        [HttpPost]
        [HandlerAuthorize(false)]
        public void UploadForm()
        {
            string result = "";
            try
            {
                // 获取上传文件保存路径
                string savePath = Configs.GetValue("UploadFileSavePath");
                if (string.IsNullOrWhiteSpace(savePath))
                    savePath = "~/Content/Attachments";
                string directory = "";
                // 判断是否绝对路径，此处认为只要路径中包含:竟是绝对路径，否则就是虚拟路径
                if (savePath.IndexOf(':') > 0)
                    directory = savePath;
                else
                    directory = Server.MapPath(savePath);
                this.FileLog.Debug("\r\n文件保存配置路径：[" + savePath + "]  \r\n文件保存实际路径：[" + directory + "]");
                // 判断路径是否存在，不存在则创建该路径
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extName = Path.GetExtension(file.FileName);
                    string newFileName = System.Guid.NewGuid().ToString() + extName;
                    string newFilePath = Path.Combine(directory, newFileName);
                    file.SaveAs(newFilePath);

                    AttachmentEntity attachment = new AttachmentEntity();
                    attachment.F_Size = file.ContentLength;
                    attachment.F_Name = fileName;
                    attachment.F_ExtName = extName.TrimStart(new char[] { '.' });
                    attachment.F_Path = newFilePath;
                    attachment.F_Description = "";
                    attachmentApp.SubmitForm(attachment, "");
                }

                result += "<script>";
                result += "parent.reloadGridData();";
                result += "alert('上传已完成！');";
                result += "</script>";
            }
            catch (System.Exception e1)
            {
                result += "<script>";
                result += "alert('上传失败！原因：{0}');";
                result += "</script>";
                result = string.Format(result, e1.Message.Replace("\\", "\\\\"));
            }

            Response.Write(result);
            Response.End();
        }
    }
}
