using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;


namespace NFine.Web.Areas.FileManage.Controllers
{
    public class FileDownloadResult : ActionResult
    {
        public FileDownloadResult(string fileFullPath)
        {
            this.FileFullPath = fileFullPath;
        }

        public FileDownloadResult(string fileName, string fileFullPath)
        {
            this.FileName = fileName;
            this.FileFullPath = fileFullPath;
        }

        public byte[] Content { get; set; }

        public string FileInfoId
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }

        public string FileFullPath
        {
            get;
            private set;
        }

        public string ContentType { get; set; }

        public bool Inline { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                return;
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(this.FileFullPath);
            SetFileName(fileInfo);
            long fileSize = (Content != null ? Content.Length : fileInfo.Length);
            SetResponse(context.HttpContext.Response, fileSize);
            OutputFile(context.HttpContext.Response, fileInfo,Content);
        }

        private void SetFileName(System.IO.FileInfo fileInfo)
        {
            if (string.IsNullOrWhiteSpace(this.FileName))
            {
                this.FileName = fileInfo.Name;
            }
        }

        private static void OutputFile(HttpResponseBase response, System.IO.FileInfo fileInfo, byte[] content)
        {
            if (content!=null)
            {
                response.BinaryWrite(content);
            }
            else
            {
                response.WriteFile(fileInfo.FullName, 0, fileInfo.Length);
            }
            response.Flush();
            response.End();
        }

        private void SetResponse(HttpResponseBase response, long fileSize)
        {
            SetResponseState(response);
            SetResponseHead(response, fileSize);
            SetResponseContent(response);
            if (ContentType!=null)
            {
                response.ContentType = ContentType;
            }
        }

        private static void SetResponseState(HttpResponseBase response)
        {
            response.ClearHeaders();
            response.Clear();
            response.Expires = 0;
            response.Buffer = true;
        }

        private static void SetResponseContent(HttpResponseBase response)
        {
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "Application/octet-stream";
        }

        private void SetResponseHead(HttpResponseBase response, long fileSize)
        {
            response.HeaderEncoding = Encoding.UTF8;
            if (Inline)
            {
                response.AddHeader("Content-Disposition", "inline;filename=" +
                    HttpUtility.UrlEncode(this.FileName, Encoding.UTF8).Replace("+", " "));
            }
            else
            {
                response.AddHeader("Content-Disposition", "attachment;filename=" +
                    HttpUtility.UrlEncode(this.FileName, Encoding.UTF8).Replace("+", " "));
            }
            response.AddHeader("Content-Length", fileSize.ToString());
        }
    }
}
