using NFine.Code;
using NFine.Data;
using NFine.Domain.Entity.FileManage;
using NFine.Domain.IRepository.FileManage;
using NFine.Repository.FileManage;

namespace NFine.Repository.FileManage
{
    public class AttachmentRepository : RepositoryBase<AttachmentEntity>, IAttachmentRepository
    {
        public void DeleteForm(string keyValue)
        {
            if (keyValue.Length == 0) return;

            using (var db = new RepositoryBase().BeginTrans())
            {
                var keyArr = keyValue.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string key in keyArr)
                {
                    AttachmentEntity attachment = db.FindEntity<AttachmentEntity>(key);
                    if (attachment != null)
                    {
                        try
                        {
                            System.IO.File.Delete(attachment.F_Path);
                        }
                        catch { }
                        db.Delete<AttachmentEntity>(attachment);
                    }
                }
                db.Commit();
            }
        }
        public void SubmitForm(AttachmentEntity attachmentEntity, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    db.Update(attachmentEntity);
                }
                else
                {
                    db.Insert(attachmentEntity);
                }
                db.Commit();
            }
        }
    }
}
