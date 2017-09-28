using NFine.Data;
using NFine.Domain.Entity.FileManage;

namespace NFine.Domain.IRepository.FileManage
{
    public interface IAttachmentRepository : IRepositoryBase<AttachmentEntity>
    {
        void DeleteForm(string keyValue);
        void SubmitForm(AttachmentEntity attachmentEntity, string keyValue);
    }
}
